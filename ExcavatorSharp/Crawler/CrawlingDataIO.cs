// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Crawler.CrawlingDataIO
using System;
using System.IO;
using ExcavatorSharp.Objects;
using Newtonsoft.Json;

namespace ExcavatorSharp.Crawler
{
    /// <summary>
    /// IO operations for crawled data
    /// </summary>
    internal class CrawlingDataIO
    {
    	/// <summary>
    	/// Constant with name of folder contains grabbed data
    	/// </summary>
    	public const string CrawledDataRootFolderName = "crawled-data";

    	/// <summary>
    	/// Name of the folder with crawling html source data
    	/// </summary>
    	public const string CrawledDataHtmlStorageName = "html-storage";

    	/// <summary>
    	/// File name for storing page source HTML codes
    	/// </summary>
    	public const string CrawledDataHtmlPageSourceFileName = "page-source.html";

    	/// <summary>
    	/// File name for storing crawled page meta information
    	/// </summary>
    	public const string CrawledDataHtmlMetaInformationFileName = "page-meta.json";

    	/// <summary>
    	/// File name for storing crawled links and links to crawl. 
    	/// </summary>
    	public const string LinksBufferFileName = "links-buffer.json";

    	/// <summary>
    	/// Actual version of robots.txt file
    	/// </summary>
    	public const string RobotsTxtFileName = "robots-actual.json";

    	/// <summary>
    	/// Actual version of sitemap files set
    	/// </summary>
    	public const string SitemapActualFileName = "sitemap-actual.json";

    	/// <summary>
    	/// Parent grabbing server link
    	/// </summary>
    	private CrawlingServer CrawlerLink { get; set; }

    	/// <summary>
    	/// Creates new instance of grabbing data saver
    	/// </summary>
    	/// <param name="GrabberLink">Link to grabbing server</param>
    	public CrawlingDataIO(CrawlingServer CrawlerLink)
    	{
    		this.CrawlerLink = CrawlerLink;
    	}

    	/// <summary>
    	/// Saves crawled page data in original format
    	/// </summary>
    	/// <param name="CrawledPageData"></param>
    	public void SaveCrawledDataInOriginalFormat(PageCrawledCallback CrawledPageData)
    	{
    		string text = string.Format("{0}/{1}", CrawlerLink.ParentTaskOperatingDirectory, "crawled-data");
    		if (!Directory.Exists(text))
    		{
    			Directory.CreateDirectory(text);
    		}
    		string text2 = string.Format("{0}/{1}", text, "html-storage");
    		if (!Directory.Exists(text2))
    		{
    			Directory.CreateDirectory(text2);
    		}
    		string text3 = string.Format("{0}/{1}", text2, DateTime.Now.ToString("dd.MM.yyyy"));
    		if (!Directory.Exists(text3))
    		{
    			Directory.CreateDirectory(text3);
    		}
    		string text4 = $"{text3}/{Guid.NewGuid().ToString()}";
    		if (!Directory.Exists(text4))
    		{
    			Directory.CreateDirectory(text4);
    		}
    		string path = string.Format("{0}/{1}", text4, "page-source.html");
    		string path2 = string.Format("{0}/{1}", text4, "page-meta.json");
    		if (CrawlerLink.CrawlerProperties.StoreCrawledPagesHTMLSource)
    		{
    			File.WriteAllText(path, CrawledPageData.DownloadedPageHtml);
    		}
    		string[] array = new string[CrawledPageData.PageLinks.Count];
    		for (int i = 0; i < CrawledPageData.PageLinks.Count; i++)
    		{
    			array[i] = CrawledPageData.PageLinks[i].NormalizedOriginalLink;
    		}
    		object obj = new
    		{
    			PageUrl = CrawledPageData.DownloadedPageUrl.NormalizedOriginalLink,
    			PageCrawledDateTime = CrawledPageData.CrawledPageMeta.PageCrawledDateTime.ToString("dd.MM.yyyy HH:mm:ss"),
    			PageCrawlingTimeSeconds = CrawledPageData.CrawledPageMeta.PageCrawlingTime.TotalSeconds,
    			PageCrawledAttemptNumber = CrawledPageData.CrawledPageMeta.PageDownloadAttemptNumber,
    			PageLinks = array
    		};
    		string contents = JsonConvert.SerializeObject(obj, (Formatting)1);
    		File.WriteAllText(path2, contents);
    	}

    	/// <summary>
    	/// Saves actual state of the links buffer
    	/// </summary>
    	/// <param name="LinksBufferLink"></param>
    	public void SaveLinksBuffer(CrawlingServerLinksBuffer LinksBufferLink)
    	{
    		CrawlingServerLinksBuffer crawlingServerLinksBuffer = (CrawlingServerLinksBuffer)LinksBufferLink.Clone();
    		string text = string.Format("{0}/{1}", CrawlerLink.ParentTaskOperatingDirectory, "crawled-data");
    		if (!Directory.Exists(text))
    		{
    			Directory.CreateDirectory(text);
    		}
    		string path = string.Format("{0}/{1}", text, "links-buffer.json");
    		string contents = crawlingServerLinksBuffer.SerializeToJSON();
    		File.WriteAllText(path, contents);
    	}

