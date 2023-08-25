// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Excavator.DataExcavatorTaskIO
using System;
using System.Collections.Generic;
using System.IO;
using ExcavatorSharp.Objects;
using Newtonsoft.Json;

namespace ExcavatorSharp.Excavator
{
    /// <summary>
    /// IO operations list for DETask's
    /// </summary>
    public class DataExcavatorTaskIO
    {
    	/// <summary>
    	/// Folder name with DE task data
    	/// </summary>
    	internal const string DETaskFolderName = "de-task-data";

    	/// <summary>
    	/// File name with task config data
    	/// </summary>
    	internal const string DETaskPropertiesFileName = "task-common-config.json";

    	/// <summary>
    	/// File name with crawler config data
    	/// </summary>
    	internal const string DECrawlerPropertiesFileName = "task-crawler-config.json";

    	/// <summary>
    	/// File name with grabber config data
    	/// </summary>
    	internal const string DEGrabberPropertiesFileName = "task-grabber-config.json";

    	/// <summary>
    	/// Pattern file name format -&gt; pattern.{0}.json
    	/// </summary>
    	internal static string DEActualPatternFileName => "pattern.{0}.json";

    	/// <summary>
    	/// Saves DE task actual config data - properties, patterns and other data
    	/// </summary>
    	/// <param name="TaskName">Task name</param>
    	/// <param name="TaskOperatingDirectory">Task operating directory</param>
    	/// <param name="CrawlerProperties">Crawling server properties</param>
    	/// <param name="GrabberProperties">Grabbing server properties</param>
    	/// <param name="WebsiteRootUrl">Website root address</param>
    	/// <param name="DataPatterns">List of used data patterns</param>
    	/// <param name="TaskDescription">Task description</param>
    	internal void SaveDETask(string TaskName, string TaskOperatingDirectory, string WebsiteRootUrl, string TaskDescription, CrawlingServerProperties CrawlerProperties, GrabbingServerProperties GrabberProperties, List<DataGrabbingPattern> DataPatterns)
    	{
    		string text = string.Format("{0}/{1}", TaskOperatingDirectory, "de-task-data");
    		if (!Directory.Exists(text))
    		{
    			Directory.CreateDirectory(text);
    		}
    		string path = string.Format("{0}/{1}", text, "task-common-config.json");
    		File.WriteAllText(path, JsonConvert.SerializeObject((object)new { TaskName, TaskOperatingDirectory, WebsiteRootUrl, TaskDescription }, (Formatting)1));
    		string path2 = string.Format("{0}/{1}", text, "task-crawler-config.json");
    		File.WriteAllText(path2, CrawlerProperties.SerializeToJSON());
    		string path3 = string.Format("{0}/{1}", text, "task-grabber-config.json");
    		File.WriteAllText(path3, GrabberProperties.SerializeToJSON());
    		string[] files = Directory.GetFiles(text, string.Format(DEActualPatternFileName, "*"));
    		string[] array = files;
    		foreach (string path4 in array)
    		{
    			File.Delete(path4);
    		}
    		foreach (DataGrabbingPattern DataPattern in DataPatterns)
    		{
    			string path5 = $"{text}/{string.Format(DEActualPatternFileName, DataPattern.GetHashCode())}";
    			File.WriteAllText(path5, DataPattern.SerializeToJSON());
    		}
    	}

