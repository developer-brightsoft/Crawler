// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Crawler.CrawlingServer
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using ExcavatorSharp.Common;
using ExcavatorSharp.Excavator;
using ExcavatorSharp.ExtensionMethods;
using ExcavatorSharp.Licensing;
using ExcavatorSharp.Objects;

namespace ExcavatorSharp.Crawler
{
    /// <summary>
    /// Main crawling server for website unloading
    /// </summary>
    public class CrawlingServer
    {
    	/// <summary>
    	/// Delegate for PageCrawled event
    	/// </summary>
    	/// <param name="CrawledPageData">Crawled page data</param>
    	public delegate void PageCrawledHandler(PageCrawledCallback CrawledPageData);

    	/// <summary>
    	/// Delegate for RobotsTxtParsedHandler event
    	/// </summary>
    	/// <param name="CrawledPageData">Crawled page data</param>
    	public delegate void RobotsTxtParsedHandler(DERobotsTxtParsedCallback RobotsParsedData);

    	/// <summary>
    	/// Delegate for SitemapParsedHandler event
    	/// </summary>
    	/// <param name="SitemapParsedData"></param>
    	public delegate void SitemapParsedHandler(DESitemapParsedCallback SitemapParsedData);

    	/// <summary>
    	/// Crawling errors counter
    	/// </summary>
    	internal volatile int MetricsSessionTotalCrawlingErrorsCount;

    	/// <summary>
    	/// Name of parent task
    	/// </summary>
    	public string ParentTaskName { get; private set; }

    	/// <summary>
    	/// Target directory for data saving
    	/// </summary>
    	public string ParentTaskOperatingDirectory { get; private set; }

    	/// <summary>
    	/// Actual server status
    	/// </summary>
    	public CrawlingServerStatus ActualServerStatus { get; private set; }

    	/// <summary>
    	/// Crawling website base url, defining at constructing of class
    	/// </summary>
    	public Uri WebsiteBaseUrl { get; private set; }

    	/// <summary>
    	/// Crawler properties
    	/// </summary>
    	public CrawlingServerProperties CrawlerProperties { get; private set; }

    	/// <summary>
    	/// Presents robots.txt file, if allowed into CrawlerProperties
    	/// </summary>
    	public DERobotsTxtFile WebsiteRobotsTxtData { get; private set; }

    	/// <summary>
    	/// Presents sitemap file, if specified
    	/// </summary>
    	public DESitemapFile WebsiteSitemapData { get; private set; }

    	/// <summary>
    	/// Threads for pages crawling
    	/// </summary>
    	private List<CrawlingThreadBase> PageCrawlingThreads { get; set; }

    	/// <summary>
    	/// The buffer of the crawlinkg links
    	/// </summary>
    	internal CrawlingServerLinksBuffer LinksBuffer { get; private set; }

    	/// <summary>
    	/// Thread for links buffer auto-saving
    	/// </summary>
    	private Thread LinksBufferHddSavingThread { get; set; }

    	/// <summary>
    	/// Thread for reindexation links after some time
    	/// </summary>
    	private Thread LinksReindexationAnalysingThread { get; set; }

    	/// <summary>
    	/// Thread for making robots.txt and sitemap files reindexation
    	/// </summary>
    	private Thread RoutineTasksRunningThread { get; set; }

    	/// <summary>
    	/// Link to TaskLogger, hosted at DataExcavatorTask
    	/// </summary>
    	internal DataExcavatorTasksLogger TaskLoggerLink { get; set; }

    	/// <summary>
    	/// Cookie container for all child requests
    	/// </summary>
    	internal CookieContainer ChildRequestsCookieContainer { get; set; }

    	/// <summary>
    	/// Average time for page crawling.
    	/// </summary>
    	public double PageCrawlingTimeSessionAverage
    	{
    		get
    		{
    			int num = 0;
    			double num2 = 0.0;
    			foreach (double pagesCrawlingTimesSecond in PagesCrawlingTimesSeconds)
    			{
    				num2 += pagesCrawlingTimesSecond;
    				num++;
    			}
    			if (num == 0)
    			{
    				num = 1;
    			}
    			return num2 / (double)num;
    		}
    	}

    	/// <summary>
    	/// Set of pages crawling times for actual crawling session
    	/// </summary>
    	private ConcurrentBag<double> PagesCrawlingTimesSeconds { get; set; }

    	/// <summary>
    	/// Mutex used for proxies sequencive rotation. 
    	/// </summary>
    	internal Mutex ProxySequenciveRotationMutex { get; set; }

    	/// <summary>
    	/// Lock for keeping Crawl-Delay directive
    	/// </summary>
    	private Mutex CrawlDelayDistributionMutex { get; set; }

    	/// <summary>
    	/// Last lock for CrawlDelayDistributionMutex
    	/// </summary>
    	private DateTime LastCrawlingRequestDateTime { get; set; }

    	/// <summary>
    	/// Links buffer HDD saving mutex
    	/// </summary>
    	private Mutex LinksBufferHDDSavingMutex { get; set; }

    	/// <summary>
    	/// Main event, that will happen after next page been crawled.
    	/// </summary>
    	public event PageCrawledHandler PageCrawled;

    	/// <summary>
    	/// Event fires when robots.txt parsed
    	/// </summary>
    	public event RobotsTxtParsedHandler RobotsTxtParsed;

    	/// <summary>
    	/// Event fires when sitemap parsed
    	/// </summary>
    	public event SitemapParsedHandler SitemapParsed;

