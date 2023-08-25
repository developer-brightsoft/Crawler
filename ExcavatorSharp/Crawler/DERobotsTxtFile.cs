// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Crawler.DERobotsTxtFile
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ExcavatorSharp.Interfaces;
using ExcavatorSharp.Objects;
using Newtonsoft.Json;

namespace ExcavatorSharp.Crawler
{
    /// <summary>
    /// Class for presenting robots.txt content
    /// </summary>
    public class DERobotsTxtFile : IJSONConvertible<DERobotsTxtFile>
    {
    	/// <summary>
    	/// Inner directives cache for using CanCrawlLink method 
    	/// </summary>
    	[JsonIgnore]
    	private ConcurrentDictionary<DERobotsEntryType, List<DERobotsTXTInnerRow>> AnyDirectivesCache = new ConcurrentDictionary<DERobotsEntryType, List<DERobotsTXTInnerRow>>();

    	/// <summary>
    	/// Inner directives cache for Clean-param directive
    	/// </summary>
    	[JsonIgnore]
    	private List<DERobotsTxtCleanParamInnerRow> CleanParamDirectivesCache = new List<DERobotsTxtCleanParamInnerRow>();

    	/// <summary>
    	/// Robots.txt source text
    	/// </summary>
    	public string SourceContent { get; set; }

    	/// <summary>
    	/// Actual respected user-agent
    	/// </summary>
    	public string RespectedUserAgent { get; set; }

    	/// <summary>
    	/// Last date and time of robots.txt crawling
    	/// </summary>
    	public DateTime LastCrawlingDateTime { get; set; }

    	/// <summary>
    	/// Robots.txt parsed content. Key =&gt; User-Agent, Value =&gt; User-Agent params dictionary
    	/// </summary>
    	public Dictionary<string, List<DERobotsTXTInnerRow>> ParsedOriginalContent { get; set; }

    	/// <summary>
    	/// Get website crawl-delay param
    	/// </summary>
    	[JsonIgnore]
    	public double? CrawlDelay { get; private set; }

    	/// <summary>
    	/// Presents, is file includes Clean-param directives
    	/// </summary>
    	[JsonIgnore]
    	public bool HasCleanParamDirectives { get; private set; }

    	/// <summary>
    	/// Empty ctor for import/export operations
    	/// </summary>
    	public DERobotsTxtFile()
    	{
    	}

    	/// <summary>
    	/// Creates a new instance of DERobotsTXTAnalyser
    	/// </summary> 
    	/// <param name="RobotsTxtSourceContent">Robots.txt source text</param>
    	/// <param name="RobotsTxtLastCrawlingDateTime">Last date and time of robots.txt crawling</param>
    	/// <param name="RobotsTxtParsedContent">Robots.txt parsed content. Key =&gt; User-Agent, Value =&gt; User-Agent params dictionary</param>
    	public DERobotsTxtFile(string RobotsTxtSourceContent, string RespectedUserAgent, DateTime RobotsTxtLastCrawlingDateTime, Dictionary<string, List<DERobotsTXTInnerRow>> RobotsTxtParsedContent)
    	{
    		SourceContent = RobotsTxtSourceContent;
    		this.RespectedUserAgent = RespectedUserAgent;
    		LastCrawlingDateTime = RobotsTxtLastCrawlingDateTime;
    		ParsedOriginalContent = RobotsTxtParsedContent;
    		InitializeCrawlDelayParam();
    		InitializeDirectivesCacheByRespectedUserAgent();
    	}

    	/// <summary>
    	/// Initializes JSON file if it was loaded from JSON string
    	/// </summary>
    	internal void InitializeRobotsTxtFileIfItLoadedFromJSON()
    	{
    		InitializeCrawlDelayParam();
    		InitializeDirectivesCacheByRespectedUserAgent();
    	}

    	/// <summary>
    	/// Initializes crawl-delay param after robots.txt parsing
    	/// </summary>
    	private void InitializeCrawlDelayParam()
    	{
    		CrawlDelay = null;
    		List<DERobotsTXTInnerRow> kVParamByUserAgent = GetKVParamByUserAgent(RespectedUserAgent, DERobotsEntryType.CrawlDelay);
    		if (kVParamByUserAgent.Count <= 0)
    		{
    			return;
    		}
    		DERobotsTXTInnerRow dERobotsTXTInnerRow = kVParamByUserAgent.First();
    		string text = dERobotsTXTInnerRow.Value.Trim().Replace(',', '.');
    		if (text != string.Empty)
    		{
    			double result = -1.0;
    			if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
    			{
    				CrawlDelay = result;
    			}
    		}
    	}

