// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Grabber.GrabbingThread
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using ExcavatorSharp.Common;
using ExcavatorSharp.Crawler;
using ExcavatorSharp.ExtensionMethods;
using ExcavatorSharp.Objects;
using ExcavatorSharp.Parser;
using HtmlAgilityPack;

namespace ExcavatorSharp.Grabber
{
    /// <summary>
    /// Grabbing thread for parsing data from pages
    /// </summary>
    internal class GrabbingThread
    {
    	/// <summary>
    	/// Delegate for PageGrabbed event
    	/// </summary>
    	/// <param name="GrabbedPageData">Grabbed page data</param>
    	public delegate void PageGrabbedHandler(PageGrabbedCallback GrabbedPageData);

    	/// <summary>
    	/// Grabbing inner thread
    	/// </summary>
    	private Thread ThreadObject { get; set; }

    	/// <summary>
    	/// Link to parent grabbing environment
    	/// </summary>
    	private GrabbingServer GrabbingServerEnvironmentLink { get; set; }

    	/// <summary>
    	/// The main event occurring at the completion of data capture from the page
    	/// </summary>
    	public event PageGrabbedHandler PageGrabbed;

    	/// <summary>
    	/// Creates a new instance of GrabbingThread 
    	/// </summary>
    	/// <param name="GrabbingServerEnvironmentLink">Link to parent grabbing server</param>
    	public GrabbingThread(GrabbingServer GrabbingServerEnvironmentLink)
    	{
    		this.GrabbingServerEnvironmentLink = GrabbingServerEnvironmentLink;
    	}

    	/// <summary>
    	/// Start crawling pages
    	/// </summary>
    	public void StartGrabbing()
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
    	public void StopGrabbing()
    	{
    		ThreadObject.Abort();
    	}

    	/// <summary>
    	/// Main thread body
    	/// </summary>
    	private void ThreadBody()
    	{
    		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
    		//IL_0040: Expected O, but got Unknown
    		while (!DEConfig.ShutDownProgram)
    		{
    			DataGrabbingTask result = null;
    			while (!GrabbingServerEnvironmentLink.GrabbingQueue.TryDequeue(out result))
    			{
    				Thread.Sleep(10);
    			}
    			string text = result.PageCrawledCallbackData.DownloadedPageHtml;
    			HtmlDocument val = new HtmlDocument();
    			if (GrabbingServerEnvironmentLink.CrawlerLink.CrawlerProperties.ExpandPageFrames && result.PageCrawledCallbackData.DownloadedPageFrames != null)
    			{
    				try
    				{
    					string text2 = HTMLFramesHelper.ExpandIFrames(text, result.PageCrawledCallbackData.DownloadedPageFrames);
    					text = text2;
    				}
    				catch (Exception occuredException)
    				{
    					GrabbingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.GrabbingServer, $"Cannot expand iframes on page = {result.PageCrawledCallbackData.DownloadedPageUrl}", occuredException);
    				}
    			}
    			val.LoadHtml(text);
    			int num = 0;
    			Dictionary<DataGrabbingPattern, DataGrabbingResult> dictionary = new Dictionary<DataGrabbingPattern, DataGrabbingResult>();
    			for (int i = 0; i < GrabbingServerEnvironmentLink.GrabbingPatterns.Count; i++)
    			{
    				DataGrabbingPattern dataGrabbingPattern = GrabbingServerEnvironmentLink.GrabbingPatterns[i];
    				bool flag = false;
    				if (dataGrabbingPattern.AllowedPageUrlsSubstrings != null && dataGrabbingPattern.AllowedPageUrlsSubstrings.Length != 0)
    				{
    					if (dataGrabbingPattern.AllowedPageUrlsSubstrings.Length == 1 && dataGrabbingPattern.AllowedPageUrlsSubstrings[0] == "*")
    					{
    						flag = true;
    						num++;
    					}
    					else
    					{
    						string[] allowedPageUrlsSubstrings = dataGrabbingPattern.AllowedPageUrlsSubstrings;
    						foreach (string value in allowedPageUrlsSubstrings)
    						{
    							if (result.PageCrawledCallbackData.DownloadedPageUrl.NormalizedOriginalLink.ToLower().IndexOf(value) != -1)
    							{
    								flag = true;
    								num++;
    								break;
    							}
    						}
    					}
    				}
    				else
    				{
    					flag = true;
    				}
    				if (flag)
    				{
    					dictionary.Add(dataGrabbingPattern, GrabDataFromPage(result.PageCrawledCallbackData.DownloadedPageUrl, val, dataGrabbingPattern));
    				}
    			}
    			FirePageGrabbedEvent(new PageGrabbedCallback(result, dictionary, num));
    			Thread.Sleep(GrabbingServerEnvironmentLink.GrabberProperties.GrabbingThreadDelayMilliseconds);
    		}
    	}

