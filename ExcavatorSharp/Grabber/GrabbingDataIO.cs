// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Grabber.GrabbingDataIO
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExcavatorSharp.Common;
using ExcavatorSharp.Crawler;
using ExcavatorSharp.Objects;
using ExcavatorSharp.Parser;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace ExcavatorSharp.Grabber
{
    /// <summary>
    /// IO operations for grabbed data
    /// </summary>
    internal class GrabbingDataIO
    {
    	/// <summary>
    	/// Constant with name of folder contains grabbed data
    	/// </summary>
    	public const string GrabbedDataFolderName = "grabbed-data";

    	/// <summary>
    	/// Constant with name of file that stores meta logs about grabbed data
    	/// </summary>
    	public const string GrabbedPagesListMetaFileName = "meta-logs.json";

    	/// <summary>
    	/// Constant with name of file that stores parsed data
    	/// </summary>
    	public const string ParsedDataFileName = "parsed-data.json";

    	/// <summary>
    	/// Folder name for storing binary data
    	/// </summary>
    	public const string ParsedBinaryDataFolderName = "binary-files";

    	/// <summary>
    	/// Constant with name of folder contains grabbing patterns
    	/// </summary>
    	public const string ParsingDataPatternsFolderName = "data-patterns";

    	/// <summary>
    	/// Format of a file for storing pattern data
    	/// </summary>
    	public const string PatternFileNameFormat = "pattern.{0}.json";

    	/// <summary>
    	/// Cash for storing already saved data patterns
    	/// </summary>
    	private List<int> SavedDataPatternsCache = new List<int>();

    	/// <summary>
    	/// Parent grabbing server link
    	/// </summary>
    	private GrabbingServer GrabberLink { get; set; }

    	/// <summary>
    	/// Creates new instance of grabbing data saver
    	/// </summary>
    	/// <param name="GrabberLink">Link to grabbing server</param>
    	public GrabbingDataIO(GrabbingServer GrabberLink)
    	{
    		this.GrabberLink = GrabberLink;
    	}

    	/// <summary>
    	/// Saves data grabbing patterns with a versions control
    	/// </summary>
    	/// <param name="PatternsList">List of saving patterns</param>
    	public void SaveDataPatternsWithVersionControl(List<DataGrabbingPattern> PatternsList)
    	{
    		string text = string.Format("{0}/{1}", GrabberLink.ParentTaskOperatingDirectory, "grabbed-data");
    		if (!Directory.Exists(text))
    		{
    			Directory.CreateDirectory(text);
    		}
    		string text2 = string.Format("{0}/{1}", text, "data-patterns");
    		if (!Directory.Exists(text2))
    		{
    			Directory.CreateDirectory(text2);
    		}
    		GrabberLink.PatternsIOMutex.WaitOne();
    		foreach (DataGrabbingPattern Patterns in PatternsList)
    		{
    			int hashCode = Patterns.GetHashCode();
    			if (SavedDataPatternsCache.IndexOf(hashCode) == -1)
    			{
    				string arg = $"pattern.{hashCode}.json";
    				string path = $"{text2}/{arg}";
    				if (!File.Exists(path))
    				{
    					File.WriteAllText(path, Patterns.SerializeToJSON());
    				}
    				SavedDataPatternsCache.Add(hashCode);
    			}
    		}
    		GrabberLink.PatternsIOMutex.ReleaseMutex();
    	}

    	/// <summary>
    	/// Saves grabbed data to HDD, packed into inner-format JSON.
    	/// </summary>
    	public GrabbedDataGroupContainer SaveGrabbedDataInOriginalFormat(PageGrabbedCallback GrabbedDataCallback)
    	{
    		string text = string.Format("{0}/{1}", GrabberLink.ParentTaskOperatingDirectory, "grabbed-data");
    		if (!Directory.Exists(text))
    		{
    			Directory.CreateDirectory(text);
    		}
    		string text2 = string.Format("{0}/", DateTime.Now.ToString("dd.MM.yyyy"));
    		string text3 = $"{text}/{text2}";
    		if (!Directory.Exists(text3))
    		{
    			Directory.CreateDirectory(text3);
    		}
    		if ((GrabbedDataCallback.GrabbingResults == null || GrabbedDataCallback.GrabbingResults.Count == 0 || GrabbedDataCallback.GrabbingResults.ElementAt(0).Value.GrabbingResults.Count == 0) && GrabberLink.GrabberProperties.StoreOnlyNonEmptyData)
    		{
    			AddInformationAboutGrabbedPageIntoMetafile(DateTime.Now, GrabbedDataCallback.GrabbingTask.PageCrawledCallbackData.DownloadedPageUrl.NormalizedOriginalLink, HasResults: false, string.Empty, 0.0, 0);
    			return null;
    		}
    		string text4 = Guid.NewGuid().ToString();
    		text2 += text4;
    		string text5 = $"{text3}/{text4}";
    		if (!Directory.Exists(text5))
    		{
    			Directory.CreateDirectory(text5);
    		}
    		string path = string.Format("{0}/{1}", text5, "parsed-data.json");
    		string binaryFilesSavingPath = string.Format("{0}/{1}", text5, "binary-files");
    		GrabbedDataGroupContainer grabbedDataGroupContainer = ConvertPageGrabbedCallbackToGrabbedDataGroup(GrabbedDataCallback, SaveBinaryFiles: true, binaryFilesSavingPath);
    		grabbedDataGroupContainer.UpdateDataGroupMetrics();
    		string contents = JsonConvert.SerializeObject((object)grabbedDataGroupContainer.GrabbedDataGroups, (Formatting)1);
    		try
    		{
    			File.WriteAllText(path, contents);
    		}
    		catch (Exception occuredException)
    		{
    			GrabberLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.GrabbingServer, "[GSE1] Error during grabbed data saving", occuredException);
    		}
    		double num = Math.Round(grabbedDataGroupContainer.DataGroupMetrics.GrabbedDataTotalSizeKb / 1024.0, 0);
    		try
    		{
    			IOCommon iOCommon = new IOCommon();
    			string destDirName = $"{text5};datasize={num}Kb";
    			text2 += $";datasize={num}Kb";
    			Directory.Move(text5, destDirName);
    		}
    		catch (Exception occuredException2)
    		{
    			GrabberLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.GrabbingServer, "[GSE2] Error during grabbed data directory moving", occuredException2);
    		}
    		AddInformationAboutGrabbedPageIntoMetafile(GrabbedDataCallback.GrabbingResults.ElementAt(0).Value.PageGrabbedDateTime, GrabbedDataCallback.GrabbingTask.PageCrawledCallbackData.DownloadedPageUrl.NormalizedOriginalLink, grabbedDataGroupContainer.DataGroupMetrics.HasResults, text2, num, grabbedDataGroupContainer.DataGroupMetrics.BinaryFilesCount);
    		return grabbedDataGroupContainer;
    	}

    	/// <summary>
    	/// Packs PageGrabbedCallback to objective inner-format presentation
    	/// </summary>
    	/// <param name="GrabbedPageData">Grabbed page data callback</param>
    	/// <param name="BinaryFilesSavingPath">Path for saving binary files</param>
    	/// <returns></returns>
    	public GrabbedDataGroupContainer ConvertPageGrabbedCallbackToGrabbedDataGroup(PageGrabbedCallback GrabbedDataCallback, bool SaveBinaryFiles, string BinaryFilesSavingPath)
    	{
    		GrabbedDataGroupContainer grabbedDataGroupContainer = new GrabbedDataGroupContainer(GrabbedDataCallback.GrabbingTask.PageCrawledCallbackData.DownloadedPageUrl.NormalizedOriginalLink);
    		WebsiteInnerLinksAnalyser websiteInnerLinksAnalyser = new WebsiteInnerLinksAnalyser();
    		foreach (KeyValuePair<DataGrabbingPattern, DataGrabbingResult> grabbingResult in GrabbedDataCallback.GrabbingResults)
    		{
    			List<List<GroupedDataItem>> list = new List<List<GroupedDataItem>>();
    			foreach (List<KeyValuePair<DataGrabbingPatternItem, DataGrabbingResultItem>> grabbingResult2 in grabbingResult.Value.GrabbingResults)
    			{
    				list.Add(new List<GroupedDataItem>());
    				foreach (KeyValuePair<DataGrabbingPatternItem, DataGrabbingResultItem> item2 in grabbingResult2)
    				{
    					GroupedDataItemDescendant[] array = new GroupedDataItemDescendant[item2.Value.ResultedNodes.Count];
    					for (int i = 0; i < array.Length; i++)
    					{
    						List<DescendantAttributeData> list2 = new List<DescendantAttributeData>();
    						if (item2.Value.NodeAttributesContent != null && item2.Value.NodeAttributesContent.Count > 0)
    						{
    							foreach (KeyValuePair<HtmlNode, DataGrabbingResultItemBinaryAttributeData> item3 in item2.Value.NodeAttributesContent)
    							{
    								if (!((object)item3.Key).Equals((object)item2.Value.ResultedNodes[i]))
    								{
    									continue;
    								}
    								string text = string.Empty;
    								bool isFileSuccesfullySaved = false;
    								if (SaveBinaryFiles)
    								{
    									if (item3.Value.ResourceContent != null && item3.Value.ResourceContent.Length != 0)
    									{
    										if (!Directory.Exists(BinaryFilesSavingPath))
    										{
    											Directory.CreateDirectory(BinaryFilesSavingPath);
    										}
    										string text2 = websiteInnerLinksAnalyser.GetFileExtension(websiteInnerLinksAnalyser.GetLinkWithoutArguments(item3.Value.AttributeValue));
    										if (text2 == string.Empty && item3.Value.AttributeValue.IndexOf("data:") != -1)
    										{
    											try
    											{
    												HTTPDataURLContent hTTPDataURLContent = HTTPDataURLParser.ParseData(item3.Value.AttributeValue);
    												if (hTTPDataURLContent.contentDetails != null && hTTPDataURLContent.contentDetails.Length > 0)
    												{
    													text2 = hTTPDataURLContent.contentDetails;
    												}
    											}
    											catch (Exception occuredException)
    											{
    												GrabberLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.GrabbingServer, "[GSE9] Error during binary attribute data parsing (data:)", occuredException);
    											}
    										}
    										text = $"{item3.Value.AttributeDataGuid}.{text2}";
    										string path = $"{BinaryFilesSavingPath}/{text}";
    										try
    										{
    											File.WriteAllBytes(path, item3.Value.ResourceContent);
    											isFileSuccesfullySaved = true;
    										}
    										catch (Exception occuredException2)
    										{
    											GrabberLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.GrabbingServer, $"[GSE10] Exception during binary file saving", occuredException2);
    											isFileSuccesfullySaved = false;
    										}
    									}
    								}
    								else
    								{
    									text = item3.Value.AttributeDataGuid;
    								}
    								grabbedDataGroupContainer.BinaryDataItems.Add(item3.Value);
    								list2.Add(new DescendantAttributeData
    								{
    									AttributeName = item3.Value.AttributeName,
    									AttributeValue = item3.Value.AttributeValue,
    									ContentNotNullable = (item3.Value.ResourceContent != null && item3.Value.ResourceContent.Length != 0),
    									AttributeSavedFileName = text,
    									IsFileSuccesfullySaved = isFileSuccesfullySaved
    								});
    							}
    						}
    						array[i] = new GroupedDataItemDescendant
    						{
    							ElementSequenceNr = i + 1,
    							ElementOuterHtml = item2.Value.ResultedNodes[i].OuterHtml,
    							ElementInnerText = item2.Value.ResultedNodes[i].InnerText,
    							ElementAttributes = list2.ToArray()
    						};
    					}
    					list.Last().Add(new GroupedDataItem
    					{
    						DataGrabbingPatternItemElementName = item2.Key.ElementName,
    						GrabbedItemsData = array
    					});
    				}
    			}
    			GrabbedDataGroup item = new GrabbedDataGroup
    			{
    				PatternName = grabbingResult.Key.PatternName,
    				PatternHash = grabbingResult.Key.GetHashCode(),
    				GrabbedPageUrl = grabbingResult.Value.GrabbedPageSourceUrl.NormalizedOriginalLink,
    				IsEmptyResultSet = grabbingResult.Value.IsEmptyResultsSet,
    				PageGrabbedDateTime = grabbingResult.Value.PageGrabbedDateTime,
    				GrabbingResults = list
    			};
    			grabbedDataGroupContainer.GrabbedDataGroups.Add(item);
    		}
    		return grabbedDataGroupContainer;
    	}

    	/// <summary>
    	/// Adds information about next grabbed page
    	/// </summary>
    	/// <param name="PageGrabbedDateTime">Date and time when page was grabbed</param>
    	/// <param name="GrabbedPageUrl">Page original URL</param>
    	/// <param name="HasResults">Has results on page</param>
    	/// <param name="ResultsFolderLink">Link to folder with results</param>
    	/// <param name="DataSizeKb">Data size of grabbed data</param>
    	/// <param name="BinaryFilesCount">Binary files count</param>
    	private void AddInformationAboutGrabbedPageIntoMetafile(DateTime PageGrabbedDateTime, string GrabbedPageUrl, bool HasResults, string ResultsFolderLink, double DataSizeKb, int BinaryFilesCount)
    	{
    		bool flag = false;
    		string text = string.Format("{0}/{1}", GrabberLink.ParentTaskOperatingDirectory, "grabbed-data");
    		if (!Directory.Exists(text))
    		{
    			Directory.CreateDirectory(text);
    		}
    		string path = string.Format("{0}/{1}", text, "meta-logs.json");
    		try
    		{
    			GrabberLink.CommonMetaDataSavingMutex.WaitOne();
    			if (!File.Exists(path))
    			{
    				File.WriteAllText(path, "[]");
    				flag = true;
    			}
    			else
    			{
    				using FileStream stream = new FileStream(path, FileMode.Open);
    				using StreamReader streamReader = new StreamReader(stream);
    				string text2 = streamReader.ReadLine().Trim();
    				if (text2 == "[]")
    				{
    					flag = true;
    				}
    			}
    		}
    		catch (Exception occuredException)
    		{
    			GrabberLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.GrabbingServer, "[GSE3] Error during grabbed meta information addition", occuredException);
    		}
    		finally
    		{
    			if (!flag)
    			{
    				GrabberLink.CommonMetaDataSavingMutex.ReleaseMutex();
    			}
    		}
    		GrabbedPageMetaInformationDataEntry grabbedPageMetaInformationDataEntry = new GrabbedPageMetaInformationDataEntry(PageGrabbedDateTime, GrabbedPageUrl, HasResults, ResultsFolderLink, DataSizeKb, BinaryFilesCount);
    		string arg = grabbedPageMetaInformationDataEntry.SerializeToJSON();
    		try
    		{
    			if (!flag)
    			{
    				GrabberLink.CommonMetaDataSavingMutex.WaitOne();
    			}
    			FileStream fileStream = new FileStream(path, FileMode.Open);
    			StreamWriter streamWriter = new StreamWriter(fileStream);
    			streamWriter.BaseStream.Seek(-1L, SeekOrigin.End);
    			streamWriter.Write(string.Format("{0}{1}]", flag ? string.Empty : ",\r\n", arg));
    			streamWriter.Close();
    			streamWriter.Dispose();
    			fileStream.Close();
    			fileStream.Dispose();
    		}
    		catch (Exception occuredException2)
    		{
    			GrabberLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.GrabbingServer, "[GSE4] Error during meta file writing", occuredException2);
    		}
    		finally
    		{
    			GrabberLink.CommonMetaDataSavingMutex.ReleaseMutex();
    		}
    	}

    	/// <summary>
    	/// Returns actual meta-information set of grabbed pages
    	/// </summary>
    	/// <returns>List of meta information about grabbed pages</returns>
    	public List<GrabbedPageMetaInformationDataEntry> GetActualGrabbedPagesMeta()
    	{
    		string text = string.Format("{0}/{1}", GrabberLink.ParentTaskOperatingDirectory, "grabbed-data");
    		if (!Directory.Exists(text))
    		{
    			Directory.CreateDirectory(text);
    		}
    		string path = string.Format("{0}/{1}", text, "meta-logs.json");
    		if (!File.Exists(path))
    		{
    			return new List<GrabbedPageMetaInformationDataEntry>();
    		}
    		GrabberLink.CommonMetaDataSavingMutex.WaitOne();
    		string jSONData = File.ReadAllText(path);
    		GrabberLink.CommonMetaDataSavingMutex.ReleaseMutex();
    		GrabbedPageMetaInformationDataEntry grabbedPageMetaInformationDataEntry = new GrabbedPageMetaInformationDataEntry();
    		List<GrabbedPageMetaInformationDataEntry> source = grabbedPageMetaInformationDataEntry.UnserializeListFromJSON(jSONData);
    		return source.Where((GrabbedPageMetaInformationDataEntry item) => item != null).ToList();
    	}

    	/// <summary>
    	/// Deletes some grabbed data from HDD
    	/// </summary>
    	/// <param name="ParentMetaPointer"></param>
    	public void DeleteSpecifiedGrabbedDataEntries(List<GrabbedPageMetaInformationDataEntry> ParentDataPointers, Action<ActionProcessingContainer> ProcessingCounter = null)
    	{
    		string text = string.Format("{0}/{1}", GrabberLink.ParentTaskOperatingDirectory, "grabbed-data");
    		if (!Directory.Exists(text))
    		{
    			Directory.CreateDirectory(text);
    		}
    		string path = string.Format("{0}/{1}", text, "meta-logs.json");
    		if (!File.Exists(path))
    		{
    			return;
    		}
    		GrabberLink.CommonMetaDataSavingMutex.WaitOne();
    		string jSONData = File.ReadAllText(path);
    		GrabbedPageMetaInformationDataEntry grabbedPageMetaInformationDataEntry = new GrabbedPageMetaInformationDataEntry();
    		List<GrabbedPageMetaInformationDataEntry> list = grabbedPageMetaInformationDataEntry.UnserializeListFromJSON(jSONData);
    		int i;
    		for (i = 0; i < ParentDataPointers.Count; i++)
    		{
    			GrabbedPageMetaInformationDataEntry grabbedPageMetaInformationDataEntry2 = list.Where((GrabbedPageMetaInformationDataEntry item) => item.GrabbedPageUrl == ParentDataPointers[i].GrabbedPageUrl).FirstOrDefault();
    			if (grabbedPageMetaInformationDataEntry2 != null)
    			{
    				list.Remove(grabbedPageMetaInformationDataEntry2);
    				try
    				{
    					Directory.Delete($"{text}/{grabbedPageMetaInformationDataEntry2.ResultsFolderLink}", recursive: true);
    				}
    				catch (Exception occuredException)
    				{
    					GrabberLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.GrabbingServer, "[GSE7] Error during data deleting", occuredException);
    				}
    				ProcessingCounter?.Invoke(new ActionProcessingContainer(i + 1, ParentDataPointers.Count, $"{DateTime.Now.ToString()}: results from page = {grabbedPageMetaInformationDataEntry2.GrabbedPageUrl} deleted"));
    			}
    		}
    		string contents = JsonConvert.SerializeObject((object)list);
    		try
    		{
    			File.WriteAllText(path, contents);
    		}
    		catch (Exception occuredException2)
    		{
    			GrabberLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.GrabbingServer, "[GSE8] Error during meta-files swapping", occuredException2);
    		}
    		GrabberLink.CommonMetaDataSavingMutex.ReleaseMutex();
    	}
    }
}