    	/// <summary>
    	/// Creates a new instance of crawling server
    	/// </summary>
    	/// <param name="WebsiteUrl">Url of the crawling website</param>
    	/// <param name="CrawlerProperties">Crawling server properties</param> 
    	/// <param name="ParentTaskName">Parent task name</param>
    	/// <param name="ParentTaskOperatingDirectory">Parent task operating directory</param>
    	/// <param name="TaskLoggerLink">[NULLABLE] Task logger link</param>
    	public CrawlingServer(string WebsiteUrl, CrawlingServerProperties CrawlerProperties, DataExcavatorTasksLogger TaskLoggerLink, string ParentTaskName, string ParentTaskOperatingDirectory)
    	{
    		this.ParentTaskName = ParentTaskName;
    		this.ParentTaskOperatingDirectory = ParentTaskOperatingDirectory;
    		ActualServerStatus = CrawlingServerStatus.Waiting;
    		LinksBuffer = null;
    		if (CrawlerProperties.StoreCrawledData)
    		{
    			CrawlingDataIO crawlingDataIO = new CrawlingDataIO(this);
    			OverloadLinksBuffer(crawlingDataIO.TryToLoadLinksBuffer());
    		}
    		if (LinksBuffer == null)
    		{
    			LinksBuffer = new CrawlingServerLinksBuffer();
    		}
    		Uri uri = new Uri(WebsiteUrl);
    		WebsiteBaseUrl = new Uri($"{uri.Scheme.ToLower()}://{uri.Authority.ToLower()}");
    		this.CrawlerProperties = CrawlerProperties;
    		PageCrawlingThreads = new List<CrawlingThreadBase>(this.CrawlerProperties.CrawlingThreadsCount);
    		Random random = new Random();
    		int num = 100;
    		int num2 = this.CrawlerProperties.CrawlingThreadDelayMilliseconds * 2;
    		if (num > num2)
    		{
    			num = 0;
    		}
    		ProxySequenciveRotationMutex = new Mutex();
    		CrawlDelayDistributionMutex = new Mutex();
    		LinksBufferHDDSavingMutex = new Mutex();
    		for (int i = 0; i < this.CrawlerProperties.CrawlingThreadsCount; i++)
    		{
    			int threadInitialSleepingTime = random.Next(num, num2);
    			CrawlingThreadBase crawlingThreadBase = null;
    			crawlingThreadBase = ((this.CrawlerProperties.PrimaryDataCrawlingWay != 0) ? ((CrawlingThreadBase)new CrawlingThreadCEF(this, threadInitialSleepingTime, this.CrawlerProperties.TakeScreenshotAfterPageLoaded)) : ((CrawlingThreadBase)new CrawlingThreadNative(this, threadInitialSleepingTime)));
    			PageCrawlingThreads.Add(crawlingThreadBase);
    			crawlingThreadBase.PageCrawled += NewCrawlingThread_PageCrawledFromChildrenThread;
    		}
    		PagesCrawlingTimesSeconds = new ConcurrentBag<double>();
    		this.TaskLoggerLink = TaskLoggerLink;
    		if (this.CrawlerProperties.HTTPWebRequestProxiesList == null || this.CrawlerProperties.HTTPWebRequestProxiesList.Count == 0)
    		{
    			ChildRequestsCookieContainer = new CookieContainer();
    		}
    	}

    	/// <summary>
    	/// Overloades links buffer by some specified buffer.
    	/// </summary>
    	/// <param name="LinkBuffer"></param>
    	public void OverloadLinksBuffer(CrawlingServerLinksBuffer LinksBuffer)
    	{
    		this.LinksBuffer = LinksBuffer;
    	}

    	/// <summary>
    	/// Thread body for links-buffer auto-saving
    	/// </summary>
    	private void AutoSaveLinksBufferOnHDDThreadBody()
    	{
    		while (!DEConfig.ShutDownProgram)
    		{
    			SaveLinksBufferOnHDD();
    			Thread.Sleep(CrawlerProperties.LinksBufferHDDAutoSavingMilliseconds);
    		}
    	}

    	/// <summary>
    	/// Thread body for reindexation already indexed links
    	/// </summary>
    	private void LinksReindexationAnalysingThreadBody()
    	{
    		while (!DEConfig.ShutDownProgram)
    		{
    			AnalyseOutdatedLinks();
    			Thread.Sleep(180000);
    		}
    	}

    	/// <summary>
    	/// Analyses outdated links and sends them to the re-crawling buffer
    	/// </summary>
    	private void AnalyseOutdatedLinks()
    	{
    		DateTime CurrentDateTime = DateTime.Now;
    		IEnumerable<PageLink> enumerable = LinksBuffer.CrawledLinks.AsParallel().WithDegreeOfParallelism(CrawlerProperties.ConcurrentCollectionsParallelismQuantity).Where(delegate(PageLink NextLink)
    		{
    			int result;
    			if (!NextLink.PageAtRecrawlingProcess)
    			{
    				_ = NextLink.LinkLastCrawlingDateAndTime;
    				if (NextLink.LinkOwnerType == PageLinkOwnerType.InnerLink && NextLink.LinkResourceType == PageLinkResourceType.ContentPageLink)
    				{
    					result = (((CurrentDateTime - NextLink.LinkLastCrawlingDateAndTime).TotalMinutes >= (double)CrawlerProperties.ReindexCrawledPagesAfterSpecifiedMinutesInterval) ? 1 : 0);
    					goto IL_0056;
    				}
    			}
    			result = 0;
    			goto IL_0056;
    			IL_0056:
    			return (byte)result != 0;
    		});
    		foreach (PageLink item in enumerable)
    		{
    			item.PageAtRecrawlingProcess = true;
    			LinksBuffer.LinksToCrawl.Enqueue(item);
    		}
    		TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"There are {enumerable.Count()} links marked as outdated and sended to re-crawling");
    	}

    	/// <summary>
    	/// Thread body for routine tasks running - robots.txt and sitemap recrawling
    	/// </summary>
    	private void RoutineTasksRunningThreadBody()
    	{
    		while (!DEConfig.ShutDownProgram)
    		{
    			TryToLoadRobotsTxtFile();
    			TryToReloadSitemapFile();
    			Thread.Sleep(180000);
    		}
    	}

    	/// <summary>
    	/// Saves actual links buffer state
    	/// </summary>
    	internal void SaveLinksBufferOnHDD()
    	{
    		if (CrawlerProperties.StoreCrawledData)
    		{
    			LinksBufferHDDSavingMutex.WaitOne();
    			CrawlingDataIO crawlingDataIO = new CrawlingDataIO(this);
    			crawlingDataIO.SaveLinksBuffer(LinksBuffer);
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Links buffer was regularly saved on HDD. There are {LinksBuffer.CrawledLinks.Count} crawled links and {LinksBuffer.LinksToCrawl.Count} link to crawl");
    			LinksBufferHDDSavingMutex.ReleaseMutex();
    		}
    	}

