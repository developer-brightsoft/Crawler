// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Crawler.DESitemapAnalyser
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml;
using ExcavatorSharp.Excavator;
using ExcavatorSharp.Objects;

namespace ExcavatorSharp.Crawler
{
    /// <summary>
    /// Class for analysing sitemaps, associates with some website. Allowed format - only XML sitemaps.
    /// </summary>
    public class DESitemapAnalyser
    {
    	/// <summary>
    	/// Crawler link
    	/// </summary>
    	private CrawlingServer CrawlerLink { get; set; }

    	/// <summary>
    	/// Link to task logger object
    	/// </summary>
    	private DataExcavatorTasksLogger TaskLoggerLink { get; set; }

    	/// <summary>
    	/// Creates new instance of DESitemapAnalyser
    	/// </summary>
    	/// <param name="CrawlerLink">Link to parent crawling server</param>
    	public DESitemapAnalyser(CrawlingServer CrawlerLink, DataExcavatorTasksLogger TaskLoggerLink)
    	{
    		this.CrawlerLink = CrawlerLink;
    		this.TaskLoggerLink = TaskLoggerLink;
    	}

    	/// <summary>
    	/// Tryes to parse sitemap by specified URL
    	/// </summary>
    	/// <param name="SitemapUrl">Sitemap URL</param>
    	/// <returns>Parsed sitemap data</returns>
    	public DESitemapParsedCallback DownloadAndAnalyseSitemapFile(string SitemapUrl)
    	{
    		TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Trying to load sitemap from url = {SitemapUrl}");
    		PageLink pageLink = new PageLink(SitemapUrl);
    		WebsiteInnerLinksAnalyser websiteInnerLinksAnalyser = new WebsiteInnerLinksAnalyser();
    		websiteInnerLinksAnalyser.AnalyseWebsitePageLink(CrawlerLink.WebsiteBaseUrl, null, pageLink);
    		if (pageLink.LinkResourceExtension != "xml")
    		{
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Error reading sitemap file with url = {SitemapUrl}, sitemap extension {pageLink.LinkResourceExtension} not supported.");
    			return new DESitemapParsedCallback(null, IsParsedSuccessfully: false, $"Wrong sitemap extension = {pageLink.LinkResourceExtension}");
    		}
    		BinaryResourceDownloader binaryResourceDownloader = new BinaryResourceDownloader(CrawlerLink);
    		PageLink pageLink2 = new PageLink(SitemapUrl);
    		pageLink2.NormalizedOriginalLink = SitemapUrl;
    		BinaryResourceDownloadingResult binaryResourceDownloadingResult = binaryResourceDownloader.DownloadResource(pageLink2);
    		CrawlingDataIO crawlingDataIO = new CrawlingDataIO(CrawlerLink);
    		if (binaryResourceDownloadingResult.ResourceHttpStatusCode != HttpStatusCode.OK)
    		{
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Error reading sitemap file with url = {SitemapUrl}, HttpStatusCode={binaryResourceDownloadingResult.ResourceHttpStatusCode}");
    			return new DESitemapParsedCallback(null, IsParsedSuccessfully: false, $"Error reading sitemap file. HttpStatusCode={binaryResourceDownloadingResult.ResourceHttpStatusCode}");
    		}
    		if (binaryResourceDownloadingResult.ResourceData == null || binaryResourceDownloadingResult.ResourceData.Length == 0)
    		{
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Error reading sitemap file with url = {SitemapUrl}. Sitemap content length = 0");
    			return new DESitemapParsedCallback(null, IsParsedSuccessfully: false, $"Error reading sitemap file. Sitemap content length = 0");
    		}
    		string empty = string.Empty;
    		try
    		{
    			Stream stream = new MemoryStream(binaryResourceDownloadingResult.ResourceData);
    			TextReader textReader = new StreamReader(stream);
    			empty = textReader.ReadToEnd();
    			textReader.Close();
    			textReader.Dispose();
    			stream.Close();
    			stream.Dispose();
    			textReader = null;
    			stream = null;
    		}
    		catch (Exception ex)
    		{
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Error reading sitemap file with url = {SitemapUrl}", ex);
    			return new DESitemapParsedCallback(null, IsParsedSuccessfully: false, $"Error reading sitemap file, exception data = {ex.Message}");
    		}
    		XmlDocument xmlDocument = new XmlDocument();
    		try
    		{
    			xmlDocument.LoadXml(empty);
    		}
    		catch (Exception ex2)
    		{
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Error parsing sitemap file with url = {SitemapUrl}", ex2);
    			return new DESitemapParsedCallback(null, IsParsedSuccessfully: false, $"Error parsing sitemap file, exception data = {ex2.Message}");
    		}
    		if (xmlDocument.ChildNodes.Count == 0)
    		{
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Error parsing sitemap file with url = {SitemapUrl}. Sitemap does not contains data nodes.");
    			return new DESitemapParsedCallback(null, IsParsedSuccessfully: false, $"Error parsing sitemap file. Sitemap does not contains data nodes.");
    		}
    		XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
    		xmlNamespaceManager.AddNamespace("s", "http://www.sitemaps.org/schemas/sitemap/0.9");
    		DESitemapFile dESitemapFile = new DESitemapFile();
    		dESitemapFile.SitemapLocation = SitemapUrl;
    		dESitemapFile.LastReindexDateTime = DateTime.Now;
    		if (xmlDocument.SelectNodes("/s:sitemapindex", xmlNamespaceManager).Count > 0)
    		{
    			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/s:sitemapindex/s:sitemap", xmlNamespaceManager);
    			foreach (XmlNode item in xmlNodeList)
    			{
    				DESitemapContentEntry dESitemapContentEntry = FetchNextEntry(item, xmlNamespaceManager);
    				if (dESitemapContentEntry != null)
    				{
    					dESitemapContentEntry.LinkType = DESitemapContentEntryType.SitemapLink;
    					dESitemapFile.ContentEntries.Add(dESitemapContentEntry);
    					string location = dESitemapContentEntry.Location;
    					DESitemapParsedCallback dESitemapParsedCallback = DownloadAndAnalyseSitemapFile(dESitemapContentEntry.Location);
    					if (dESitemapParsedCallback.SitemapData != null && dESitemapParsedCallback.IsParsedSuccessfully)
    					{
    						dESitemapFile.ChildSitemaps.Add(dESitemapParsedCallback.SitemapData);
    					}
    				}
    			}
    		}
    		else if (xmlDocument.SelectNodes("/s:urlset", xmlNamespaceManager).Count > 0)
    		{
    			XmlNodeList xmlNodeList2 = xmlDocument.SelectNodes("/s:urlset/s:url", xmlNamespaceManager);
    			foreach (XmlNode item2 in xmlNodeList2)
    			{
    				DESitemapContentEntry dESitemapContentEntry2 = FetchNextEntry(item2, xmlNamespaceManager);
    				if (dESitemapContentEntry2 != null)
    				{
    					dESitemapContentEntry2.LinkType = DESitemapContentEntryType.PageLink;
    					dESitemapFile.ContentEntries.Add(dESitemapContentEntry2);
    				}
    			}
    		}
    		TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Sitemap with url = {SitemapUrl} parsed. Sitemap includes {dESitemapFile.GetLinksCount()} links.");
    		return new DESitemapParsedCallback(dESitemapFile, IsParsedSuccessfully: true, "Sitemap parsed");
    	}

