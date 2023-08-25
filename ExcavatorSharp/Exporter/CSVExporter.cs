// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Exporter.CSVExporter
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using ExcavatorSharp.Excavator;
using ExcavatorSharp.ExtensionMethods;
using ExcavatorSharp.Grabber;
using ExcavatorSharp.Interfaces;
using ExcavatorSharp.Objects;

namespace ExcavatorSharp.Exporter
{
    /// <summary>
    /// Exporter for saving data into csv file.
    /// </summary>
    internal class CSVExporter : IDataContextualExportable
    {
    	/// <summary>
    	/// Separator for CSV columns
    	/// </summary>
    	public static string CSVColumnsSeparator = ";";

    	/// <summary>
    	/// List for associating data and column coordinates
    	/// </summary>
    	private List<TableDataColumnLocation> DataLocationMap = new List<TableDataColumnLocation>();

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
    	/// Link to the task logger
    	/// </summary>
    	private DataExcavatorTasksLogger TaskLoggerLink { get; set; }

    	/// <summary>
    	/// Project patterns for all time, since project created.
    	/// </summary>
    	private List<DataGrabbingPattern> ProjectPatternsList { get; set; }

    	/// <summary>
    	/// Buffer for storing data before HDD IO
    	/// </summary>
    	private Dictionary<CSVExportingFilePtr, DataTable> ExportingDataBuffer { get; set; }

    	/// <summary>
    	/// Initializes a new insance of data exporter
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
    		ExportingDataBuffer = ExportingDataBuffer;
    		this.TaskLoggerLink = TaskLoggerLink;
    		InitializeDataBuffer();
    	}

    	/// <summary>
    	/// Initializes DataTable buffer
    	/// </summary>
    	private void InitializeDataBuffer()
    	{
    		ExportingDataBuffer = new Dictionary<CSVExportingFilePtr, DataTable>();
    		int num = 0;
    		foreach (DataGrabbingPattern projectPatterns in ProjectPatternsList)
    		{
    			int num2 = num + 1;
    			num++;
    			string stringLink = $"{num2}-{projectPatterns.PatternName}";
    			stringLink = stringLink.ReplaceMany(new string[7] { "[", "]", ":", "*", "?", "/", "\\" }, string.Empty);
    			if (stringLink.Length > 32)
    			{
    				stringLink = stringLink.Substring(0, 32);
    			}
    			CSVExportingFilePtr key = new CSVExportingFilePtr(num2, stringLink);
    			DataTable dataTable = new DataTable();
    			ExportingDataBuffer.Add(key, dataTable);
    			int num3 = 1;
    			DataLocationMap.Add(new TableDataColumnLocation(projectPatterns.GetHashCode(), "PageURL", num2, num3));
    			dataTable.Columns.Add("PageURL", typeof(string));
    			num3++;
    			DataLocationMap.Add(new TableDataColumnLocation(projectPatterns.GetHashCode(), "GrabDateTime", num2, num3));
    			dataTable.Columns.Add("GrabDateTime", typeof(string));
    			num3++;
    			foreach (DataGrabbingPatternItem grabbingItemsPattern in projectPatterns.GrabbingItemsPatterns)
    			{
    				string columnName = $"{grabbingItemsPattern.ElementName}-Data";
    				dataTable.Columns.Add(columnName, typeof(string));
    				DataLocationMap.Add(new TableDataColumnLocation(projectPatterns.GetHashCode(), columnName, num2, num3));
    				num3++;
    				if (grabbingItemsPattern.ParseBinaryAttributes)
    				{
    					for (int i = 0; i < grabbingItemsPattern.ParsingBinaryAttributes.Length; i++)
    					{
    						string columnName2 = $"{grabbingItemsPattern.ElementName}-{grabbingItemsPattern.ParsingBinaryAttributes[i].AttributeName}-AttrValue";
    						dataTable.Columns.Add(columnName2, typeof(string));
    						DataLocationMap.Add(new TableDataColumnLocation(projectPatterns.GetHashCode(), columnName2, num2, num3));
    						num3++;
    						string columnName3 = $"{grabbingItemsPattern.ElementName}-{grabbingItemsPattern.ParsingBinaryAttributes[i].AttributeName}-AttrFileName";
    						dataTable.Columns.Add(columnName3, typeof(string));
    						DataLocationMap.Add(new TableDataColumnLocation(projectPatterns.GetHashCode(), columnName3, num2, num3));
    						num3++;
    					}
    				}
    			}
    		}
    	}

