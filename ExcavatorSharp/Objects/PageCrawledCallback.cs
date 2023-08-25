// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.PageCrawledCallback
using System.Collections.Generic;
using System.Drawing;

namespace ExcavatorSharp.Objects
{
    /// <summary>
    /// Crawling thread callback
    /// </summary>
    public class PageCrawledCallback
    {
    	/// <summary>
    	/// The meta information about page crawling
    	/// </summary>
    	public PageCrawlingResultMetaInformation CrawledPageMeta { get; private set; }

    	/// <summary>
    	/// Downloaded page full url
    	/// </summary>
    	public PageLink DownloadedPageUrl { get; private set; }

    	/// <summary>
    	/// Downloaded page HTML content
    	/// </summary>
    	public string DownloadedPageHtml { get; private set; }

    	/// <summary>
    	/// Page downloaded IFrames
    	/// </summary>
    	public Dictionary<string, string> DownloadedPageFrames { get; private set; }

    	/// <summary>
    	/// All links, fetched from crawled page
    	/// </summary>
    	public List<PageLink> PageLinks { get; set; }

    	/// <summary>
    	/// Prevents page grabbing. By default, setted to false.
    	/// </summary>
    	public bool PreventPageGrabbing { get; set; }

    	/// <summary>
    	/// Page screenshot data
    	/// </summary>
    	public Bitmap PageScreenshot { get; set; }

    	public PageCrawledCallback(PageCrawlingResultMetaInformation CrawledPageMeta, PageLink DownloadedPageUrl, string DownloadedPageHtml, List<PageLink> PageLinks, Bitmap PageScreenshot, Dictionary<string, string> DownloadedPageFrames)
    	{
    		this.CrawledPageMeta = CrawledPageMeta;
    		this.DownloadedPageHtml = DownloadedPageHtml;
    		this.PageLinks = PageLinks;
    		this.DownloadedPageUrl = DownloadedPageUrl;
    		PreventPageGrabbing = false;
    		this.PageScreenshot = PageScreenshot;
    		this.DownloadedPageFrames = DownloadedPageFrames;
    	}

    	/// <summary>
    	/// Object finalizer
    	/// </summary>
    	~PageCrawledCallback()
    	{
    		if (PageScreenshot != null)
    		{
    			PageScreenshot.Dispose();
    			PageScreenshot = null;
    		}
    	}
    }
}
