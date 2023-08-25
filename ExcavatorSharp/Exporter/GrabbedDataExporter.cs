// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Exporter.GrabbedDataExporter
using System;
using System.Collections.Generic;
using System.IO;
using ExcavatorSharp.Excavator;
using ExcavatorSharp.Grabber;
using ExcavatorSharp.Interfaces;
using ExcavatorSharp.Objects;

namespace ExcavatorSharp.Exporter
{
    /// <summary>
    /// Class for data exporting to known formats - csv, xls, json (and other)
    /// </summary>
    public class GrabbedDataExporter
    {
    	/// <summary>
    	/// Exports selected grabbed data in a simplifyed format with
    	/// </summary>
    	/// <param name="TaskPath">Task of the path</param>
    	/// <param name="ExportingFormat">Exporting format - file type to save assembled data</param>
    	/// <param name="DirectoryToExportData">Target directory for exporting data</param>
    	/// <param name="ExportingType">Data exporting type</param>
    	/// <param name="ItemsSequencesSeparator">Separator for separating items sequences.</param>
    	/// <param name="TaskLoggerLink">Link to task logger</param>
    	/// <param name="SelectedEntriesToExport">Selected entries to export</param>
    	public void ExportSelectedGrabbedData(string TaskPath, string DirectoryToExportData, DataExcavatorTasksLogger TaskLoggerLink, DataExportingFormat ExportingFormat, DataExportingType ExportingType, string ItemsSequencesSeparator, List<GrabbedPageMetaInformationDataEntry> SelectedEntriesToExport, Action<DataExportingProcessStat> DataExportingProcessCallback = null)
    	{
    		TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.DataExporter, $"Data exporting started. Exporting format = {ExportingFormat.ToString()}, Exporting type = {ExportingType.ToString()}, Count of selected entries to export = {SelectedEntriesToExport.Count}");
    		string text = string.Format("{0}/{1}", TaskPath, "grabbed-data");
    		if (!Directory.Exists(text))
    		{
    			return;
    		}
    		string path = string.Format("{0}/{1}", text, "meta-logs.json");
    		if (!File.Exists(path) || !Directory.Exists(DirectoryToExportData))
    		{
    			return;
    		}
    		IDataContextualExportable dataExporter = GetDataExporter(TaskLoggerLink, text, DirectoryToExportData, ExportingFormat, ExportingType, ItemsSequencesSeparator);
    		string text2 = $"{DirectoryToExportData}/binary-files";
    		try
    		{
    			if (!Directory.Exists(text2))
    			{
    				Directory.CreateDirectory(text2);
    			}
    		}
    		catch (Exception occuredException)
    		{
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.DataExporter, $"Error during export initialization. Can't create directory {text2}", occuredException);
    		}
    		GrabbedDataGroup grabbedDataGroup = new GrabbedDataGroup();
    		int num = 0;
    		int num2 = 0;
    		int num3 = 0;
    		int num4 = 0;
    		double num5 = SelectedEntriesToExport.Count;
    		foreach (GrabbedPageMetaInformationDataEntry item in SelectedEntriesToExport)
    		{
    			string text3 = string.Format("{0}/{1}/{2}", TaskPath, "grabbed-data", item.ResultsFolderLink);
    			if (!item.HasResults || !Directory.Exists(text3))
    			{
    				continue;
    			}
    			string text4 = string.Format("{0}/{1}", text3, "parsed-data.json");
    			string path2 = string.Format("{0}/{1}", text3, "binary-files");
    			string[] array = null;
    			if (Directory.Exists(path2))
    			{
    				array = Directory.GetFiles(path2);
    			}
    			if (array != null)
    			{
    				string[] array2 = array;
    				foreach (string text5 in array2)
    				{
    					FileInfo fileInfo = new FileInfo(text5);
    					string text6 = $"{text2}/{fileInfo.Name}";
    					if (!File.Exists(text6))
    					{
    						File.Copy(text5, text6);
    					}
    					num3++;
    				}
    			}
    			bool flag = false;
    			string empty = string.Empty;
    			List<GrabbedDataGroup> exportingData = null;
    			try
    			{
    				empty = File.ReadAllText(text4);
    				exportingData = grabbedDataGroup.UnserializeListFromJSON(empty);
    				flag = true;
    			}
    			catch (Exception occuredException2)
    			{
    				TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.DataExporter, $"[DERR2] Can't read and unserialize data from next entry. Path={text4}", occuredException2);
    				num4++;
    				flag = false;
    			}
    			if (flag)
    			{
    				try
    				{
    					dataExporter.AddDataIntoExportingFile(exportingData);
    				}
    				catch (Exception occuredException3)
    				{
    					TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.DataExporter, "[DEER1] Data exporting error", occuredException3);
    					num4++;
    					flag = false;
    				}
    			}
    			num++;
    			if (flag)
    			{
    				num2++;
    			}
    			double exportCompletionPercentage = Math.Round((double)num / num5 * 100.0, 2);
    			DataExportingProcessCallback?.Invoke(new DataExportingProcessStat((int)num5, num, exportCompletionPercentage, num4));
    		}
    		dataExporter.FinishExport();
    		TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.DataExporter, $"Data exporting finished. Exported entities count = {num2}, binary files count = {num3}");
    	}

    	/// <summary>
    	/// Exports all grabbed data in a simplifyed format with filtering by two dates
    	/// </summary>
    	/// <param name="TaskPath">Task of the path</param>
    	/// <param name="ExportingFormat">Exporting format - file type to save assembled data</param>
    	/// <param name="ExportStartDate">Start date of parsed data. If null - will be used data from all periods</param>
    	/// <param name="ExportEndDate">End date of parsed data. If null - will be used data from all periods</param>
    	/// <param name="DirectoryToExportData">Target directory for exporting data</param>
    	/// <param name="ExportingType">Data exporting type</param>
    	/// <param name="ItemsSequencesSeparator">Separator for separating items sequences.</param>
    	/// <param name="TaskLoggerLink">Link to task logger</param>
    	public void ExportAllGrabbedData(string TaskPath, string DirectoryToExportData, DataExcavatorTasksLogger TaskLoggerLink, DataExportingFormat ExportingFormat, DataExportingType ExportingType, string ItemsSequencesSeparator, DateTime ExportStartDate, DateTime ExportEndDate, Action<DataExportingProcessStat> DataExportingProcessCallback = null)
    	{
    		TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.DataExporter, string.Format("Data exporting started. Exporting format = {0}, Exporting type = {1}, Start date = {2}, End date = {3}", ExportingFormat.ToString(), ExportingType.ToString(), ExportStartDate.ToString("dd.MM.yyyy HH:mm"), ExportEndDate.ToString("dd.MM.yyyy HH:mm")));
    		string text = string.Format("{0}/{1}", TaskPath, "grabbed-data");
    		if (!Directory.Exists(text))
    		{
    			return;
    		}
    		string path = string.Format("{0}/{1}", text, "meta-logs.json");
    		if (!File.Exists(path) || !Directory.Exists(DirectoryToExportData))
    		{
    			return;
    		}
    		List<DataGrabbingPattern> projectAllPatterns = GetProjectAllPatterns(text);
    		IDataContextualExportable dataExporter = GetDataExporter(TaskLoggerLink, text, DirectoryToExportData, ExportingFormat, ExportingType, ItemsSequencesSeparator);
    		string text2 = $"{DirectoryToExportData}/binary-files";
    		if (!Directory.Exists(text2))
    		{
    			Directory.CreateDirectory(text2);
    		}
    		GrabbedDataGroup grabbedDataGroup = new GrabbedDataGroup();
    		string jSONData = File.ReadAllText(path);
    		GrabbedPageMetaInformationDataEntry grabbedPageMetaInformationDataEntry = new GrabbedPageMetaInformationDataEntry();
    		List<GrabbedPageMetaInformationDataEntry> list = grabbedPageMetaInformationDataEntry.UnserializeListFromJSON(jSONData);
    		int num = 0;
    		int num2 = 0;
    		int num3 = 0;
    		int num4 = 0;
    		double num5 = list.Count;
    		foreach (GrabbedPageMetaInformationDataEntry item in list)
    		{
    			string text3 = string.Format("{0}/{1}/{2}", TaskPath, "grabbed-data", item.ResultsFolderLink);
    			if (!item.HasResults || !Directory.Exists(text3) || item.PageGrabbedDateTime < ExportStartDate || item.PageGrabbedDateTime > ExportEndDate)
    			{
    				continue;
    			}
    			string text4 = string.Format("{0}/{1}", text3, "parsed-data.json");
    			string path2 = string.Format("{0}/{1}", text3, "binary-files");
    			string[] array = null;
    			if (Directory.Exists(path2))
    			{
    				array = Directory.GetFiles(path2);
    			}
    			if (array != null)
    			{
    				string[] array2 = array;
    				foreach (string text5 in array2)
    				{
    					FileInfo fileInfo = new FileInfo(text5);
    					string text6 = $"{text2}/{fileInfo.Name}";
    					if (!File.Exists(text6))
    					{
    						File.Copy(text5, text6);
    					}
    					num3++;
    				}
    			}
    			bool flag = false;
    			string empty = string.Empty;
    			List<GrabbedDataGroup> exportingData = null;
    			try
    			{
    				empty = File.ReadAllText(text4);
    				exportingData = grabbedDataGroup.UnserializeListFromJSON(empty);
    				flag = true;
    			}
    			catch (Exception occuredException)
    			{
    				TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.DataExporter, $"[DERR2] Can't read and unserialize data from next entry. Path={text4}", occuredException);
    				num4++;
    				flag = false;
    			}
    			if (flag)
    			{
    				try
    				{
    					dataExporter.AddDataIntoExportingFile(exportingData);
    				}
    				catch (Exception occuredException2)
    				{
    					num4++;
    					TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.DataExporter, "[DEER2] Data exporting error", occuredException2);
    				}
    			}
    			num++;
    			if (flag)
    			{
    				num2++;
    			}
    			double exportCompletionPercentage = Math.Round((double)num / num5 * 100.0, 2);
    			DataExportingProcessCallback?.Invoke(new DataExportingProcessStat((int)num5, num, exportCompletionPercentage, num4));
    		}
    		dataExporter.FinishExport();
    		TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.DataExporter, $"Data exporting finished. Exported entities count = {num2}, binary files count = {num3}");
    	}

    	/// <summary>
    	/// Get specifies data exporter with actual used context
    	/// </summary>
    	/// <returns></returns>
    	private IDataContextualExportable GetDataExporter(DataExcavatorTasksLogger TaskLoggerLink, string GrabbedDataRootDirectory, string DirectoryToExportData, DataExportingFormat ExportingFormat, DataExportingType ExportingType, string ItemsSequencesSeparator)
    	{
    		List<DataGrabbingPattern> projectAllPatterns = GetProjectAllPatterns(GrabbedDataRootDirectory);
    		IDataContextualExportable dataContextualExportable = null;
    		string arg = string.Empty;
    		switch (ExportingFormat)
    		{
    		case DataExportingFormat.CSV:
    			arg = "{0}.csv";
    			dataContextualExportable = new CSVExporter();
    			break;
    		case DataExportingFormat.JSON:
    			arg = "json";
    			dataContextualExportable = new JSONExporter();
    			break;
    		case DataExportingFormat.XLSX:
    			arg = "xlsx";
    			dataContextualExportable = new XLSXExpoter();
    			break;
    		case DataExportingFormat.MySQL:
    			arg = "sql";
    			dataContextualExportable = new MySQLExporter();
    			break;
    		}
    		string fileSavingPath = $"{DirectoryToExportData}/exported-data.{arg}";
    		dataContextualExportable.InitializeExpoter(fileSavingPath, ExportingType, ItemsSequencesSeparator, projectAllPatterns, TaskLoggerLink);
    		return dataContextualExportable;
    	}

    	/// <summary>
    	/// Return some grabbed data group overview
    	/// </summary>
    	/// <param name="ParentMetaPointer"></param>
    	/// <returns></returns>
    	public List<GrabbedDataGroup> GetGrabbedDataEntryOverview(GrabbedPageMetaInformationDataEntry ParentMetaPointer)
    	{
    		GrabbedDataGroup grabbedDataGroup = new GrabbedDataGroup();
    		string resultsFolderLink = ParentMetaPointer.ResultsFolderLink;
    		string path = string.Format("{0}/{1}", resultsFolderLink, "parsed-data.json");
    		string text = string.Format("{0}/{1}", resultsFolderLink, "binary-files");
    		string jSONData = File.ReadAllText(path);
    		return grabbedDataGroup.UnserializeListFromJSON(jSONData);
    	}

    	/// <summary>
    	/// Returns a set of all GrabbingPatterns used in project since first data was grabbed
    	/// </summary>
    	/// <param name="GrabbingDataPath">Path to grabbied data directory</param>
    	/// <returns>A set of DataGrabbingPattern</returns>
    	private List<DataGrabbingPattern> GetProjectAllPatterns(string GrabbingDataPath)
    	{
    		List<DataGrabbingPattern> list = new List<DataGrabbingPattern>();
    		string path = string.Format("{0}/{1}", GrabbingDataPath, "data-patterns");
    		if (!Directory.Exists(path))
    		{
    			return list;
    		}
    		DataGrabbingPattern dataGrabbingPattern = new DataGrabbingPattern();
    		string[] files = Directory.GetFiles(path, $"pattern.{'*'}.json");
    		string[] array = files;
    		foreach (string path2 in array)
    		{
    			string jSONData = File.ReadAllText(path2);
    			DataGrabbingPattern item = dataGrabbingPattern.UnserializeFromJSON(jSONData);
    			list.Add(item);
    		}
    		return list;
    	}
    }
}