    	/// <summary>
    	/// Adds data to exporting file
    	/// </summary>
    	/// <param name="ExportingData">Block of data to export</param>
    	public void AddDataIntoExportingFile(List<GrabbedDataGroup> ExportingData)
    	{
    		for (int i = 0; i < ExportingData.Count; i++)
    		{
    			int sheetIndexByPatternHash = TableDataColumnLocation.GetSheetIndexByPatternHash(ExportingData[i].PatternHash, DataLocationMap);
    			CSVExportingFilePtr key = null;
    			foreach (KeyValuePair<CSVExportingFilePtr, DataTable> item in ExportingDataBuffer)
    			{
    				if (item.Key.FileIndex == sheetIndexByPatternHash)
    				{
    					key = item.Key;
    					break;
    				}
    			}
    			foreach (List<GroupedDataItem> grabbingResult in ExportingData[i].GrabbingResults)
    			{
    				DataTable dataTable = ExportingDataBuffer[key];
    				DataRow dataRow = dataTable.NewRow();
    				dataTable.Rows.Add(dataRow);
    				dataRow["PageURL"] = ExportingData[i].GrabbedPageUrl.Trim();
    				dataRow["GrabDateTime"] = ExportingData[i].PageGrabbedDateTime.ToString().Trim();
    				foreach (GroupedDataItem item2 in grabbingResult)
    				{
    					string columnName = $"{item2.DataGrabbingPatternItemElementName}-Data";
    					GroupedDataItemDescendant[] grabbedItemsData = item2.GrabbedItemsData;
    					foreach (GroupedDataItemDescendant groupedDataItemDescendant in grabbedItemsData)
    					{
    						object obj = dataRow[columnName];
    						string inputData = string.Empty;
    						if (FoundedDataExportingType == DataExportingType.InnerText)
    						{
    							inputData = groupedDataItemDescendant.ElementInnerText.Trim();
    						}
    						else if (FoundedDataExportingType == DataExportingType.OuterHTML)
    						{
    							inputData = groupedDataItemDescendant.ElementOuterHtml.Trim();
    						}
    						if (obj == null || obj.ToString() == string.Empty)
    						{
    							dataRow[columnName] = NormalizeCellValueForCSVFile(inputData);
    						}
    						else
    						{
    							dataRow[columnName] = $"{dataRow[columnName].ToString()}{ItemsSequencesSeparator}{NormalizeCellValueForCSVFile(inputData)}";
    						}
    						if (groupedDataItemDescendant.ElementAttributes.Length == 0)
    						{
    							continue;
    						}
    						DescendantAttributeData[] elementAttributes = groupedDataItemDescendant.ElementAttributes;
    						foreach (DescendantAttributeData descendantAttributeData in elementAttributes)
    						{
    							string text = $"{item2.DataGrabbingPatternItemElementName}-{descendantAttributeData.AttributeName}-AttrValue";
    							string text2 = $"{item2.DataGrabbingPatternItemElementName}-{descendantAttributeData.AttributeName}-AttrFileName";
    							object obj2 = null;
    							if (dataTable.Columns.Contains(text))
    							{
    								obj2 = dataRow[text];
    								if (obj2 == null || obj2.ToString().Length == 0)
    								{
    									dataRow[text] = NormalizeCellValueForCSVFile(descendantAttributeData.AttributeValue.Trim());
    								}
    								else
    								{
    									dataRow[text] = $"{dataRow[text].ToString()}{ItemsSequencesSeparator}{NormalizeCellValueForCSVFile(descendantAttributeData.AttributeValue.Trim())}";
    								}
    							}
    							if (dataTable.Columns.Contains(text2))
    							{
    								object obj3 = dataRow[text2];
    								if (obj2 == null || obj2.ToString().Length == 0)
    								{
    									dataRow[text2] = NormalizeCellValueForCSVFile(descendantAttributeData.AttributeSavedFileName.Trim());
    								}
    								else
    								{
    									dataRow[text2] = $"{dataRow[text2].ToString()}{ItemsSequencesSeparator}{NormalizeCellValueForCSVFile(descendantAttributeData.AttributeSavedFileName.Trim())}";
    								}
    							}
    						}
    					}
    				}
    			}
    		}
    	}

    	/// <summary>
    	/// Finishes export
    	/// </summary>
    	public void FinishExport()
    	{
    		int num = 0;
    		foreach (KeyValuePair<CSVExportingFilePtr, DataTable> item in ExportingDataBuffer)
    		{
    			string path = string.Format(FilePath, num++);
    			StringBuilder stringBuilder = new StringBuilder();
    			for (int i = 0; i < item.Value.Columns.Count; i++)
    			{
    				stringBuilder.Append('"').Append(item.Value.Columns[i].ColumnName).Append('"');
    				if (i + 1 < item.Value.Columns.Count)
    				{
    					stringBuilder.Append(CSVColumnsSeparator);
    				}
    			}
    			stringBuilder.Append("\r\n");
    			File.WriteAllText(path, stringBuilder.ToString(), Encoding.UTF8);
    			stringBuilder.Clear();
    			int num2 = 0;
    			foreach (DataRow row in item.Value.Rows)
    			{
    				for (int j = 0; j < item.Value.Columns.Count; j++)
    				{
    					stringBuilder.Append('"').Append(row[item.Value.Columns[j]].ToString()).Append('"');
    					if (j + 1 < item.Value.Columns.Count)
    					{
    						stringBuilder.Append(CSVColumnsSeparator);
    					}
    				}
    				num2++;
    				if (num2 < item.Value.Rows.Count)
    				{
    					stringBuilder.Append("\r\n");
    				}
    				File.AppendAllText(path, stringBuilder.ToString());
    				stringBuilder.Clear();
    			}
    		}
    	}

    	/// <summary>
    	/// Normalizes values before saving in CSV format
    	/// </summary>
    	/// <param name="InputData">Original value</param>
    	/// <returns>Normalized value</returns>
    	private string NormalizeCellValueForCSVFile(string InputData)
    	{
    		return InputData.ReplaceMany(new string[5]
    		{
    			"\r\n",
    			"\r",
    			"\n",
    			$"\"{CSVColumnsSeparator}",
    			CSVColumnsSeparator
    		}, string.Empty);
    	}
    }
}