    	/// <summary>
    	/// Initializes cache for allow and disallow directives
    	/// </summary>
    	private void InitializeDirectivesCacheByRespectedUserAgent()
    	{
    		List<DERobotsTXTInnerRow> kVParamByUserAgent = GetKVParamByUserAgent(RespectedUserAgent, DERobotsEntryType.Allow);
    		List<DERobotsTXTInnerRow> kVParamByUserAgent2 = GetKVParamByUserAgent(RespectedUserAgent, DERobotsEntryType.Disallow);
    		AnyDirectivesCache.TryAdd(DERobotsEntryType.Allow, kVParamByUserAgent);
    		AnyDirectivesCache.TryAdd(DERobotsEntryType.Disallow, kVParamByUserAgent2);
    		CleanParamDirectivesCache = GetCleanParamByUserAgent(RespectedUserAgent);
    		HasCleanParamDirectives = CleanParamDirectivesCache.Count > 0;
    	}

    	/// <summary>
    	/// Checks link to crawling possibility by respected User-Agent
    	/// Based on algorhytm: https://developers.google.com/search/reference/robots_txt?hl=ru
    	/// </summary>
    	/// <param name="PageNormalizedURL">Link normalized URL</param>
    	/// <returns>Link check results</returns>
    	public bool CanCrawlLink(string PageNormalizedURL)
    	{
    		List<DERobotsTXTInnerRow> source = AnyDirectivesCache[DERobotsEntryType.Allow];
    		List<DERobotsTXTInnerRow> source2 = AnyDirectivesCache[DERobotsEntryType.Disallow];
    		DERobotsTXTInnerRow dERobotsTXTInnerRow = source.LastOrDefault((DERobotsTXTInnerRow item) => (item.ValueRegex == string.Empty) ? (PageNormalizedURL.IndexOf(item.Value) != -1) : Regex.IsMatch(PageNormalizedURL, item.ValueRegex));
    		DERobotsTXTInnerRow dERobotsTXTInnerRow2 = source2.LastOrDefault((DERobotsTXTInnerRow item) => (item.ValueRegex == string.Empty) ? (PageNormalizedURL.IndexOf(item.Value) != -1) : Regex.IsMatch(PageNormalizedURL, item.ValueRegex));
    		if (dERobotsTXTInnerRow == null && dERobotsTXTInnerRow2 == null)
    		{
    			return true;
    		}
    		if (dERobotsTXTInnerRow != null && dERobotsTXTInnerRow2 == null)
    		{
    			return true;
    		}
    		if (dERobotsTXTInnerRow2 != null && dERobotsTXTInnerRow == null)
    		{
    			return false;
    		}
    		if (dERobotsTXTInnerRow != null && dERobotsTXTInnerRow2 != null)
    		{
    			if (dERobotsTXTInnerRow.Value.Length > dERobotsTXTInnerRow2.Value.Length)
    			{
    				return true;
    			}
    			if (dERobotsTXTInnerRow.Value.Length < dERobotsTXTInnerRow2.Value.Length)
    			{
    				return false;
    			}
    		}
    		return true;
    	}

    	/// <summary>
    	/// Returns parameter(s) by user agent and name. If param not specified, will return empty list.
    	/// </summary>
    	/// <param name="UserAgent">Using user agent</param>
    	/// <param name="Param">Required param name</param>
    	/// <returns>Values list</returns>
    	public List<DERobotsTXTInnerRow> GetKVParamByUserAgent(string UserAgent, DERobotsEntryType Param)
    	{
    		if (!ParsedOriginalContent.ContainsKey(UserAgent))
    		{
    			return new List<DERobotsTXTInnerRow>();
    		}
    		List<DERobotsTXTInnerRow> list = new List<DERobotsTXTInnerRow>();
    		foreach (DERobotsTXTInnerRow item in ParsedOriginalContent[UserAgent])
    		{
    			if (item.ParamName == Param)
    			{
    				list.Add(item);
    			}
    		}
    		return list;
    	}

    	/// <summary>
    	/// Returns clean-param parameter(s) by users agent. If param not specified, will return empty list.
    	/// </summary>
    	/// <param name="UserAgent"></param>
    	/// <returns></returns>
    	public List<DERobotsTxtCleanParamInnerRow> GetCleanParamByUserAgent(string UserAgent)
    	{
    		if (!ParsedOriginalContent.ContainsKey(UserAgent))
    		{
    			return new List<DERobotsTxtCleanParamInnerRow>();
    		}
    		List<DERobotsTxtCleanParamInnerRow> list = new List<DERobotsTxtCleanParamInnerRow>();
    		foreach (DERobotsTXTInnerRow item in ParsedOriginalContent[UserAgent])
    		{
    			if (item.RowType == "CleanParamDirective")
    			{
    				list.Add(DERobotsTxtCleanParamInnerRow.FromRawData(item));
    			}
    		}
    		return list;
    	}

