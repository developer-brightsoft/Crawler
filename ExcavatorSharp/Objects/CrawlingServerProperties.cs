// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.CrawlingServerProperties
using System;
using System.Collections.Generic;
using ExcavatorSharp.Captcha;
using ExcavatorSharp.CEF;
using ExcavatorSharp.Interfaces;
using Newtonsoft.Json;

namespace ExcavatorSharp.Objects
{
    /// <summary>
    /// Container for crawler properties
    /// </summary>
    public class CrawlingServerProperties : IJSONConvertible<CrawlingServerProperties>, ICloneable
    {
    	/// <summary>
    	/// Default user-agent respectation chain for robots.txt execution
    	/// </summary>
    	public static readonly string[] DefaultRobotsTxtUserAgentRespectationChain = new string[4] { "Googlebot", "Yandex", "*", "YandexBot" };

    	/// <summary>
    	/// Download the IFrame content and position it as part of the page for further use.
    	/// </summary>
    	public bool ExpandPageFrames = true;

    	/// <summary>
    	/// Pointer for proxies list sequencive rotation. Only for using from ProxyAccessor.
    	/// </summary>
    	[JsonIgnore]
    	internal volatile int ProxySequenciveRotationPointer = 0;

    	/// <summary>
    	/// Primary technology to crawl data. Use NativeCrawling for downloading data with HttpWebRequest (.NET), or CEFCrawling for download data using Chromium Embedded Framework.
    	/// </summary>
    	public DataCrawlingType PrimaryDataCrawlingWay { get; set; }

    	/// <summary>
    	/// A set of the URLs or URLs substrings to be indexed. All other URLs will be ignored.
    	/// Be attentive - there is no regexp here, only indexOf will applied.
    	/// </summary>
    	public string[] RespectOnlySpecifiedUrls { get; set; }

    	/// <summary>
    	/// A set of the URL substrings, restricted for indexing.
    	/// For example, if you want to restrinct from indexing page with url that contains '/foo?a=b', just include ['/foo?a-b'] in array.
    	/// Be attentive - there is no regexp here, only indexOf will applied.
    	/// </summary> 
    	public string[] UrlSubstringsRestrictions { get; set; }

    	/// <summary>
    	/// Respects or unrespects robots.txt file and it's rules
    	/// </summary>
    	public bool RespectRobotsTxtFile { get; set; }

    	/// <summary>
    	/// Chain of respect for the directive User-agent. The first user-agent from this list, if it is mentioned in robots.txt, will be used to apply the corresponding rule block from robots.txt.
    	/// </summary>
    	public string[] RobotsTxtUserAgentRespectationChain { get; set; }

    	/// <summary>
    	/// Days to reindex robots.txt file if RespectRobotsTxtFile=true. If value is set to -1 we will not updage robots.txt automatically.
    	/// </summary>
    	public int RobotsTxtReindexTimeDays { get; set; }

    	/// <summary>
    	/// The number of crawling threads. Should be setted ONLY AT INITIALIZATION. No effect if changed from the initialized CrawlingServer object.
    	/// </summary>
    	public int CrawlingThreadsCount { get; set; }

    	/// <summary>
    	/// Delay of crawling thread, in milliseconds.
    	/// </summary>
    	public int CrawlingThreadDelayMilliseconds { get; set; }

    	/// <summary>
    	/// User agent that will be used for @header(UserAgent) before page downloaded
    	/// </summary>
    	public string CrawlerUserAgent { get; set; }

    	/// <summary>
    	/// Web request method. Allowed values - "GET", "POST"
    	/// </summary>
    	public string HttpWebRequestMethod { get; set; }

    	/// <summary>
    	/// Set of POST or GET variables, applied for each crawling request.
    	/// </summary>
    	public Dictionary<string, string> CrawlingRequestAdditionalParamsList { get; set; }

    	/// <summary>
    	/// Link to the sitemap file
    	/// </summary>
    	public string SitemapUrl { get; set; }

    	/// <summary>
    	/// Days between sitemap file refreshes. If value is set to -1 we will not updage sitemap automatically.
    	/// </summary>
    	public int SitemapReindexTimeDays { get; set; }

    	/// <summary>
    	/// Total limit of pages that will be crawled. -1 means that's no limit.
    	/// </summary>
    	public int PagesToCrawlLimit { get; set; }

    	/// <summary>
    	/// Maximum time in milliseconds while each thread will be tried to download some crawling website page
    	/// </summary>
    	public int PageDownloadTimeoutMilliseconds { get; set; }

