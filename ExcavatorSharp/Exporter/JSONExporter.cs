// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Exporter.JSONExporter
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using ExcavatorSharp.Excavator;
using ExcavatorSharp.Grabber;
using ExcavatorSharp.Interfaces;
using ExcavatorSharp.Objects;
using Newtonsoft.Json;

namespace ExcavatorSharp.Exporter
{
    /// <summary>
    /// Exporting for saving data into JSON file
    /// </summary>
    internal class JSONExporter : IDataContextualExportable
    {
    	/// <summary>
    	/// Number of writed objects
    	/// </summary>
    	private int WritedObjectsCounter = 0;

    	/// <summary>
    	/// Link to saving file
    	/// </summary>
    	private string FilePath { get; set; }

    	/// <summary>
    	/// Type of exporting data
    	/// </summary>
    	private DataExportingType FoundedDataExportingType { get; set; }

    	/// <summary>
    	/// Separator for separating many elements, founded by one PatternItemSelector
    	/// </summary>
    	private string ItemsSequencesSeparator { get; set; }

    	/// <summary>
    	/// Project patterns for all time, since project created.
    	/// </summary>
    	private List<DataGrabbingPattern> ProjectPatternsList { get; set; }

    	/// <summary>
    	/// Link to the task logger
    	/// </summary>
    	private DataExcavatorTasksLogger TaskLoggerLink { get; set; }

    	/// <summary>
    	/// Adds data to exporting file
    	/// </summary>
    	/// <param name="ExportingData"></param>
    	public void AddDataIntoExportingFile(List<GrabbedDataGroup> ExportingData)
    	{
    		List<object> list = PackExportingDataIntoJSONPreparedObjects(ExportingData);
    		foreach (dynamic item in list)
    		{
    			string text = JsonConvert.SerializeObject(item, (Formatting)1);
    			File.AppendAllText(FilePath, (WritedObjectsCounter > 0) ? $",\r\n{text}" : text);
    			WritedObjectsCounter++;
    		}
    	}

    	/// <summary>
    	/// Packs data to JSON format
    	/// </summary>
    	/// <param name="ExportingData">List of data to export</param>
    	/// <returns>Data, packed to JSON-prepared containers</returns>
    	public List<dynamic> PackExportingDataIntoJSONPreparedObjects(List<GrabbedDataGroup> ExportingData)
    	{
    		List<object> list = new List<object>();
    		foreach (GrabbedDataGroup ExportingDatum in ExportingData)
    		{
    			IDictionary<string, object> dictionary = new ExpandoObject();
    			dictionary.Add("PageURL", ExportingDatum.GrabbedPageUrl);
    			dictionary.Add("GrabDateTime", ExportingDatum.PageGrabbedDateTime.ToString());
    			dictionary.Add("PatternName", ExportingDatum.PatternName);
    			List<List<Dictionary<string, object>>> list2 = new List<List<Dictionary<string, object>>>();
    			foreach (List<GroupedDataItem> grabbingResult in ExportingDatum.GrabbingResults)
    			{
    				List<Dictionary<string, object>> list3 = new List<Dictionary<string, object>>();
    				foreach (GroupedDataItem item in grabbingResult)
    				{
    					List<Dictionary<string, object>> list4 = new List<Dictionary<string, object>>();
    					GroupedDataItemDescendant[] grabbedItemsData = item.GrabbedItemsData;
    					foreach (GroupedDataItemDescendant groupedDataItemDescendant in grabbedItemsData)
    					{
    						List<object> list5 = new List<object>();
    						DescendantAttributeData[] elementAttributes = groupedDataItemDescendant.ElementAttributes;
    						foreach (DescendantAttributeData descendantAttributeData in elementAttributes)
    						{
    							list5.Add(new Dictionary<string, string>
    							{
    								{
    									"{0}-{1}-AttrName".Replace("{0}-{1}-", string.Empty),
    									descendantAttributeData.AttributeName
    								},
    								{
    									"{0}-{1}-AttrValue".Replace("{0}-{1}-", string.Empty),
    									descendantAttributeData.AttributeValue
    								},
    								{
    									"{0}-{1}-AttrFileName".Replace("{0}-{1}-", string.Empty),
    									descendantAttributeData.AttributeSavedFileName
    								}
    							});
    						}
    						list4.Add(new Dictionary<string, object>
    						{
    							{
    								"GrabbedData",
    								(FoundedDataExportingType == DataExportingType.InnerText) ? groupedDataItemDescendant.ElementInnerText : groupedDataItemDescendant.ElementOuterHtml
    							},
    							{ "Attributes", list5 }
    						});
    					}
    					list3.Add(new Dictionary<string, object>
    					{
    						{ "PatternItemName", item.DataGrabbingPatternItemElementName },
    						{
    							"{0}-Data".Replace("{0}-", string.Empty),
    							list4
    						}
    					});
    				}
    				list2.Add(list3);
    			}
    			dictionary.Add("GrabbedData", list2);
    			list.Add(dictionary);
    		}
    		return list;
    	}

    	/// <summary>
    	/// Close file and finish export
    	/// </summary>
    	public void FinishExport()
    	{
    		File.AppendAllText(FilePath, "\r\n]");
    	}

    	/// <summary>
    	/// Initializes exporter
    	/// </summary>
    	/// <param name="FileSavingPath">Path to exporting file</param>
    	/// <param name="FoundedDataExportingType">Type of exporting data</param>
    	/// <param name="ItemsSequencesSeparator">Separator for separating data sequences, if some data includes many items</param>
    	/// <param name="ProjectPatternsList">All project patterns list from first grabbing</param>
    	public void InitializeExpoter(string FileSavingPath, DataExportingType FoundedDataExportingType, string ItemsSequencesSeparator, List<DataGrabbingPattern> ProjectPatternsList, DataExcavatorTasksLogger TaskLoggerLink)
    	{
    		FilePath = FileSavingPath;
    		this.FoundedDataExportingType = FoundedDataExportingType;
    		this.ItemsSequencesSeparator = ItemsSequencesSeparator;
    		this.ProjectPatternsList = ProjectPatternsList;
    		this.TaskLoggerLink = TaskLoggerLink;
    		if (!(FileSavingPath != string.Empty))
    		{
    			return;
    		}
    		try
    		{
    			if (File.Exists(FilePath))
    			{
    				File.Delete(FilePath);
    			}
    			File.WriteAllText(FilePath, "[\r\n");
    		}
    		catch (Exception occuredException)
    		{
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.DataExporter, $"Can't initialize JSON exporter - can't write data to file {FilePath}", occuredException);
    		}
    	}
    }
}
