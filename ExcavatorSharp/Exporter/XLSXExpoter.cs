// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Exporter.XLSXExpoter
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ExcavatorSharp.Excavator;
using ExcavatorSharp.ExtensionMethods;
using ExcavatorSharp.Grabber;
using ExcavatorSharp.Interfaces;
using ExcavatorSharp.Objects;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ExcavatorSharp.Exporter
{
    /// <summary>
    /// Exporter for saving data into Excel file. Uses EPPlus for saving data.
    /// </summary>
    internal class XLSXExpoter : IDataContextualExportable
    {
    	/// <summary>
    	/// List for associating data and column coordinates
    	/// </summary>
    	private List<TableDataColumnLocation> DataLocationMap = new List<TableDataColumnLocation>();

    	/// <summary>
    	/// Counters for real completed rows in each sheet { SheetIndex =&gt; RowPtr }
    	/// </summary>
    	private Dictionary<int, int> SheetsRowsCounters = new Dictionary<int, int>();

    	/// <summary>
    	/// Counters for real completed columns in each sheet { SheetIndex =&gt; ColumnPtr }
    	/// </summary>
    	private Dictionary<int, int> SheetsColumnsCounters = new Dictionary<int, int>();

    	/// <summary>
    	/// Column size, width
    	/// </summary>
    	public const int ColumnWith = 30;

    	/// <summary>
    	/// Data font size
    	/// </summary>
    	public const int FontSize = 12;

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
    	/// Object for storing data in Excel
    	/// </summary>
    	private ExcelPackage ExcelFileLink { get; set; }

    	/// <summary>
    	/// Project patterns for all time, since project created.
    	/// </summary>
    	private List<DataGrabbingPattern> ProjectPatternsList { get; set; }

    	/// <summary>
    	/// Link to the task logger
    	/// </summary>
    	private DataExcavatorTasksLogger TaskLoggerLink { get; set; }

    	/// <summary>
    	/// Initializes exporter
    	/// </summary>
    	/// <param name="FileSavingPath">Path to exporting file</param>
    	/// <param name="FoundedDataExportingType">Type of exporting data</param>
    	/// <param name="ItemsSequencesSeparator">Separator for separating data sequences, if some data includes many items</param>
    	/// <param name="ProjectPatternsList">All project patterns list from first grabbing</param>
    	public void InitializeExpoter(string FileSavingPath, DataExportingType FoundedDataExportingType, string ItemsSequencesSeparator, List<DataGrabbingPattern> ProjectPatternsList, DataExcavatorTasksLogger TaskLoggerLink)
    	{
    		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
    		//IL_0053: Expected O, but got Unknown
    		FilePath = FileSavingPath;
    		this.FoundedDataExportingType = FoundedDataExportingType;
    		this.ItemsSequencesSeparator = ItemsSequencesSeparator;
    		this.TaskLoggerLink = TaskLoggerLink;
    		if (File.Exists(FilePath))
    		{
    			File.Delete(FilePath);
    		}
    		ExcelFileLink = new ExcelPackage(new FileInfo(FilePath));
    		this.ProjectPatternsList = ProjectPatternsList;
    		InitializeFileHeader();
    	}

    	/// <summary>
    	/// File initialization
    	/// </summary>
    	private void InitializeFileHeader()
    	{
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
    			ExcelFileLink.Workbook.Worksheets.Add(stringLink);
    			ExcelWorksheet val = ((IEnumerable<ExcelWorksheet>)ExcelFileLink.Workbook.Worksheets).Last();
    			SheetsRowsCounters.Add(num2, 1);
    			int num3 = 1;
    			DataLocationMap.Add(new TableDataColumnLocation(projectPatterns.GetHashCode(), "PageURL", num2, num3));
    			val.Column(num3).Width = 30.0;
    			SheetsColumnsCounters.Add(num2, 1);
    			((ExcelRangeBase)val.Cells[1, num3++]).Value = "PageURL";
    			DataLocationMap.Add(new TableDataColumnLocation(projectPatterns.GetHashCode(), "GrabDateTime", num2, num3));
    			val.Column(num3).Width = 30.0;
    			SheetsColumnsCounters[num2]++;
    			((ExcelRangeBase)val.Cells[1, num3++]).Value = "GrabDateTime";
    			foreach (DataGrabbingPatternItem grabbingItemsPattern in projectPatterns.GrabbingItemsPatterns)
    			{
    				string text = $"{grabbingItemsPattern.ElementName}-Data";
    				((ExcelRangeBase)val.Cells[1, num3]).Value = text;
    				DataLocationMap.Add(new TableDataColumnLocation(projectPatterns.GetHashCode(), text, num2, num3));
    				val.Column(num3).Width = 30.0;
    				SheetsColumnsCounters[num2]++;
    				num3++;
    				if (grabbingItemsPattern.ParseBinaryAttributes)
    				{
    					for (int i = 0; i < grabbingItemsPattern.ParsingBinaryAttributes.Length; i++)
    					{
    						string text2 = $"{grabbingItemsPattern.ElementName}-{grabbingItemsPattern.ParsingBinaryAttributes[i].AttributeName}-AttrValue";
    						((ExcelRangeBase)val.Cells[1, num3]).Value = text2;
    						DataLocationMap.Add(new TableDataColumnLocation(projectPatterns.GetHashCode(), text2, num2, num3));
    						val.Column(num3).Width = 30.0;
    						SheetsColumnsCounters[num2]++;
    						num3++;
    						string text3 = $"{grabbingItemsPattern.ElementName}-{grabbingItemsPattern.ParsingBinaryAttributes[i].AttributeName}-AttrFileName";
    						((ExcelRangeBase)val.Cells[1, num3]).Value = text3;
    						DataLocationMap.Add(new TableDataColumnLocation(projectPatterns.GetHashCode(), text3, num2, num3));
    						val.Column(num3).Width = 30.0;
    						SheetsColumnsCounters[num2]++;
    						num3++;
    					}
    				}
    			}
    		}
    	}

    	/// <summary>
    	/// Adds data row to excel file
    	/// </summary>
    	/// <param name="ExportingData"></param>
    	public void AddDataIntoExportingFile(List<GrabbedDataGroup> ExportingData)
    	{
    		for (int i = 0; i < ExportingData.Count; i++)
    		{
    			int sheetIndexByPatternHash = TableDataColumnLocation.GetSheetIndexByPatternHash(ExportingData[i].PatternHash, DataLocationMap);
    			if (sheetIndexByPatternHash == -1)
    			{
    				TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.DataExporter, $"[ERROR IN DATA EXPORTING] [DEEX1] Can't fund pattern by specified hash = {ExportingData[i].PatternHash}. Data skipped.");
    				continue;
    			}
    			ExcelWorksheet val = ExcelFileLink.Workbook.Worksheets[sheetIndexByPatternHash];
    			foreach (List<GroupedDataItem> grabbingResult in ExportingData[i].GrabbingResults)
    			{
    				SheetsRowsCounters[sheetIndexByPatternHash]++;
    				int num = SheetsRowsCounters[sheetIndexByPatternHash];
    				int columnIndexByItemDataName = TableDataColumnLocation.GetColumnIndexByItemDataName(ExportingData[i].PatternHash, "PageURL", DataLocationMap);
    				int columnIndexByItemDataName2 = TableDataColumnLocation.GetColumnIndexByItemDataName(ExportingData[i].PatternHash, "GrabDateTime", DataLocationMap);
    				((ExcelRangeBase)val.Cells[num, columnIndexByItemDataName]).Value = ExportingData[i].GrabbedPageUrl.Trim();
    				((ExcelRangeBase)val.Cells[num, columnIndexByItemDataName2]).Value = ExportingData[i].PageGrabbedDateTime.ToString().Trim();
    				foreach (GroupedDataItem item in grabbingResult)
    				{
    					string dataName = $"{item.DataGrabbingPatternItemElementName}-Data";
    					int columnIndexByItemDataName3 = TableDataColumnLocation.GetColumnIndexByItemDataName(ExportingData[i].PatternHash, dataName, DataLocationMap);
    					GroupedDataItemDescendant[] grabbedItemsData = item.GrabbedItemsData;
    					foreach (GroupedDataItemDescendant groupedDataItemDescendant in grabbedItemsData)
    					{
    						object value = ((ExcelRangeBase)val.Cells[num, columnIndexByItemDataName3]).Value;
    						string text = string.Empty;
    						if (FoundedDataExportingType == DataExportingType.InnerText)
    						{
    							text = groupedDataItemDescendant.ElementInnerText.Trim();
    						}
    						else if (FoundedDataExportingType == DataExportingType.OuterHTML)
    						{
    							text = groupedDataItemDescendant.ElementOuterHtml.Trim();
    						}
    						if (value == null || value.ToString() == string.Empty)
    						{
    							((ExcelRangeBase)val.Cells[num, columnIndexByItemDataName3]).Value = text;
    						}
    						else
    						{
    							((ExcelRangeBase)val.Cells[num, columnIndexByItemDataName3]).Value = $"{((ExcelRangeBase)val.Cells[num, columnIndexByItemDataName3]).Value}{ItemsSequencesSeparator}{text}";
    						}
    						if (groupedDataItemDescendant.ElementAttributes.Length == 0)
    						{
    							continue;
    						}
    						DescendantAttributeData[] elementAttributes = groupedDataItemDescendant.ElementAttributes;
    						foreach (DescendantAttributeData descendantAttributeData in elementAttributes)
    						{
    							string dataName2 = $"{item.DataGrabbingPatternItemElementName}-{descendantAttributeData.AttributeName}-AttrValue";
    							string dataName3 = $"{item.DataGrabbingPatternItemElementName}-{descendantAttributeData.AttributeName}-AttrFileName";
    							int columnIndexByItemDataName4 = TableDataColumnLocation.GetColumnIndexByItemDataName(ExportingData[i].PatternHash, dataName2, DataLocationMap);
    							int columnIndexByItemDataName5 = TableDataColumnLocation.GetColumnIndexByItemDataName(ExportingData[i].PatternHash, dataName3, DataLocationMap);
    							object obj = null;
    							if (columnIndexByItemDataName4 != -1)
    							{
    								obj = ((ExcelRangeBase)val.Cells[num, columnIndexByItemDataName4]).Value;
    								if (obj == null || obj.ToString().Length == 0)
    								{
    									((ExcelRangeBase)val.Cells[num, columnIndexByItemDataName4]).Value = descendantAttributeData.AttributeValue.Trim();
    								}
    								else
    								{
    									((ExcelRangeBase)val.Cells[num, columnIndexByItemDataName4]).Value = $"{((ExcelRangeBase)val.Cells[num, columnIndexByItemDataName4]).Value.ToString()}{ItemsSequencesSeparator}{descendantAttributeData.AttributeValue.Trim()}";
    								}
    							}
    							if (columnIndexByItemDataName5 != -1)
    							{
    								object value2 = ((ExcelRangeBase)val.Cells[num, columnIndexByItemDataName5]).Value;
    								if (obj == null || obj.ToString().Length == 0)
    								{
    									((ExcelRangeBase)val.Cells[num, columnIndexByItemDataName5]).Value = descendantAttributeData.AttributeSavedFileName.Trim();
    								}
    								else
    								{
    									((ExcelRangeBase)val.Cells[num, columnIndexByItemDataName5]).Value = $"{((ExcelRangeBase)val.Cells[num, columnIndexByItemDataName5]).Value.ToString()}{ItemsSequencesSeparator}{descendantAttributeData.AttributeSavedFileName.Trim()}";
    								}
    							}
    						}
    					}
    				}
    			}
    		}
    	}

    	/// <summary>
    	/// Close file and finish export
    	/// </summary>
    	public void FinishExport()
    	{
    		for (int i = 0; i < ExcelFileLink.Workbook.Worksheets.Count; i++)
    		{
    			ExcelWorksheet val = ExcelFileLink.Workbook.Worksheets[i + 1];
    			int num = SheetsRowsCounters[i + 1];
    			string address = ((ExcelAddress)val.Cells[1, SheetsColumnsCounters[i + 1]]).Address;
    			string arg = address.SkipNumbersAndPunctuation();
    			((ExcelRangeBase)val.Cells[$"A1:{arg}{num}"]).Style.Numberformat.Format = "@";
    			((ExcelRangeBase)val.Cells[$"A1:{arg}{num}"]).Style.Font.SetFromFont(new Font("Arial", 12f));
    			((ExcelRangeBase)val.Cells[$"A1:{arg}{num}"]).Style.Font.Size = 12f;
    			((ExcelRangeBase)val.Cells[$"A1:{arg}{num}"]).Style.HorizontalAlignment = (ExcelHorizontalAlignment)1;
    			((ExcelRangeBase)val.Cells[$"A1:{arg}{num}"]).Style.VerticalAlignment = (ExcelVerticalAlignment)0;
    		}
    		ExcelFileLink.Save();
    		ExcelFileLink.Dispose();
    	}
    }
}