    	/// <summary>
    	/// Fetched Sitemap content entry from next XmlNode
    	/// </summary>
    	/// <param name="NextNode">Next xml node with sitemap row content</param>
    	/// <returns>Parsed sitemap content entry</returns>
    	private DESitemapContentEntry FetchNextEntry(XmlNode NextNode, XmlNamespaceManager SitemapNamespaceManagerLink)
    	{
    		if (!NextNode.HasChildNodes)
    		{
    			return null;
    		}
    		XmlNode xmlNode = NextNode.SelectSingleNode("s:loc", SitemapNamespaceManagerLink);
    		if (xmlNode == null)
    		{
    			return null;
    		}
    		XmlNode xmlNode2 = NextNode.SelectSingleNode("s:lastmod", SitemapNamespaceManagerLink);
    		XmlNode xmlNode3 = NextNode.SelectSingleNode("s:changefreq", SitemapNamespaceManagerLink);
    		XmlNode xmlNode4 = NextNode.SelectSingleNode("s:priority", SitemapNamespaceManagerLink);
    		DESitemapContentEntry dESitemapContentEntry = new DESitemapContentEntry();
    		dESitemapContentEntry.Location = UnescapeSpecialCharacters(xmlNode.InnerText);
    		if (xmlNode2 != null)
    		{
    			dESitemapContentEntry.LastMod = DateTime.Parse(xmlNode2.InnerText);
    		}
    		if (xmlNode3 != null)
    		{
    			dESitemapContentEntry.ChangeFreq = xmlNode3.InnerText;
    		}
    		if (xmlNode4 != null)
    		{
    			double result = 0.0;
    			if (double.TryParse(xmlNode4.InnerText.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
    			{
    				dESitemapContentEntry.Priority = result;
    			}
    		}
    		return dESitemapContentEntry;
    	}

    	private string UnescapeSpecialCharacters(string LocationUrl)
    	{
    		if (LocationUrl.IndexOf("&amp;") != -1)
    		{
    			LocationUrl = LocationUrl.Replace("&amp;", "&");
    		}
    		if (LocationUrl.IndexOf("&apos;") != -1)
    		{
    			LocationUrl = LocationUrl.Replace("&apos;", "'");
    		}
    		if (LocationUrl.IndexOf("&quot;") != -1)
    		{
    			LocationUrl = LocationUrl.Replace("&quot;", "\"");
    		}
    		if (LocationUrl.IndexOf("&gt;") != -1)
    		{
    			LocationUrl = LocationUrl.Replace("&gt;", ">");
    		}
    		if (LocationUrl.IndexOf("&lt;") != -1)
    		{
    			LocationUrl = LocationUrl.Replace("&lt;", "<");
    		}
    		return LocationUrl;
    	}
    }
}