    	/// <summary>
    	/// Number of repetitions of page download attempts. Min value = 1.
    	/// </summary>
    	public int PageDownloadAttempts { get; set; }

    	/// <summary>
    	/// Does the application need to collect links from downloadable pages and crawl them?
    	/// </summary>
    	public bool CrawlWebsiteLinks { get; set; }

    	/// <summary>
    	/// Is it necessary to store crawled data and links on HDD and uses it during analysing process. We strongly recommend 
    	/// to use this option with TRUE value, because it protects you from  infine looping on the some isolated website segment, protects you
    	/// from reindexing actually indexed pages after program reloaded and someother obvious things, that depends on big data analysing and storing.
    	/// </summary>
    	public bool StoreCrawledData { get; set; }

    	/// <summary>
    	/// Is it necessary to store pages html source data on HDD.
    	/// </summary>
    	public bool StoreCrawledPagesHTMLSource { get; set; }

    	/// <summary>
    	/// Is it necessary to reindex pages after some time, automatically
    	/// </summary>
    	public bool ReindexCrawledPages { get; set; }

    	/// <summary>
    	/// The time period in minutes, when indexed pages will be marked as pages to reindexing and will scanned again.
    	/// </summary>
    	public int ReindexCrawledPagesAfterSpecifiedMinutesInterval { get; set; }

    	/// <summary>
    	/// Web proxy, used for making HTTPWebRequest on parsing. By default, presented as Empty web proxy.
    	/// </summary>
    	public List<DataCrawlingWebProxy> HTTPWebRequestProxiesList { get; set; }

    	/// <summary>
    	/// Rotate HTTPWebRequestProxiesList after each web request. Each new request will use next proxy in list. Used only if Proxy serie specified.
    	/// </summary>
    	public ProxiesRotationType ProxiesRotation { get; set; }

    	/// <summary>
    	/// Time in milliseconds between swap actual links buffer to HDD
    	/// </summary>
    	public int LinksBufferHDDAutoSavingMilliseconds { get; set; }

    	/// <summary>
    	/// Parallelism limit to search data in linear collections. We use most of linq operations with AsParallel().WithDegreeOfParallelism(n) feature.
    	/// </summary>
    	public int ConcurrentCollectionsParallelismQuantity { get; set; }

    	/// <summary>
    	/// A set of CEF behaviors, if PrimaryDataCrawlingWay setted to CEFCrawling
    	/// </summary>
    	public List<CEFCrawlingBehavior> CEFCrawlingBehaviors { get; set; }

    	/// <summary>
    	/// Authentication data for logging into the site, which should be used to work with the site.
    	/// </summary>
    	public CEFWebsiteAuthBehavior CEFWebsiteAuthenticationBehavior { get; set; }

    	/// <summary>
    	/// Is it necessary to take screenshot after page loading (applied only for CEF crawling)
    	/// </summary>
    	public bool TakeScreenshotAfterPageLoaded { get; set; }

    	/// <summary>
    	/// CAPTCHA settings [Under development, not supported at the moment.]
    	/// </summary>
    	public CAPTCHASolverSettings CaptchaSettings { get; set; }