    	/// <summary>
    	/// Grabs data from downloaded HTML page
    	/// </summary>
    	/// <param name="SourceDocument">Page document</param>
    	/// <param name="DataPattern">Data pattern for selected page</param>
    	/// <param name="PageUrl">Url of the crawled page</param>
    	/// <returns></returns>
    	private DataGrabbingResult GrabDataFromPage(PageLink PageUrl, HtmlDocument SourceDocument, DataGrabbingPattern DataPattern)
    	{
    		DataGrabbingResult dataGrabbingResult = new DataGrabbingResult
    		{
    			PageSourceHtml = SourceDocument.DocumentNode.InnerHtml,
    			GrabbedPageSourceUrl = PageUrl,
    			GrabbingResults = new List<List<KeyValuePair<DataGrabbingPatternItem, DataGrabbingResultItem>>>(),
    			PatternName = DataPattern.PatternName,
    			PageGrabbedDateTime = DateTime.Now,
    			PatternHashCode = DataPattern.GetHashCode()
    		};
    		if (DataPattern.OuterBlockSelector == null)
    		{
    			dataGrabbingResult.GrabbingResults.Add(new List<KeyValuePair<DataGrabbingPatternItem, DataGrabbingResultItem>>());
    			foreach (DataGrabbingPatternItem grabbingItemsPattern in DataPattern.GrabbingItemsPatterns)
    			{
    				List<HtmlNode> resultedNodes = null;
    				if (grabbingItemsPattern.DataSelector.Selector == "~ENTRYPAGE")
    				{
    					resultedNodes = new List<HtmlNode> { SourceDocument.DocumentNode };
    				}
    				else if (grabbingItemsPattern.DataSelector.SelectorType == DataGrabbingSelectorType.CSS_Selector)
    				{
    					resultedNodes = HapCssExtensionMethods.QuerySelectorAll(SourceDocument, grabbingItemsPattern.DataSelector.Selector).ToList();
    				}
    				else if (grabbingItemsPattern.DataSelector.SelectorType == DataGrabbingSelectorType.XPath_Selector)
    				{
    					resultedNodes = ((IEnumerable<HtmlNode>)SourceDocument.DocumentNode.SelectNodes(grabbingItemsPattern.DataSelector.Selector)).ToList();
    				}
    				dataGrabbingResult.GrabbingResults[0].Add(new KeyValuePair<DataGrabbingPatternItem, DataGrabbingResultItem>(grabbingItemsPattern, new DataGrabbingResultItem
    				{
    					ResultedNodes = resultedNodes
    				}));
    			}
    		}
    		else
    		{
    			List<HtmlNode> list = null;
    			if (DataPattern.OuterBlockSelector.SelectorType == DataGrabbingSelectorType.CSS_Selector)
    			{
    				list = HapCssExtensionMethods.QuerySelectorAll(SourceDocument, DataPattern.OuterBlockSelector.Selector).ToList();
    			}
    			else if (DataPattern.OuterBlockSelector.SelectorType == DataGrabbingSelectorType.XPath_Selector)
    			{
    				list = ((IEnumerable<HtmlNode>)SourceDocument.DocumentNode.SelectNodes(DataPattern.OuterBlockSelector.Selector)).ToList();
    			}
    			for (int i = 0; i < list.Count; i++)
    			{
    				List<KeyValuePair<DataGrabbingPatternItem, DataGrabbingResultItem>> list2 = new List<KeyValuePair<DataGrabbingPatternItem, DataGrabbingResultItem>>();
    				foreach (DataGrabbingPatternItem grabbingItemsPattern2 in DataPattern.GrabbingItemsPatterns)
    				{
    					List<HtmlNode> resultedNodes2 = null;
    					if (grabbingItemsPattern2.DataSelector.SelectorType == DataGrabbingSelectorType.CSS_Selector)
    					{
    						resultedNodes2 = HapCssExtensionMethods.QuerySelectorAll(list[i], grabbingItemsPattern2.DataSelector.Selector).ToList();
    					}
    					else if (grabbingItemsPattern2.DataSelector.SelectorType == DataGrabbingSelectorType.XPath_Selector)
    					{
    						resultedNodes2 = ((IEnumerable<HtmlNode>)list[i].SelectNodes(grabbingItemsPattern2.DataSelector.Selector)).ToList();
    					}
    					list2.Add(new KeyValuePair<DataGrabbingPatternItem, DataGrabbingResultItem>(grabbingItemsPattern2, new DataGrabbingResultItem
    					{
    						ResultedNodes = resultedNodes2
    					}));
    				}
    				if (list2.Count > 0)
    				{
    					dataGrabbingResult.GrabbingResults.Add(list2);
    				}
    			}
    		}
    		if (!dataGrabbingResult.IsEmptyResultsSet)
    		{
    			WebsiteInnerLinksAnalyser websiteInnerLinksAnalyser = null;
    			for (int j = 0; j < dataGrabbingResult.GrabbingResults.Count; j++)
    			{
    				for (int k = 0; k < dataGrabbingResult.GrabbingResults[j].Count; k++)
    				{
    					if (!dataGrabbingResult.GrabbingResults[j][k].Key.ParseBinaryAttributes || dataGrabbingResult.GrabbingResults[j][k].Value.ResultedNodes.Count <= 0)
    					{
    						continue;
    					}
    					for (int l = 0; l < dataGrabbingResult.GrabbingResults[j][k].Value.ResultedNodes.Count; l++)
    					{
    						ParsingBinaryAttributePattern[] parsingBinaryAttributes = dataGrabbingResult.GrabbingResults[j][k].Key.ParsingBinaryAttributes;
    						foreach (ParsingBinaryAttributePattern NextParsingAttribute in parsingBinaryAttributes)
    						{
    							IEnumerable<HtmlAttribute> enumerable = ((IEnumerable<HtmlAttribute>)dataGrabbingResult.GrabbingResults[j][k].Value.ResultedNodes[l].Attributes).Where((HtmlAttribute attribute) => attribute.Name == NextParsingAttribute.AttributeName);
    							if (enumerable.Count() <= 0)
    							{
    								continue;
    							}
    							foreach (HtmlAttribute item in enumerable)
    							{
    								BinaryResourceDownloadingResult binaryResourceDownloadingResult = null;
    								PageLink pageLink = null;
    								if (NextParsingAttribute.IsAttributeAreLinkToSomeResouce && NextParsingAttribute.IsWeMustDownloadContentUnderAttributeLink)
    								{
    									if (websiteInnerLinksAnalyser == null)
    									{
    										websiteInnerLinksAnalyser = new WebsiteInnerLinksAnalyser();
    									}
    									if (item.Value.IndexOf("data:") != -1 && item.Value.IndexOf("base64") != -1)
    									{
    										byte[] array = null;
    										try
    										{
    											HTTPDataURLContent hTTPDataURLContent = HTTPDataURLParser.ParseData(item.Value);
    											if (hTTPDataURLContent != null)
    											{
    												array = Convert.FromBase64String(hTTPDataURLContent.base64Data);
    												binaryResourceDownloadingResult = new BinaryResourceDownloadingResult(array, HttpStatusCode.OK, 1, new TimeSpan(0, 0, 1), null);
    											}
    										}
    										catch (Exception occuredException)
    										{
    											GrabbingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.GrabbingServer, "[GSE11] An error occurred while saving the data attribute", occuredException);
    										}
    									}
    									else
    									{
    										pageLink = new PageLink(item.Value);
    										websiteInnerLinksAnalyser.AnalyseWebsitePageLink(GrabbingServerEnvironmentLink.WebsiteBaseUrl, PageUrl, pageLink);
    										BinaryResourceDownloader binaryResourceDownloader = new BinaryResourceDownloader(GrabbingServerEnvironmentLink.CrawlerLink);
    										binaryResourceDownloadingResult = binaryResourceDownloader.DownloadResource(pageLink);
    										if (binaryResourceDownloadingResult.ResourceDownloadException != null)
    										{
    											GrabbingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.GrabbingServer, $"An error occurred while downloading the binary data, link = {pageLink.OriginalLink}", binaryResourceDownloadingResult.ResourceDownloadException);
    										}
    									}
    									if (GrabbingServerEnvironmentLink.CrawlerLink.CrawlerProperties.StoreCrawledData && (binaryResourceDownloadingResult.ResourceHttpStatusCode != HttpStatusCode.OK || binaryResourceDownloadingResult.ResourceData == null || binaryResourceDownloadingResult.ResourceData.Length == 0))
    									{
    										GrabbingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.GrabbingServer, $"Error in binary resource downloading. HttpStatusCode = {binaryResourceDownloadingResult.ResourceHttpStatusCode}");
    									}
    								}
    								byte[] resourceContent = null;
    								if (binaryResourceDownloadingResult != null)
    								{
    									resourceContent = binaryResourceDownloadingResult.ResourceData;
    								}
    								DataGrabbingResultItemBinaryAttributeData value = new DataGrabbingResultItemBinaryAttributeData(NextParsingAttribute.AttributeName, item.Value, resourceContent, Guid.NewGuid().ToString());
    								if (dataGrabbingResult.GrabbingResults[j][k].Value.NodeAttributesContent == null)
    								{
    									dataGrabbingResult.GrabbingResults[j][k].Value.NodeAttributesContent = new List<KeyValuePair<HtmlNode, DataGrabbingResultItemBinaryAttributeData>>();
    								}
    								dataGrabbingResult.GrabbingResults[j][k].Value.NodeAttributesContent.Add(new KeyValuePair<HtmlNode, DataGrabbingResultItemBinaryAttributeData>(dataGrabbingResult.GrabbingResults[j][k].Value.ResultedNodes[l], value));
    							}
    						}
    					}
    				}
    			}
    		}
    		return dataGrabbingResult;
    	}

    	/// <summary>
    	/// Fire page grabbed event
    	/// </summary>
    	/// <param name="DownloadedPageUrl"></param>
    	/// <param name="PageHtml"></param>
    	/// <param name="PageLinks"></param>
    	private void FirePageGrabbedEvent(PageGrabbedCallback GrabbedPageData)
    	{
    		this.PageGrabbed?.Invoke(GrabbedPageData);
    	}
    }
}