    	/// <summary>
    	/// Creates new insance of DETask from task project path
    	/// </summary>
    	/// <param name="ProjectPath"></param>
    	/// <returns></returns>
    	internal DataExcavatorTask CreateTaskFromProjectPath(string TaskOperatingDirectory)
    	{
    		string text = string.Format("{0}/{1}", TaskOperatingDirectory, "de-task-data");
    		if (!Directory.Exists(text))
    		{
    			return null;
    		}
    		string path = string.Format("{0}/{1}", text, "task-common-config.json");
    		string text2 = File.ReadAllText(path);
    		Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(text2);
    		string path2 = string.Format("{0}/{1}", text, "task-crawler-config.json");
    		string jSONData = File.ReadAllText(path2);
    		CrawlingServerProperties crawlingServerProperties = new CrawlingServerProperties();
    		crawlingServerProperties = crawlingServerProperties.UnserializeFromJSON(jSONData);
    		string path3 = string.Format("{0}/{1}", text, "task-grabber-config.json");
    		string jSONData2 = File.ReadAllText(path3);
    		GrabbingServerProperties grabbingServerProperties = new GrabbingServerProperties();
    		grabbingServerProperties = grabbingServerProperties.UnserializeFromJSON(jSONData2);
    		string[] files = Directory.GetFiles(text, string.Format(DEActualPatternFileName, "*"));
    		List<DataGrabbingPattern> list = new List<DataGrabbingPattern>();
    		DataGrabbingPattern dataGrabbingPattern = new DataGrabbingPattern();
    		string[] array = files;
    		foreach (string path4 in array)
    		{
    			list.Add(dataGrabbingPattern.UnserializeFromJSON(File.ReadAllText(path4)));
    		}
    		string taskName = (dictionary.ContainsKey("TaskName") ? dictionary["TaskName"].ToString() : "");
    		string websiteRootUrl = (dictionary.ContainsKey("WebsiteRootUrl") ? dictionary["WebsiteRootUrl"].ToString() : "");
    		string taskDescription = (dictionary.ContainsKey("TaskDescription") ? dictionary["TaskDescription"].ToString() : "");
    		string taskOperatingDirectory = (dictionary.ContainsKey("TaskOperatingDirectory") ? dictionary["TaskOperatingDirectory"].ToString() : "");
    		return new DataExcavatorTask(taskName, websiteRootUrl, taskDescription, list, crawlingServerProperties, grabbingServerProperties, taskOperatingDirectory);
    	}

    	/// <summary>
    	/// Exports task into JSON string
    	/// </summary>
    	/// <param name="TaskLink">Link to exporting task</param>
    	/// <returns>Serialized task data</returns>
    	public string ExportDETaskIntoJSON(DataExcavatorTask TaskLink)
    	{
    		DataExcavatorTaskExportContainer dataExcavatorTaskExportContainer = new DataExcavatorTaskExportContainer(TaskLink.TaskName, TaskLink.WebsiteRootUrl, TaskLink.TaskDescription, TaskLink.GetCrawlingServerPropertiesCopy(), TaskLink.GetGrabbingServerPropertiesCopy(), TaskLink.GetDataGrabbingPatternsCopy());
    		return dataExcavatorTaskExportContainer.SerializeToJSON();
    	}

    	/// <summary>
    	/// Imports data from specified JSON array
    	/// </summary>
    	/// <param name="JSONData"></param>
    	/// <returns></returns>
    	public DataExcavatorTaskExportContainer ImportDETaskFromJSON(string JSONData)
    	{
    		DataExcavatorTaskExportContainer dataExcavatorTaskExportContainer = new DataExcavatorTaskExportContainer();
    		return dataExcavatorTaskExportContainer.UnserializeFromJSON(JSONData);
    	}

    	/// <summary>
    	/// Exports DataExcavatorTask settings
    	/// </summary>
    	/// <param name="FilePath">Path to saving file</param>
    	/// <param name="TaskLink">Link to exporting task</param>
    	public void ExportDETaskIntoFile(string FilePath, DataExcavatorTask TaskLink)
    	{
    		string contents = ExportDETaskIntoJSON(TaskLink);
    		File.WriteAllText(FilePath, contents);
    	}

    	/// <summary>
    	/// Imports data from specified file
    	/// </summary>
    	/// <param name="FilePath"></param>
    	public DataExcavatorTaskExportContainer ImportDETaskFromFile(string FilePath)
    	{
    		if (!File.Exists(FilePath))
    		{
    			throw new FileNotFoundException($"File with path {FilePath} not found");
    		}
    		string jSONData = File.ReadAllText(FilePath);
    		return ImportDETaskFromJSON(jSONData);
    	}

    	/// <summary>
    	/// Deletes some task from HDD
    	/// </summary>
    	public void DeleteDETask(string TaskOperatingDirectory)
    	{
    		string[] files = Directory.GetFiles(TaskOperatingDirectory);
    		string[] directories = Directory.GetDirectories(TaskOperatingDirectory);
    		string[] array = files;
    		foreach (string path in array)
    		{
    			try
    			{
    				File.Delete(path);
    			}
    			catch (Exception)
    			{
    			}
    		}
    		string[] array2 = directories;
    		foreach (string path2 in array2)
    		{
    			try
    			{
    				Directory.Delete(path2, recursive: true);
    			}
    			catch (Exception)
    			{
    			}
    		}
    		try
    		{
    			Directory.Delete(TaskOperatingDirectory);
    		}
    		catch (Exception)
    		{
    		}
    	}
    }
}
