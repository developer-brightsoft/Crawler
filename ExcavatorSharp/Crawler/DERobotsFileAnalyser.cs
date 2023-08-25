// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Crawler.DERobotsFileAnalyser
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using ExcavatorSharp.Objects;

namespace ExcavatorSharp.Crawler
{
    /// <summary>
    /// Class for analysing robots.txt file and it's content
    /// </summary>
    public class DERobotsFileAnalyser
    {
    	/// <summary>
    	/// Dictionary for associating Robots.txt params and inner enum
    	/// </summary>
    	private Dictionary<string, DERobotsEntryType> RobotsTXTParamsAssoc = new Dictionary<string, DERobotsEntryType>
    	{
    		{
    			"Allow",
    			DERobotsEntryType.Allow
    		},
    		{
    			"Disallow",
    			DERobotsEntryType.Disallow
    		},
    		{
    			"Sitemap",
    			DERobotsEntryType.Sitemap
    		},
    		{
    			"Clean-param",
    			DERobotsEntryType.CleanParam
    		},
    		{
    			"Crawl-delay",
    			DERobotsEntryType.CrawlDelay
    		}
    	};

    	/// <summary>
    	/// Parent crawler link
    	/// </summary>
    	private CrawlingServer CrawlerLink { get; set; }

    	/// <summary>
    	/// Creates new instance of DERobotsFileAnalyser
    	/// </summary>
    	/// <param name="CrawlerLink">Link to the parent crawling server</param>
    	public DERobotsFileAnalyser(CrawlingServer CrawlerLink)
    	{
    		this.CrawlerLink = CrawlerLink;
    	}

    	/// <summary>
    	/// Parse robots.txt file and returns objective presentation of it's content
    	/// </summary>
    	/// <param name="RobotsTXTSourceFile">Source robots.txt content</param>
    	/// <returns>New instance of DERobotsTXT</returns>
    	public DERobotsTxtFile ParseRobotsTXT(string RobotsTXTSourceFile)
    	{
    		Dictionary<string, List<DERobotsTXTInnerRow>> dictionary = new Dictionary<string, List<DERobotsTXTInnerRow>>();
    		string text = string.Empty;
    		string[] array = RobotsTXTSourceFile.Split('\n');
    		for (int i = 0; i < array.Length; i++)
    		{
    			try
    			{
    				string text2 = array[i];
    				int num = text2.IndexOf(' ');
    				string[] array2 = new string[2];
    				string text3 = text2.Substring(0, num).Replace(":", string.Empty).Trim();
    				string text4 = text2.Substring(num).Trim();
    				if (text3 == "User-agent")
    				{
    					text = text4;
    					dictionary.Add(text, new List<DERobotsTXTInnerRow>());
    				}
    				else if (dictionary.ContainsKey(text) && RobotsTXTParamsAssoc.ContainsKey(text3))
    				{
    					if (RobotsTXTParamsAssoc[text3] != DERobotsEntryType.CleanParam)
    					{
    						dictionary[text].Add(new DERobotsTXTInnerRow(text, RobotsTXTParamsAssoc[text3], text4, i, "KeyValueDirective"));
    					}
    					else
    					{
    						dictionary[text].Add(new DERobotsTXTInnerRow(text, RobotsTXTParamsAssoc[text3], text4, i, "CleanParamDirective"));
    					}
    				}
    			}
    			catch (Exception)
    			{
    			}
    		}
    		Dictionary<string, List<DERobotsTXTInnerRow>> dictionary2 = new Dictionary<string, List<DERobotsTXTInnerRow>>(dictionary.Count);
    		foreach (KeyValuePair<string, List<DERobotsTXTInnerRow>> item in dictionary)
    		{
    			List<DERobotsTXTInnerRow> value = item.Value.OrderBy((DERobotsTXTInnerRow item) => item.Value.Length).ToList();
    			dictionary2.Add(item.Key, value);
    		}
    		string respectedUserAgent = string.Empty;
    		for (int j = 0; j < CrawlerLink.CrawlerProperties.RobotsTxtUserAgentRespectationChain.Length; j++)
    		{
    			if (dictionary2.ContainsKey(CrawlerLink.CrawlerProperties.RobotsTxtUserAgentRespectationChain[j]))
    			{
    				respectedUserAgent = CrawlerLink.CrawlerProperties.RobotsTxtUserAgentRespectationChain[j];
    				break;
    			}
    		}
    		return new DERobotsTxtFile(RobotsTXTSourceFile, respectedUserAgent, DateTime.Now, dictionary);
    	}

    	/// <summary>
    	/// Reads robots.txt file from website
    	/// </summary>
    	/// <param name="WebsiteBaseUrl">Website root url</param>
    	/// <param name="CrawlerLink">Link to parent crawler</param>
    	/// <returns>New instance of DERobotsTxt file</returns>
    	public DERobotsTxtParsedCallback DownloadAndAnalyseRobotsTxtFile(string WebsiteBaseUrl)
    	{
    		BinaryResourceDownloader binaryResourceDownloader = new BinaryResourceDownloader(CrawlerLink);
    		PageLink pageLink = new PageLink(string.Format("{0}{1}", WebsiteBaseUrl, "robots.txt"));
    		pageLink.NormalizedOriginalLink = string.Format("{0}{1}", WebsiteBaseUrl, "robots.txt");
    		BinaryResourceDownloadingResult binaryResourceDownloadingResult = binaryResourceDownloader.DownloadResource(pageLink);
    		string empty = string.Empty;
    		if (binaryResourceDownloadingResult.ResourceHttpStatusCode == HttpStatusCode.OK && binaryResourceDownloadingResult.ResourceData != null && binaryResourceDownloadingResult.ResourceData.Length != 0)
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
    			DERobotsTxtFile robotsTxtFileLink = ParseRobotsTXT(empty);
    			return new DERobotsTxtParsedCallback(robotsTxtFileLink, IsParsedSuccessfully: true, "OK");
    		}
    		return new DERobotsTxtParsedCallback(null, IsParsedSuccessfully: false, $"Can't load robots.txt data, http status code = {binaryResourceDownloadingResult.ResourceHttpStatusCode}");
    	}
    }
}