    	/// <summary>
    	/// Returns sitemap from actual respected User-agent section OR first founded sitemap from the all directives
    	/// </summary>
    	/// <returns>Sitemap link or string.Empty</returns>
    	public string GetSitemapUrl(CrawlingServer CrawlerLink)
    	{
    		List<DERobotsTXTInnerRow> kVParamByUserAgent = GetKVParamByUserAgent(RespectedUserAgent, DERobotsEntryType.Sitemap);
    		if (kVParamByUserAgent.Count > 0)
    		{
    			return kVParamByUserAgent.First().Value;
    		}
    		for (int i = 0; i < CrawlerLink.CrawlerProperties.RobotsTxtUserAgentRespectationChain.Length; i++)
    		{
    			if (CrawlerLink.CrawlerProperties.RobotsTxtUserAgentRespectationChain[i] != RespectedUserAgent)
    			{
    				List<DERobotsTXTInnerRow> kVParamByUserAgent2 = GetKVParamByUserAgent(CrawlerLink.CrawlerProperties.RobotsTxtUserAgentRespectationChain[i], DERobotsEntryType.Sitemap);
    				if (kVParamByUserAgent2.Count > 0)
    				{
    					return kVParamByUserAgent2.First().Value;
    				}
    			}
    		}
    		return string.Empty;
    	}

    	/// <summary>
    	/// Cleans URL params if directive defined in robots.txt
    	/// </summary>
    	/// <param name="Url"></param>
    	/// <returns>Url with cleaned parameters</returns>
    	public string TryToCleanUrlParams(string Url)
    	{
    		if (CleanParamDirectivesCache.Count == 0)
    		{
    			return Url;
    		}
    		foreach (DERobotsTxtCleanParamInnerRow item in CleanParamDirectivesCache)
    		{
    			if (item == null)
    			{
    				continue;
    			}
    			if (item.Value != string.Empty)
    			{
    				if (Url.IndexOf(item.Value) == -1)
    				{
    					continue;
    				}
    				for (int i = 0; i < item.CleaningParamsStrReplacePrepared.Length; i++)
    				{
    					if (Url.IndexOf(item.CleaningParamsStrReplacePrepared[i]) != -1)
    					{
    						Url = CleanUrlParamWithItsValue(Url, item.CleaningParamsStrReplacePrepared[i]);
    					}
    				}
    				continue;
    			}
    			for (int j = 0; j < item.CleaningParamsStrReplacePrepared.Length; j++)
    			{
    				if (Url.IndexOf(item.CleaningParamsStrReplacePrepared[j]) != -1)
    				{
    					Url = CleanUrlParamWithItsValue(Url, item.CleaningParamsStrReplacePrepared[j]);
    				}
    			}
    		}
    		return Url;
    	}

    	/// <summary>
    	/// Replaces parameter and it's value to string.Empty
    	/// </summary>
    	/// <param name="Url">Some url with replacing params</param>
    	/// <param name="ReplacingParam">Replacing param full name with special characters</param>
    	/// <returns>Cleaned url version</returns>
    	private string CleanUrlParamWithItsValue(string Url, string ReplacingParam)
    	{
    		int num = Url.IndexOf(ReplacingParam);
    		int num2 = -1;
    		num2 = Url.IndexOf('&', num + 1);
    		if (num2 == -1)
    		{
    			Url = Url.Substring(0, num);
    		}
    		else
    		{
    			string arg = Url.Substring(0, num);
    			string arg2 = Url.Substring(num2);
    			Url = $"{arg}{arg2}";
    		}
    		return Url;
    	}

    	/// <summary>
    	/// Serializes DERobotsTxt into JSON 
    	/// </summary>
    	/// <returns></returns>
    	public string SerializeToJSON()
    	{
    		return JsonConvert.SerializeObject((object)this, (Formatting)1);
    	}

    	/// <summary>
    	/// Deserializes DERobotsTxt from JSON data
    	/// </summary>
    	/// <param name="JSONData">Source JSON data</param>
    	/// <returns>New instance of DERobotsTxt</returns>
    	public DERobotsTxtFile UnserializeFromJSON(string JSONData)
    	{
    		DERobotsTxtFile dERobotsTxtFile = JsonConvert.DeserializeObject<DERobotsTxtFile>(JSONData);
    		dERobotsTxtFile.InitializeRobotsTxtFileIfItLoadedFromJSON();
    		return dERobotsTxtFile;
    	}
    }
}