    	/// <summary>
    	/// Event that happens when one of children threads crawls some page.
    	/// </summary>
    	/// <param name="CrawledPageData">Crawled page data</param>
    	private void NewCrawlingThread_PageCrawledFromChildrenThread(PageCrawledCallback CrawledPageData)
    	{
    		ExecuteChildCrawledPage(CrawledPageData);
    	}

    	/// <summary>
    	/// Executes crawled page from one of the child thread
    	/// </summary>
    	/// <param name="CrawledPageData">Data of the crawled page</param>
    	private void ExecuteChildCrawledPage(PageCrawledCallback CrawledPageData)
    	{
    		DateTime now = DateTime.Now;
    		CrawledPageData.DownloadedPageUrl.LinkLastCrawlingDateAndTime = now;
    		PagesCrawlingTimesSeconds.Add(CrawledPageData.CrawledPageMeta.PageCrawlingTime.TotalSeconds);
    		if (CrawledPageData.CrawledPageMeta.CrawledPageThrownException == null && CrawledPageData.CrawledPageMeta.ResponseStatusCode == HttpStatusCode.OK)
    		{
    			CrawledPageData.DownloadedPageUrl.LinkLastCrawlingSuccessDateTime = now;
    		}
    		if ((from crawledLink in LinksBuffer.CrawledLinks.AsParallel().WithDegreeOfParallelism(CrawlerProperties.ConcurrentCollectionsParallelismQuantity)
    			where crawledLink.NormalizedOriginalLink == CrawledPageData.DownloadedPageUrl.NormalizedOriginalLink
    			select crawledLink).Count() == 0)
    		{
    			if (CrawledPageData.DownloadedPageUrl.DEBehaviorCycledIndexationProcess)
    			{
    				CrawledPageData.DownloadedPageUrl.PageAtRecrawlingProcess = true;
    			}
    			LinksBuffer.CrawledLinks.Add(CrawledPageData.DownloadedPageUrl);
    		}
    		else
    		{
    			PageLink pageLink = LinksBuffer.CrawledLinks.AsParallel().WithDegreeOfParallelism(CrawlerProperties.ConcurrentCollectionsParallelismQuantity).First();
    			if (!pageLink.DEBehaviorCycledIndexationProcess)
    			{
    				pageLink.PageAtRecrawlingProcess = false;
    			}
    			pageLink.LinkLastCrawlingDateAndTime = now;
    			if (CrawledPageData.CrawledPageMeta.CrawledPageThrownException == null && CrawledPageData.CrawledPageMeta.ResponseStatusCode == HttpStatusCode.OK)
    			{
    				pageLink.LinkLastCrawlingSuccessDateTime = now;
    			}
    		}
    		if (CrawledPageData.CrawledPageMeta.ResponseStatusCode != HttpStatusCode.OK || CrawledPageData.CrawledPageMeta.CrawledPageThrownException != null)
    		{
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"The page with the url = {CrawledPageData.DownloadedPageUrl.GetNormalizedOriginalLinkWithBehaviorPostfix()} doesn't crawled", CrawledPageData.CrawledPageMeta.CrawledPageThrownException);
    			Interlocked.Increment(ref MetricsSessionTotalCrawlingErrorsCount);
    		}
    		else
    		{
    			bool flag = true;
    			_ = CrawlerProperties.PagesToCrawlLimit;
    			if (CrawlerProperties.PagesToCrawlLimit > 0 && LinksBuffer.CrawledLinks.Count > CrawlerProperties.PagesToCrawlLimit)
    			{
    				flag = false;
    			}
    			if (!flag)
    			{
    				TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "The limit of pages to crawl was exceeded. See CrawlingServerProperties.PagesToCrawlLimit");
    			}
    			if (CrawlerProperties.CrawlWebsiteLinks && flag)
    			{
    				WebsiteInnerLinksAnalyser websiteInnerLinksAnalyser = new WebsiteInnerLinksAnalyser();
    				websiteInnerLinksAnalyser.AnalyseWebsitePageLinks(WebsiteBaseUrl, CrawledPageData.DownloadedPageUrl, CrawledPageData.PageLinks);
    				string oldValue = $"{WebsiteBaseUrl.Scheme}://{WebsiteBaseUrl.Authority}";
    				List<PageLink> LinksToCrawl = CrawledPageData.PageLinks;
    				LinksToCrawl = (from NextPageLink in LinksToCrawl.AsParallel().WithDegreeOfParallelism(CrawlerProperties.ConcurrentCollectionsParallelismQuantity)
    					where NextPageLink.LinkOwnerType == PageLinkOwnerType.InnerLink && NextPageLink.LinkResourceType == PageLinkResourceType.ContentPageLink && NextPageLink.IsLinkResolvedAndWellFormattedToAbsolutePathUrl
    					select NextPageLink).ToList();
    				if (CrawlerProperties.UrlSubstringsRestrictions != null && CrawlerProperties.UrlSubstringsRestrictions.Length != 0)
    				{
    					List<PageLink> list = new List<PageLink>(LinksToCrawl.Count);
    					foreach (PageLink item in LinksToCrawl)
    					{
    						bool flag2 = false;
    						string[] urlSubstringsRestrictions = CrawlerProperties.UrlSubstringsRestrictions;
    						foreach (string text in urlSubstringsRestrictions)
    						{
    							if (item.OriginalLinkLowercase.Contains(text) || text == "*")
    							{
    								flag2 = true;
    								break;
    							}
    						}
    						if (!flag2)
    						{
    							list.Add(item);
    						}
    					}
    					LinksToCrawl = list;
    				}
    				if (CrawlerProperties.RespectOnlySpecifiedUrls != null && CrawlerProperties.RespectOnlySpecifiedUrls.Length != 0)
    				{
    					List<PageLink> list2 = new List<PageLink>(LinksToCrawl.Count);
    					foreach (PageLink item2 in LinksToCrawl)
    					{
    						bool flag3 = false;
    						string[] respectOnlySpecifiedUrls = CrawlerProperties.RespectOnlySpecifiedUrls;
    						foreach (string text2 in respectOnlySpecifiedUrls)
    						{
    							if (item2.OriginalLinkLowercase.Contains(text2) || text2 == "*")
    							{
    								flag3 = true;
    								break;
    							}
    						}
    						if (flag3)
    						{
    							list2.Add(item2);
    						}
    					}
    					LinksToCrawl = list2;
    				}
    				if (CrawlerProperties.RespectRobotsTxtFile && WebsiteRobotsTxtData != null)
    				{
    					List<PageLink> list3 = new List<PageLink>(LinksToCrawl.Count);
    					foreach (PageLink item3 in LinksToCrawl)
    					{
    						string pageNormalizedURL = item3.NormalizedOriginalLink.Replace(oldValue, string.Empty);
    						if (WebsiteRobotsTxtData.CanCrawlLink(pageNormalizedURL))
    						{
    							list3.Add(item3);
    						}
    					}
    					if (WebsiteRobotsTxtData.HasCleanParamDirectives)
    					{
    						for (int l = 0; l < list3.Count; l++)
    						{
    							list3[l].NormalizedOriginalLink = WebsiteRobotsTxtData.TryToCleanUrlParams(list3[l].NormalizedOriginalLink);
    						}
    					}
    					LinksToCrawl = list3;
    				}
    				int i;
    				for (i = 0; i < LinksToCrawl.Count; i++)
    				{
    					if ((from item in LinksBuffer.LinksToCrawl.AsParallel().WithDegreeOfParallelism(CrawlerProperties.ConcurrentCollectionsParallelismQuantity)
    						where item.NormalizedOriginalLink == LinksToCrawl[i].NormalizedOriginalLink
    						select item).Count() == 0 && (from item in LinksBuffer.CrawledLinks.AsParallel().WithDegreeOfParallelism(CrawlerProperties.ConcurrentCollectionsParallelismQuantity)
    						where item.NormalizedOriginalLink == LinksToCrawl[i].NormalizedOriginalLink
    						select item).Count() == 0)
    					{
    						LinksBuffer.LinksToCrawl.Enqueue(LinksToCrawl[i]);
    					}
    				}
    			}
    			if (CrawlerProperties.StoreCrawledData)
    			{
    				CrawlingDataIO crawlingDataIO = new CrawlingDataIO(this);
    				crawlingDataIO.SaveCrawledDataInOriginalFormat(CrawledPageData);
    			}
    		}
    		this.PageCrawled?.Invoke(CrawledPageData);
    	}

    	/// <summary>
    	/// Marks specified link as outdated and forces it's recrawling
    	/// </summary>
    	/// <param name="PageLink"></param>
    	internal void ForceLinkRecrawling(string PageLink)
    	{
    		PageLink pageLink = LinksBuffer.CrawledLinks.Where((PageLink link) => link.NormalizedOriginalLink == PageLink).FirstOrDefault();
    		if (pageLink == null)
    		{
    			pageLink = LinksBuffer.LinksToCrawl.Where((PageLink link) => link.NormalizedOriginalLink == PageLink).FirstOrDefault();
    		}
    		if (pageLink != null && !pageLink.PageAtRecrawlingProcess)
    		{
    			pageLink.LinkLastCrawlingDateAndTime = new DateTime(2000, 1, 1);
    		}
    		if (!LinksBuffer.LinksToCrawl.Contains(pageLink))
    		{
    			LinksBuffer.LinksToCrawl.Enqueue(pageLink);
    		}
    	}

    	/// <summary>
    	/// Removes list of links from links buffer
    	/// </summary>
    	/// <param name="LinksToRemove">Links to remove</param>
    	internal void RemoveLinksFromLinksBuffer(List<string> LinksToRemove)
    	{
    		List<PageLink> list = LinksBuffer.CrawledLinks.ToList();
    		List<PageLink> list2 = LinksBuffer.LinksToCrawl.ToList();
    		int count = list.Count;
    		int count2 = list2.Count;
    		foreach (string NextLinkToRemove in LinksToRemove)
    		{
    			PageLink pageLink = (from item in list.AsParallel().WithDegreeOfParallelism(CrawlerProperties.ConcurrentCollectionsParallelismQuantity)
    				where item.NormalizedOriginalLink == NextLinkToRemove
    				select item).FirstOrDefault();
    			if (pageLink != null)
    			{
    				list.Remove(pageLink);
    			}
    			PageLink pageLink2 = (from item in list2.AsParallel().WithDegreeOfParallelism(CrawlerProperties.ConcurrentCollectionsParallelismQuantity)
    				where item.NormalizedOriginalLink == NextLinkToRemove
    				select item).FirstOrDefault();
    			if (pageLink2 != null)
    			{
    				list2.Remove(pageLink2);
    			}
    		}
    		if (list.Count != count || list2.Count != count2)
    		{
    			OverloadLinksBuffer(new CrawlingServerLinksBuffer(new ConcurrentQueue<PageLink>(list2), new ConcurrentBag<PageLink>(list)));
    			SaveLinksBufferOnHDD();
    		}
    	}

    	/// <summary>
    	/// Manually adds a set of links for crawling.
    	/// </summary>
    	/// <param name="PagesToCrawlNormalizedLinks">A list of links to crawl</param>
    	/// <param name="ForceBufferSavingOnHDD">Force links buffer saving on HDD</param>
    	internal CrawlingServerAddLinkToCrawlResults AddLinksToCrawling(List<string> PagesToCrawlNormalizedLinks, bool ForceBufferSavingOnHDD = false)
    	{
    		if (WebsiteBaseUrl == null)
    		{
    			throw new NullReferenceException("The website base URL is not defined. Please, see CrawlingServer.WebsiteBaseUrl property.");
    		}
    		string text = $"{WebsiteBaseUrl.Scheme.ToLower()}://{WebsiteBaseUrl.Authority.ToLower()}";
    		CrawlingServerAddLinkToCrawlResults crawlingServerAddLinkToCrawlResults = new CrawlingServerAddLinkToCrawlResults();
    		if (LinksBuffer.CrawledLinks == null)
    		{
    			LinksBuffer.CrawledLinks = new ConcurrentBag<PageLink>();
    		}
    		if (LinksBuffer.LinksToCrawl == null)
    		{
    			LinksBuffer.LinksToCrawl = new ConcurrentQueue<PageLink>();
    		}
    		WebsiteInnerLinksAnalyser websiteInnerLinksAnalyser = new WebsiteInnerLinksAnalyser();
    		foreach (string PagesToCrawlNormalizedLink in PagesToCrawlNormalizedLinks)
    		{
    			try
    			{
    				string text2 = PagesToCrawlNormalizedLink.ToLower();
    				if (text2.IndexOf(text) != 0)
    				{
    					string message = $"It is impossible to add the link {text2}, because it does not include the domain of the site.";
    					TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, message);
    					crawlingServerAddLinkToCrawlResults.LinksAddingLogs.Add(new CrawlingServerAddLinkToCrawlingResult(text2, IsLinkAddedToCrawling: false, "It is impossible to add this link, because it does not include the domain of the site."));
    					crawlingServerAddLinkToCrawlResults.SkippedLinksCount++;
    					continue;
    				}
    				if (CrawlerProperties.UrlSubstringsRestrictions == null || CrawlerProperties.UrlSubstringsRestrictions.Length == 0)
    				{
    					goto IL_0221;
    				}
    				bool flag = false;
    				string[] urlSubstringsRestrictions = CrawlerProperties.UrlSubstringsRestrictions;
    				foreach (string text3 in urlSubstringsRestrictions)
    				{
    					if (text3 != null && text3 == "*")
    					{
    						flag = true;
    						break;
    					}
    					if (text3 != null && text3.Length > 0 && text2.IndexOf(text3) != -1)
    					{
    						flag = true;
    						break;
    					}
    				}
    				if (!flag)
    				{
    					goto IL_0221;
    				}
    				string message2 = $"It is impossible to add the link {text2}; Link restricted in the properties of the CrawlingServer";
    				TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, message2);
    				crawlingServerAddLinkToCrawlResults.LinksAddingLogs.Add(new CrawlingServerAddLinkToCrawlingResult(text2, IsLinkAddedToCrawling: false, "It is impossible to add this link; Link restricted in the properties of the CrawlingServer"));
    				crawlingServerAddLinkToCrawlResults.SkippedLinksCount++;
    				goto end_IL_00b5;
    				IL_0221:
    				if (CrawlerProperties.RespectOnlySpecifiedUrls == null || CrawlerProperties.RespectOnlySpecifiedUrls.Length == 0)
    				{
    					goto IL_031e;
    				}
    				bool flag2 = false;
    				string[] respectOnlySpecifiedUrls = CrawlerProperties.RespectOnlySpecifiedUrls;
    				foreach (string text4 in respectOnlySpecifiedUrls)
    				{
    					if (text4 != null && text4 == "*")
    					{
    						flag2 = true;
    						break;
    					}
    					if (text4 != null && text4.Length > 0 && text2.IndexOf(text4) != -1)
    					{
    						flag2 = true;
    						break;
    					}
    				}
    				if (flag2)
    				{
    					goto IL_031e;
    				}
    				string message3 = $"It is impossible to add the link {text2}; Link unrespected in the properties of the CrawlingServer.";
    				TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, message3);
    				crawlingServerAddLinkToCrawlResults.LinksAddingLogs.Add(new CrawlingServerAddLinkToCrawlingResult(text2, IsLinkAddedToCrawling: false, "It is impossible to add this link; Link unrespected in the properties of the CrawlingServer."));
    				crawlingServerAddLinkToCrawlResults.SkippedLinksCount++;
    				goto end_IL_00b5;
    				IL_03cc:
    				PageLink nextLinkToCrawl = new PageLink(PagesToCrawlNormalizedLink);
    				websiteInnerLinksAnalyser.AnalyseWebsitePageLink(WebsiteBaseUrl, null, nextLinkToCrawl);
    				if (LinksBuffer.LinksToCrawl.Where((PageLink item) => item.NormalizedOriginalLink == nextLinkToCrawl.NormalizedOriginalLink).Count() == 0 && LinksBuffer.CrawledLinks.Where((PageLink item) => item.NormalizedOriginalLink == nextLinkToCrawl.NormalizedOriginalLink).Count() == 0)
    				{
    					LinksBuffer.LinksToCrawl.Enqueue(nextLinkToCrawl);
    					string message4 = $"Link {text2} added to the page crawl queue";
    					TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, message4);
    					crawlingServerAddLinkToCrawlResults.LinksAddingLogs.Add(new CrawlingServerAddLinkToCrawlingResult(text2, IsLinkAddedToCrawling: true, "Link added to the page crawl queue"));
    					crawlingServerAddLinkToCrawlResults.AddedLinksCount++;
    				}
    				else
    				{
    					string message5 = $"Link {text2} is NOT added to the page crawl queue, because it is already presented in the set of links";
    					TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, message5);
    					crawlingServerAddLinkToCrawlResults.LinksAddingLogs.Add(new CrawlingServerAddLinkToCrawlingResult(text2, IsLinkAddedToCrawling: false, "Link {0} is NOT added to the page crawl queue, because it is already presented in the set of links"));
    					crawlingServerAddLinkToCrawlResults.SkippedLinksCount++;
    				}
    				goto end_IL_00b5;
    				IL_031e:
    				bool flag3 = true;
    				if (!CrawlerProperties.RespectRobotsTxtFile || WebsiteRobotsTxtData == null)
    				{
    					goto IL_03cc;
    				}
    				string pageNormalizedURL = text2.Replace(text, string.Empty);
    				if (WebsiteRobotsTxtData.CanCrawlLink(pageNormalizedURL))
    				{
    					text2 = WebsiteRobotsTxtData.TryToCleanUrlParams(text2);
    					goto IL_03cc;
    				}
    				string message6 = $"It is impossible to add the link {text2}; Link disallowed in the robots.txt";
    				TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, message6);
    				crawlingServerAddLinkToCrawlResults.LinksAddingLogs.Add(new CrawlingServerAddLinkToCrawlingResult(text2, IsLinkAddedToCrawling: false, "It is impossible to add this link; Link disallowed in the robots.txt"));
    				crawlingServerAddLinkToCrawlResults.SkippedLinksCount++;
    				end_IL_00b5:;
    			}
    			catch (Exception occuredException)
    			{
    				TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Link is NOT added to the page crawl queue, because the exception has thrown. URL = {PagesToCrawlNormalizedLink}", occuredException);
    				crawlingServerAddLinkToCrawlResults.LinksAddingLogs.Add(new CrawlingServerAddLinkToCrawlingResult(PagesToCrawlNormalizedLink, IsLinkAddedToCrawling: false, "Link is NOT added to the page crawl queue, because the exception has thrown. See log details."));
    				crawlingServerAddLinkToCrawlResults.SkippedLinksCount++;
    			}
    		}
    		if (ForceBufferSavingOnHDD)
    		{
    			SaveLinksBufferOnHDD();
    		}
    		return crawlingServerAddLinkToCrawlResults;
    	}

    	/// <summary>
    	/// Tryes to load robots.txt file from HDD or from website, if HDD version was outdated.
    	/// </summary>
    	private void TryToLoadRobotsTxtFile()
    	{
    		if (CrawlerProperties.RespectRobotsTxtFile)
    		{
    			CrawlingDataIO crawlingDataIO = new CrawlingDataIO(this);
    			DERobotsTxtFile dERobotsTxtFile = null;
    			dERobotsTxtFile = ((WebsiteRobotsTxtData == null) ? crawlingDataIO.ReadRobotsTxtFile() : WebsiteRobotsTxtData);
    			bool flag = false;
    			if (dERobotsTxtFile != null)
    			{
    				DateTime now = DateTime.Now;
    				TimeSpan timeSpan = now - dERobotsTxtFile.LastCrawlingDateTime;
    				if (timeSpan.TotalDays > (double)CrawlerProperties.RobotsTxtReindexTimeDays && CrawlerProperties.RobotsTxtReindexTimeDays > 0)
    				{
    					TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"The robots.txt file marked as outdated. Days left from the last robots.txt analysing: {timeSpan.TotalDays}");
    					flag = true;
    				}
    			}
    			else
    			{
    				flag = true;
    			}
    			if (!flag)
    			{
    				WebsiteRobotsTxtData = dERobotsTxtFile;
    				return;
    			}
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Trying to refresh the robots.txt file");
    			DERobotsFileAnalyser dERobotsFileAnalyser = new DERobotsFileAnalyser(this);
    			DERobotsTxtParsedCallback dERobotsTxtParsedCallback = dERobotsFileAnalyser.DownloadAndAnalyseRobotsTxtFile(WebsiteBaseUrl.AbsoluteUri);
    			WebsiteRobotsTxtData = dERobotsTxtParsedCallback.RobotsTxtFileLink;
    			if (CrawlerProperties.StoreCrawledData && dERobotsTxtParsedCallback.IsParsedSuccessfully)
    			{
    				crawlingDataIO.SaveRobotsTxtFile(dERobotsTxtParsedCallback.RobotsTxtFileLink);
    			}
    			if (!dERobotsTxtParsedCallback.IsParsedSuccessfully)
    			{
    				TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"It is impossible to load the robots.txt file; The reason = {dERobotsTxtParsedCallback.ParsingResultsInformation}");
    			}
    			else
    			{
    				TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Robots.txt parsed");
    				int count = dERobotsTxtParsedCallback.RobotsTxtFileLink.ParsedOriginalContent.Count;
    				int num = dERobotsTxtParsedCallback.RobotsTxtFileLink.ParsedOriginalContent.Sum((KeyValuePair<string, List<DERobotsTXTInnerRow>> item) => item.Value.Count);
    				TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"The robots.txt contains {num} directives for {count} user-agents");
    				if (!dERobotsTxtParsedCallback.RobotsTxtFileLink.CrawlDelay.HasValue)
    				{
    					TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"The robots.txt doesn't reqire the crawl-delay parameter");
    				}
    				else
    				{
    					TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"The robots.txt requires the crawl-delay parameter with the value of {dERobotsTxtParsedCallback.RobotsTxtFileLink.CrawlDelay.Value} seconds");
    				}
    			}
    			this.RobotsTxtParsed?.Invoke(dERobotsTxtParsedCallback);
    		}
    		else
    		{
    			WebsiteRobotsTxtData = null;
    		}
    	}

    	/// <summary>
    	/// Tryes to reload sitemap file
    	/// </summary>
    	private void TryToReloadSitemapFile()
    	{
    		string text = string.Empty;
    		if (CrawlerProperties.SitemapUrl != string.Empty)
    		{
    			text = CrawlerProperties.SitemapUrl;
    			if (text != string.Empty)
    			{
    				TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "The URL of the sitemap is specified in the properties of the CrawlingServer");
    			}
    		}
    		else if (WebsiteRobotsTxtData != null)
    		{
    			text = WebsiteRobotsTxtData.GetSitemapUrl(this);
    			if (text != string.Empty)
    			{
    				TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "The robots.txt is respected. The URL of the sitemap is specified in the robots.txt file");
    			}
    		}
    		if (text != string.Empty && !DEExtensions.CheckValidUrl(text))
    		{
    			WebsiteSitemapData = null;
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "The URL to the sitemap doesn't valid. Downloading of the sitemap skipped.");
    			return;
    		}
    		bool flag = false;
    		CrawlingDataIO crawlingDataIO = new CrawlingDataIO(this);
    		DESitemapFile dESitemapFile = null;
    		dESitemapFile = ((WebsiteSitemapData != null) ? WebsiteSitemapData : crawlingDataIO.ReadSitemapFile());
    		if (dESitemapFile != null)
    		{
    			DateTime now = DateTime.Now;
    			TimeSpan timeSpan = now - dESitemapFile.LastReindexDateTime;
    			if (timeSpan.TotalDays < (double)CrawlerProperties.SitemapReindexTimeDays)
    			{
    				if (dESitemapFile.SitemapLocation == text)
    				{
    					WebsiteSitemapData = dESitemapFile;
    					return;
    				}
    				flag = true;
    				TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"The link to the sitemap changed. Old link = {dESitemapFile.SitemapLocation}, New link = {text}.");
    				if (crawlingDataIO.DeleteSitemapFile())
    				{
    					TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "The old sitemap file deleted");
    				}
    			}
    			else if (CrawlerProperties.SitemapReindexTimeDays > 0)
    			{
    				flag = true;
    				TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"The sitemap file marked as outdated. Days since the last analysis: {timeSpan.TotalDays}");
    			}
    		}
    		else
    		{
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Data Excavator does not know about old downloaded and analyzed sitemap files");
    			flag = true;
    		}
    		text = text.Trim();
    		if (text.Length == 0 && flag)
    		{
    			WebsiteSitemapData = null;
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "There are no new sitemap files specified.");
    			if (crawlingDataIO.DeleteSitemapFile())
    			{
    				TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Old sitemap file deleted");
    			}
    			return;
    		}
    		TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Application starts downloading and analyzing the site map file. This may take some time.");
    		DESitemapAnalyser dESitemapAnalyser = new DESitemapAnalyser(this, TaskLoggerLink);
    		DESitemapParsedCallback dESitemapParsedCallback = dESitemapAnalyser.DownloadAndAnalyseSitemapFile(text);
    		if (dESitemapParsedCallback.IsParsedSuccessfully && dESitemapParsedCallback.SitemapData != null)
    		{
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "The sitemap file downloaded and analysed.");
    			if (CrawlerProperties.StoreCrawledData)
    			{
    				TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Saving sitemap to the HDD...");
    				CrawlingDataIO crawlingDataIO2 = new CrawlingDataIO(this);
    				crawlingDataIO2.SaveSitemapFile(dESitemapParsedCallback.SitemapData);
    			}
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Fetching links from the sitemap...");
    			WebsiteSitemapData = dESitemapParsedCallback.SitemapData;
    			List<string> pagesLinks = WebsiteSitemapData.GetPagesLinks(CrawlerProperties.RespectOnlySpecifiedUrls, CrawlerProperties.UrlSubstringsRestrictions, CrawlerProperties.ConcurrentCollectionsParallelismQuantity);
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"{pagesLinks.Count} related links were found inside the sitemap file.");
    			AddLinksToCrawling(pagesLinks);
    		}
    		SitemapParsedHandler sitemapParsed = this.SitemapParsed;
    		if (this.SitemapParsed != null)
    		{
    			this.SitemapParsed(dESitemapParsedCallback);
    		}
    	}

    	/// <summary>
    	/// Makes proxy servers testing, if set of proxy servers used
    	/// </summary>
    	private void TestProxyServers()
    	{
    		if (CrawlerProperties.HTTPWebRequestProxiesList == null || CrawlerProperties.HTTPWebRequestProxiesList.Count <= 0 || DEConfig.ProxyServerTestingResourceLink == null || DEConfig.ProxyServerTestingResourceLink.Length <= 0 || DEConfig.ProxyServerTestingExpectedSubstringInSourceCode == null || DEConfig.ProxyServerTestingExpectedSubstringInSourceCode.Length <= 0)
    		{
    			return;
    		}
    		TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"There are {CrawlerProperties.HTTPWebRequestProxiesList.Count} proxy servers used. Checking for the proxies reachability on the website {DEConfig.ProxyServerTestingResourceLink}...");
    		List<DataCrawlingWebProxy> list = new List<DataCrawlingWebProxy>();
    		PageLink pageLink = new PageLink(DEConfig.ProxyServerTestingResourceLink);
    		pageLink.NormalizedOriginalLink = DEConfig.ProxyServerTestingResourceLink;
    		for (int i = 0; i < CrawlerProperties.HTTPWebRequestProxiesList.Count; i++)
    		{
    			DataCrawlingWebProxy dataCrawlingWebProxy = CrawlerProperties.HTTPWebRequestProxiesList[i];
    			if (dataCrawlingWebProxy.ProxyServerLink == null)
    			{
    				dataCrawlingWebProxy.InitializeProxy();
    			}
    			BinaryResourceDownloader binaryResourceDownloader = new BinaryResourceDownloader(this, dataCrawlingWebProxy.ProxyServerLink);
    			BinaryResourceDownloadingResult binaryResourceDownloadingResult = binaryResourceDownloader.DownloadResource(pageLink);
    			if (binaryResourceDownloadingResult.ResourceData != null && binaryResourceDownloadingResult.ResourceData.Length != 0)
    			{
    				string text = binaryResourceDownloadingResult.ConvertResourceToString();
    				if (text.IndexOf(DEConfig.ProxyServerTestingExpectedSubstringInSourceCode) == -1)
    				{
    					string message = $"Access to the proxy {CrawlerProperties.HTTPWebRequestProxiesList[i].ProxyAddress} failed. The proxy will removed from the proxies list of the CrawlingServer. Server response code = {binaryResourceDownloadingResult.ResourceHttpStatusCode.ToString()}, data length = {text.Length} characters, control string index = -1";
    					TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, message);
    					list.Add(CrawlerProperties.HTTPWebRequestProxiesList[i]);
    				}
    				else
    				{
    					TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Proxy {CrawlerProperties.HTTPWebRequestProxiesList[i].ProxyAddress} tested and seems its working.");
    				}
    				continue;
    			}
    			int num = 0;
    			if (binaryResourceDownloadingResult.ResourceData != null)
    			{
    				num = binaryResourceDownloadingResult.ResourceData.Length;
    			}
    			string text2 = $"Access to the proxy {CrawlerProperties.HTTPWebRequestProxiesList[i].ProxyAddress} failed. The proxy will removed from the proxies list of the CrawlingServer. Server response code = {binaryResourceDownloadingResult.ResourceHttpStatusCode.ToString()}, data size (bytes) = {num}";
    			if (binaryResourceDownloadingResult.ResourceDownloadException != null)
    			{
    				text2 += $", Exception = {binaryResourceDownloadingResult.ResourceDownloadException.ToString()}";
    			}
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, text2);
    			list.Add(CrawlerProperties.HTTPWebRequestProxiesList[i]);
    		}
    		if (list.Count > 0)
    		{
    			for (int j = 0; j < list.Count; j++)
    			{
    				CrawlerProperties.HTTPWebRequestProxiesList.Remove(list[j]);
    			}
    		}
    		if (CrawlerProperties.HTTPWebRequestProxiesList.Count == 0)
    		{
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Warning! There are NO proxies used for the data crawling. All the setted proxies are not available.");
    		}
    	}

    	/// <summary>
    	/// Starts server crawling
    	/// </summary>
    	internal void StartCrawling()
    	{
    		if (!LicenseServer.CheckLicenseKeyValid())
    		{
    			throw LicenseValidationException.FromLicenseValidationResponse();
    		}
    		if (LicenseServer.ActualKey.KeyTotalThreadsLimitPerProject != -1 && CrawlerProperties.CrawlingThreadsCount > LicenseServer.ActualKey.KeyTotalThreadsLimitPerProject)
    		{
    			throw LicenseValidationException.FromCrawlingServerIfWrongThreadsCountRequested();
    		}
    		if (ActualServerStatus == CrawlingServerStatus.Crawling || ActualServerStatus == CrawlingServerStatus.Stopping)
    		{
    			return;
    		}
    		ActualServerStatus = CrawlingServerStatus.Crawling;
    		TestProxyServers();
    		TryToLoadRobotsTxtFile();
    		TryToReloadSitemapFile();
    		if (CrawlerProperties.StoreCrawledData)
    		{
    			if (LinksBufferHddSavingThread == null || LinksBufferHddSavingThread.IsThreadMustBeReloadedBeforeStart())
    			{
    				LinksBufferHddSavingThread = new Thread(AutoSaveLinksBufferOnHDDThreadBody);
    			}
    			LinksBufferHddSavingThread.Start();
    		}
    		if (CrawlerProperties.ReindexCrawledPages)
    		{
    			if (LinksReindexationAnalysingThread == null || LinksReindexationAnalysingThread.IsThreadMustBeReloadedBeforeStart())
    			{
    				LinksReindexationAnalysingThread = new Thread(LinksReindexationAnalysingThreadBody);
    			}
    			LinksReindexationAnalysingThread.Start();
    		}
    		if (LinksBuffer.LinksToCrawl.Count == 0 && LinksBuffer.CrawledLinks.Count == 0)
    		{
    			string item = $"{WebsiteBaseUrl.Scheme.ToLower()}://{WebsiteBaseUrl.Authority.ToLower()}";
    			AddLinksToCrawling(new List<string> { item });
    		}
    		if (CrawlerProperties.RespectRobotsTxtFile || CrawlerProperties.SitemapUrl != string.Empty)
    		{
    			if (RoutineTasksRunningThread == null || RoutineTasksRunningThread.IsThreadMustBeReloadedBeforeStart())
    			{
    				RoutineTasksRunningThread = new Thread(RoutineTasksRunningThreadBody);
    			}
    			RoutineTasksRunningThread.Start();
    		}
    		for (int i = 0; i < PageCrawlingThreads.Count; i++)
    		{
    			PageCrawlingThreads[i].StartCrawling();
    		}
    	}

    	/// <summary>
    	/// Stops server crawling
    	/// </summary>
    	internal void StopCrawling()
    	{
    		if (ActualServerStatus != 0 && ActualServerStatus != CrawlingServerStatus.Stopping)
    		{
    			ActualServerStatus = CrawlingServerStatus.Stopping;
    			for (int i = 0; i < PageCrawlingThreads.Count; i++)
    			{
    				PageCrawlingThreads[i].StopCrawling();
    			}
    			if (LinksBufferHddSavingThread != null)
    			{
    				LinksBufferHddSavingThread.Abort();
    			}
    			if (LinksReindexationAnalysingThread != null)
    			{
    				LinksReindexationAnalysingThread.Abort();
    			}
    			if (RoutineTasksRunningThread != null)
    			{
    				RoutineTasksRunningThread.Abort();
    			}
    			if (CrawlerProperties.StoreCrawledData)
    			{
    				SaveLinksBufferOnHDD();
    			}
    			ActualServerStatus = CrawlingServerStatus.Waiting;
    		}
    	}

    	/// <summary>
    	/// Gets lock for keep Crawl-Delay directive
    	/// </summary>
    	internal void CrawlDelayWait()
    	{
    		if (!CrawlerProperties.RespectRobotsTxtFile || WebsiteRobotsTxtData == null || !WebsiteRobotsTxtData.CrawlDelay.HasValue || WebsiteRobotsTxtData.CrawlDelay.Value <= 0.0)
    		{
    			return;
    		}
    		double num = WebsiteRobotsTxtData.CrawlDelay.Value;
    		if (num >= (double)DEConfig.MaximumCrawlDelayValueInSeconds)
    		{
    			num = DEConfig.MaximumCrawlDelayValueInSeconds;
    		}
    		bool flag = false;
    		try
    		{
    			flag = CrawlDelayDistributionMutex.WaitOne();
    			_ = LastCrawlingRequestDateTime;
    			if (false)
    			{
    				LastCrawlingRequestDateTime = DateTime.Now;
    			}
    			TimeSpan timeSpan = new TimeSpan(0, 0, 1);
    			do
    			{
    				DateTime now = DateTime.Now;
    				timeSpan = now - LastCrawlingRequestDateTime;
    				Thread.Sleep(10);
    			}
    			while (timeSpan.TotalSeconds >= num && !DEConfig.ShutDownProgram);
    		}
    		finally
    		{
    			if (flag)
    			{
    				CrawlDelayDistributionMutex.ReleaseMutex();
    			}
    		}
    	}
    }
}