    	/// <summary>
    	/// Create a container for crawler properties
    	/// </summary>
    	/// <param name="PrimaryDataCrawlingWay">Primary technology to crawl data. Use NativeCrawling for downloading data with HttpWebRequest (.NET), or CEFCrawling for download data using Chromium Embedded Framework.</param>
    	/// <param name="RespectOnlySpecifiedUrls">A set of the URLs or URLs substrings to be indexed. All other URLs will be ignored. Be attentive - there is no regexp here, only indexOf will applied.</param>
    	/// <param name="UrlSubstringsRestrictions">A set of the URL substrings, restricted for indexing. Be attentive - there is no regexp here, only indexOf will applied.</param>
    	/// <param name="RespectRobotsTxtFile">Respect or unrespect robots.txt file</param>
    	/// <param name="PagesToCrawlLimit">Pages to crawl total limit</param>
    	/// <param name="RobotsTxtUserAgentRespectationChain">Chain of respect for the directive User-agent. The first user-agent from this list, if it is mentioned in robots.txt, will be used to apply the corresponding rule block from robots.txt.</param>
    	/// <param name="RobotsTxtReindexTimeDays">Reindex robots.txt file after N days. Used only of RespectRobotsTxtFile=true</param>
    	/// <param name="CrawlingThreadsCount">The number of crawling threads count</param>
    	/// <param name="CrawlingThreadDelayMilliseconds">Delay of crawling thread, in milliseconds.</param>
    	/// <param name="CrawlerUserAgent">User agent that be used for header(UserAgent) before page downloaded</param>
    	/// <param name="HttpWebRequestMethod">Http web request methos - allowed values are "GET" or "POST"</param>
    	/// <param name="PageDownloadTimeoutMilliseconds">Maximum time in milliseconds while each thread will be tried to download some crawling website page</param>
    	/// <param name="PageDownloadAttempts">Number of repetitions of page download attempts</param>
    	/// <param name="CrawlWebsiteLinks">Do we need to collect links from downloaded pages and crawl them?</param>
    	/// <param name="StoreCrawledData">Is it necessary to store crawled data and links on HDD and uses it during analysing process. We strongly recommend to use this option with TRUE value, because it protects you from  infine looping on the some isolated website segment, protects you from reindexing actually indexed pages after program reloaded and someother obvious things, that depends on big data analysing and storing.</param>
    	/// <param name="StoreCrawledPagesHTMLSource">Is it necessary to store pages html source data on HDD.</param>
    	/// <param name="ReindexCrawledPages">Is it necessary to reindex pages after some time, automatically</param>
    	/// <param name="ReindexCrawledPagesAfterSpecifiedMinutesInterval">The time period in minutes, when indexed pages will be marked as pages to reindexing and will scanned again.</param> 
    	/// <param name="LinksBufferHDDAutoSavingMilliseconds">Time in milliseconds between swap actual links buffer to HDD</param>
    	/// <param name="ConcurrentCollectionsParallelismQuantity">Parallelism limit to search data in linear collections. We use Parallel.ForEach and most of linq operations with AsParallel().WithDegreeOfParallelism(n) feature. Be careful - increasing this param can make very hight load on your CPU.</param>
    	/// <param name="HTTPWebRequestProxiesList">List of proxy-servers, used for crawling</param>
    	/// <param name="SitemapUrl">Link to sitemap file</param>
    	/// <param name="SitemapReindexTimeDays">Days between sitemap reindexation</param>
    	/// <param name="ProxiesRotation">Proxies rotation type if necessary</param>
    	/// <param name="CEFCrawlingBehaviors">A set of CEF behaviors, if PrimaryDataCrawlingWay setted to CEFCrawling</param>
    	/// <param name="CrawlingRequestAdditionalParamsList">Set of POST or GET variables, applied for each crawling request</param>
    	/// <param name="CEFWebsiteAuthenticationBehavior">Authentication behavior for website</param>
    	/// <param name="TakeScreenshotAfterPageLoaded">Is it necessary to take screenshot after page loading (applied only for CEF crawling)</param>
    	/// <param name="ExpandPageFrames">Download the IFrame content and position it as part of the page for further use (CEF only).</param>
    	/// <param name="CaptchaSettings">CAPTCHA's resolving settings</param>
    	public CrawlingServerProperties(DataCrawlingType PrimaryDataCrawlingWay = DataCrawlingType.CEFCrawling, string[] RespectOnlySpecifiedUrls = null, string[] UrlSubstringsRestrictions = null, int CrawlingThreadsCount = 2, int CrawlingThreadDelayMilliseconds = 5000, string CrawlerUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36", string HttpWebRequestMethod = "GET", Dictionary<string, string> CrawlingRequestAdditionalParamsList = null, int PageDownloadTimeoutMilliseconds = 120000, int PageDownloadAttempts = 3, bool CrawlWebsiteLinks = true, bool StoreCrawledData = true, bool StoreCrawledPagesHTMLSource = false, bool ReindexCrawledPages = false, int ReindexCrawledPagesAfterSpecifiedMinutesInterval = 10080, bool RespectRobotsTxtFile = false, string[] RobotsTxtUserAgentRespectationChain = null, int RobotsTxtReindexTimeDays = 3, string SitemapUrl = "", int SitemapReindexTimeDays = 3, int PagesToCrawlLimit = -1, List<DataCrawlingWebProxy> HTTPWebRequestProxiesList = null, ProxiesRotationType ProxiesRotation = ProxiesRotationType.NoRotation, int LinksBufferHDDAutoSavingMilliseconds = 60000, int ConcurrentCollectionsParallelismQuantity = 2, List<CEFCrawlingBehavior> CEFCrawlingBehaviors = null, CEFWebsiteAuthBehavior CEFWebsiteAuthenticationBehavior = null, bool TakeScreenshotAfterPageLoaded = false, bool ExpandPageFrames = false, CAPTCHASolverSettings CaptchaSettings = null)
    	{
    		this.PrimaryDataCrawlingWay = PrimaryDataCrawlingWay;
    		this.RespectOnlySpecifiedUrls = RespectOnlySpecifiedUrls;
    		this.UrlSubstringsRestrictions = UrlSubstringsRestrictions;
    		this.RespectRobotsTxtFile = RespectRobotsTxtFile;
    		this.RobotsTxtUserAgentRespectationChain = RobotsTxtUserAgentRespectationChain;
    		this.RobotsTxtReindexTimeDays = RobotsTxtReindexTimeDays;
    		this.SitemapUrl = SitemapUrl;
    		this.SitemapReindexTimeDays = SitemapReindexTimeDays;
    		this.CrawlingThreadsCount = CrawlingThreadsCount;
    		this.CrawlingThreadDelayMilliseconds = CrawlingThreadDelayMilliseconds;
    		this.CrawlerUserAgent = CrawlerUserAgent;
    		this.HttpWebRequestMethod = HttpWebRequestMethod;
    		this.CrawlingRequestAdditionalParamsList = CrawlingRequestAdditionalParamsList;
    		this.PagesToCrawlLimit = PagesToCrawlLimit;
    		this.PageDownloadTimeoutMilliseconds = PageDownloadTimeoutMilliseconds;
    		this.CrawlWebsiteLinks = CrawlWebsiteLinks;
    		this.PageDownloadAttempts = PageDownloadAttempts;
    		this.StoreCrawledData = StoreCrawledData;
    		this.StoreCrawledPagesHTMLSource = StoreCrawledPagesHTMLSource;
    		this.ReindexCrawledPages = ReindexCrawledPages;
    		this.ReindexCrawledPagesAfterSpecifiedMinutesInterval = ReindexCrawledPagesAfterSpecifiedMinutesInterval;
    		this.HTTPWebRequestProxiesList = HTTPWebRequestProxiesList;
    		this.ProxiesRotation = ProxiesRotation;
    		this.LinksBufferHDDAutoSavingMilliseconds = LinksBufferHDDAutoSavingMilliseconds;
    		this.ConcurrentCollectionsParallelismQuantity = ConcurrentCollectionsParallelismQuantity;
    		this.CEFCrawlingBehaviors = CEFCrawlingBehaviors;
    		this.CEFWebsiteAuthenticationBehavior = CEFWebsiteAuthenticationBehavior;
    		ProxySequenciveRotationPointer = 0;
    		this.TakeScreenshotAfterPageLoaded = TakeScreenshotAfterPageLoaded;
    		this.ExpandPageFrames = ExpandPageFrames;
    		if (this.RobotsTxtUserAgentRespectationChain == null || this.RobotsTxtUserAgentRespectationChain.Length == 0)
    		{
    			this.RobotsTxtUserAgentRespectationChain = DefaultRobotsTxtUserAgentRespectationChain;
    		}
    		if (this.UrlSubstringsRestrictions == null || this.UrlSubstringsRestrictions.Length == 0)
    		{
    			this.UrlSubstringsRestrictions = new string[32]
    			{
    				".css", ".js", "assets", ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".ico", ".svg",
    				".tiff", ".mp3", ".mp4", ".exe", ".msi", "blob:", ".xls", ".xlsx", ".doc", ".docx",
    				".odt", ".csv", ".txt", "login", "log-in", "logout", "log-out", "exit", "tray", "checkout",
    				"account", "reviews"
    			};
    		}
    	}

    	/// <summary>
    	/// Serializes data to JSON
    	/// </summary>
    	/// <returns>Serialized object</returns>
    	public string SerializeToJSON()
    	{
    		return JsonConvert.SerializeObject((object)this, (Formatting)1);
    	}

    	/// <summary>
    	/// Unserializes data from JSON
    	/// </summary>
    	/// <param name="JSONData">Input data</param>
    	/// <returns>New instance of CrawlingServerProperties</returns>
    	public CrawlingServerProperties UnserializeFromJSON(string JSONData)
    	{
    		return JsonConvert.DeserializeObject<CrawlingServerProperties>(JSONData);
    	}

    	/// <summary>
    	/// Clones server properties
    	/// </summary>
    	/// <returns>New instance of DataCrawlingServerProperties</returns>
    	public object Clone()
    	{
    		string jSONData = SerializeToJSON();
    		return UnserializeFromJSON(jSONData);
    	}
    }
}
