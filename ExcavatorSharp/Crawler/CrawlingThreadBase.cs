// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Crawler.CrawlingThreadBase
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using ExcavatorSharp.ExtensionMethods;
using ExcavatorSharp.Objects;

namespace ExcavatorSharp.Crawler
{
    /// <summary>
    /// Base class for crawling thread
    /// </summary>
    internal abstract class CrawlingThreadBase
    {
    	/// <summary>
    	/// Delegate for PageCrawled event
    	/// </summary>
    	/// <param name="CrawledPageData">Crawled page data</param>
    	public delegate void PageCrawledHandler(PageCrawledCallback CrawledPageData);

    	/// <summary>
    	/// Crawling inner thread
    	/// </summary>
    	protected Thread ThreadObject { get; set; }

    	/// <summary>
    	/// Link to parent crawling environment
    	/// </summary>
    	internal CrawlingServer CrawlingServerEnvironmentLink { get; set; }

    	/// <summary>
    	/// Object for accessing crawling server proxies list
    	/// </summary>
    	protected ProxyAccessor ProxyServersAccessor { get; set; }

    	/// <summary>
    	/// Initial pseudo-random sleeping time for each thread. Used once only at StartCrawling
    	/// </summary>
    	protected int ThreadInitialSleepingTime { get; set; }

    	/// <summary>
    	/// Main event, that will happen after next page been crawled.
    	/// </summary>
    	public abstract event PageCrawledHandler PageCrawled;

    	/// <summary>
    	/// Creates a new instance of CrawlingThread
    	/// </summary>
    	/// <param name="CrawlingServerEnvironmentLink">Link to parent server</param>
    	public CrawlingThreadBase(CrawlingServer CrawlingServerEnvironmentLink, int ThreadInitialSleepingTime)
    	{
    		this.CrawlingServerEnvironmentLink = CrawlingServerEnvironmentLink;
    		this.ThreadInitialSleepingTime = ThreadInitialSleepingTime;
    		ProxyServersAccessor = new ProxyAccessor(CrawlingServerEnvironmentLink.CrawlerProperties, CrawlingServerEnvironmentLink.ProxySequenciveRotationMutex, ThreadInitialSleepingTime);
    	}

    	/// <summary>
    	/// Start crawling pages
    	/// </summary>
    	public virtual void StartCrawling()
    	{
    		if (ThreadObject == null || ThreadObject.IsThreadMustBeReloadedBeforeStart())
    		{
    			ThreadObject = new Thread(ThreadBody);
    		}
    		ThreadObject.Start();
    	}

    	/// <summary>
    	/// Stops crawling pages
    	/// </summary>
    	public virtual void StopCrawling()
    	{
    		ThreadObject.Abort();
    	}

    	/// <summary>
    	/// Main crawling thread body
    	/// </summary>
    	protected abstract void ThreadBody();

    	/// <summary>
    	/// Parses downloaded page links
    	/// </summary>
    	/// <param name="DownloadedHtmlPage">Page html content</param>
    	protected List<PageLink> ParsePageLinks(string DownloadedHtmlPage)
    	{
    		List<PageLink> list = new List<PageLink>();
    		try
    		{
    			Match match = Regex.Match(DownloadedHtmlPage, "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1.0));
    			while (match.Success)
    			{
    				string text = match.Groups[1].Value.Replace("&amp;", "&").Trim();
    				if (text.Length > 0)
    				{
    					list.Add(new PageLink(text));
    				}
    				match = match.NextMatch();
    			}
    		}
    		catch (RegexMatchTimeoutException)
    		{
    		}
    		return list;
    	}
    }
}