    	/// <summary>
    	/// Try to load links buffer from HDD
    	/// </summary>
    	/// <returns>[nullable] Saved links buffer from HDD</returns>
    	public CrawlingServerLinksBuffer TryToLoadLinksBuffer()
    	{
    		string text = string.Format("{0}/{1}", CrawlerLink.ParentTaskOperatingDirectory, "crawled-data");
    		if (!Directory.Exists(text))
    		{
    			Directory.CreateDirectory(text);
    		}
    		string path = string.Format("{0}/{1}", text, "links-buffer.json");
    		if (File.Exists(path))
    		{
    			string text2 = File.ReadAllText(path);
    			if (text2.Length > 0)
    			{
    				CrawlingServerLinksBuffer crawlingServerLinksBuffer = new CrawlingServerLinksBuffer();
    				try
    				{
    					return crawlingServerLinksBuffer.UnserializeFromJSON(text2);
    				}
    				catch (Exception)
    				{
    				}
    			}
    		}
    		return null;
    	}

    	/// <summary>
    	/// Writes robots.txt file to HDD
    	/// </summary>
    	/// <param name="RobotsTxtFile">Link to robots.txt file</param>
    	public void SaveRobotsTxtFile(DERobotsTxtFile RobotsTxtFile)
    	{
    		string text = string.Format("{0}/{1}", CrawlerLink.ParentTaskOperatingDirectory, "crawled-data");
    		if (!Directory.Exists(text))
    		{
    			Directory.CreateDirectory(text);
    		}
    		string path = string.Format("{0}/{1}", text, "robots-actual.json");
    		File.WriteAllText(path, RobotsTxtFile.SerializeToJSON());
    	}

    	/// <summary>
    	/// Reads robots.txt data from HDD
    	/// </summary>
    	/// <returns>New instance of DERobotsTxt</returns>
    	public DERobotsTxtFile ReadRobotsTxtFile()
    	{
    		string text = string.Format("{0}/{1}", CrawlerLink.ParentTaskOperatingDirectory, "crawled-data");
    		if (!Directory.Exists(text))
    		{
    			Directory.CreateDirectory(text);
    		}
    		string path = string.Format("{0}/{1}", text, "robots-actual.json");
    		if (File.Exists(path))
    		{
    			string jSONData = File.ReadAllText(path);
    			DERobotsTxtFile dERobotsTxtFile = new DERobotsTxtFile();
    			return dERobotsTxtFile.UnserializeFromJSON(jSONData);
    		}
    		return null;
    	}

    	/// <summary>
    	/// Writes sitemap file to HDD
    	/// </summary>
    	/// <param name="SitemapFile">Sitemap file object</param>
    	public void SaveSitemapFile(DESitemapFile SitemapFile)
    	{
    		string text = string.Format("{0}/{1}", CrawlerLink.ParentTaskOperatingDirectory, "crawled-data");
    		if (!Directory.Exists(text))
    		{
    			Directory.CreateDirectory(text);
    		}
    		string path = string.Format("{0}/{1}", text, "sitemap-actual.json");
    		File.WriteAllText(path, SitemapFile.SerializeToJSON());
    	}

    	/// <summary>
    	/// Reads sitemap file from HDD
    	/// </summary>
    	/// <returns>Sitemap file object</returns>
    	public DESitemapFile ReadSitemapFile()
    	{
    		string text = string.Format("{0}/{1}", CrawlerLink.ParentTaskOperatingDirectory, "crawled-data");
    		if (!Directory.Exists(text))
    		{
    			Directory.CreateDirectory(text);
    		}
    		string path = string.Format("{0}/{1}", text, "sitemap-actual.json");
    		if (File.Exists(path))
    		{
    			string jSONData = File.ReadAllText(path);
    			DESitemapFile dESitemapFile = new DESitemapFile();
    			return dESitemapFile.UnserializeFromJSON(jSONData);
    		}
    		return null;
    	}

    	/// <summary>
    	/// Deletes old sitemap file
    	/// </summary>
    	public bool DeleteSitemapFile()
    	{
    		string text = string.Format("{0}/{1}", CrawlerLink.ParentTaskOperatingDirectory, "crawled-data");
    		if (!Directory.Exists(text))
    		{
    			Directory.CreateDirectory(text);
    		}
    		string path = string.Format("{0}/{1}", text, "sitemap-actual.json");
    		if (File.Exists(path))
    		{
    			File.Delete(path);
    			return true;
    		}
    		return false;
    	}
    }
}
