// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Exporter.HTTPExporter
using System;
using System.Collections.Generic;
using System.Dynamic;
using ExcavatorSharp.Common;
using ExcavatorSharp.Excavator;
using ExcavatorSharp.Grabber;
using ExcavatorSharp.Objects;
using Newtonsoft.Json;
using RestSharp;

namespace ExcavatorSharp.Exporter
{
    /// <summary>
    /// Exporter for transferring data online with http-request. Not implements IDataContextualExportable, because used for online exporting.
    /// </summary>
    internal class HTTPExporter
    {
    	/// <summary>
    	/// Parent task name
    	/// </summary>
    	private string ParentTaskName { get; set; }

    	/// <summary>
    	/// Exporting URL
    	/// </summary>
    	private string ExportURL { get; set; }

    	/// <summary>
    	/// Link to task logger object
    	/// </summary>
    	private DataExcavatorTasksLogger TaskLoggerLink { get; set; }

    	/// <summary>
    	/// Creates new instance of HTTPExporter
    	/// </summary>
    	/// <param name="ParentTaskName">Parent task name</param>
    	/// <param name="ExportURL">Exporting URL</param>
    	/// <param name="TaskLoggerLink">Link to task logger object</param>
    	public HTTPExporter(string ParentTaskName, string ExportURL, DataExcavatorTasksLogger TaskLoggerLink)
    	{
    		this.ParentTaskName = ParentTaskName;
    		this.ExportURL = ExportURL;
    		this.TaskLoggerLink = TaskLoggerLink;
    	}

    	/// <summary>
    	/// Exports data using HTTP $_POST request
    	/// </summary>
    	/// <param name="PackedResults">Grabbed data results</param>
    	public void ExportDataViaHTTP(GrabbedDataGroupContainer PackedResults)
    	{
    		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
    		//IL_0167: Expected O, but got Unknown
    		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
    		//IL_0175: Expected O, but got Unknown
    		Dictionary<string, object> dictionary = new Dictionary<string, object>();
    		for (int i = 0; i < PackedResults.BinaryDataItems.Count; i++)
    		{
    			IDictionary<string, object> dictionary2 = new ExpandoObject();
    			dictionary2.Add("BinaryDataGUID", PackedResults.BinaryDataItems[i].AttributeDataGuid);
    			dictionary2.Add("AttributeName", PackedResults.BinaryDataItems[i].AttributeName);
    			dictionary2.Add("AttributeValue", PackedResults.BinaryDataItems[i].AttributeValue);
    			dictionary2.Add("DataSize", (PackedResults.BinaryDataItems[i].ResourceContent != null) ? PackedResults.BinaryDataItems[i].ResourceContent.Length : 0);
    			if (PackedResults.BinaryDataItems[i].ResourceContent != null)
    			{
    				dictionary2.Add("DataContent", Convert.ToBase64String(PackedResults.BinaryDataItems[i].ResourceContent));
    			}
    			else
    			{
    				dictionary2.Add("DataContent", "");
    			}
    			dictionary.Add(PackedResults.BinaryDataItems[i].AttributeDataGuid, dictionary2);
    		}
    		JSONExporter jSONExporter = new JSONExporter();
    		List<object> list = jSONExporter.PackExportingDataIntoJSONPreparedObjects(PackedResults.GrabbedDataGroups);
    		string text = JsonConvert.SerializeObject((object)list);
    		string text2 = JsonConvert.SerializeObject((object)dictionary);
    		RestClient val = new RestClient();
    		RestRequest val2 = new RestRequest(ExportURL, (Method)1);
    		val2.AddParameter(DEConfig.ExportDataOnFly_TaskNameParamName, (object)ParentTaskName);
    		val2.AddParameter(DEConfig.ExportDataOnFly_EncodedContentParamName, (object)text);
    		val2.AddParameter(DEConfig.ExportDataOnFly_BinaryContentParamName, (object)text2);
    		IRestResponse val3 = null;
    		try
    		{
    			val3 = val.Execute((IRestRequest)(object)val2);
    		}
    		catch (Exception occuredException)
    		{
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.DataExporter, $"Cant export data online by url = {ExportURL}", occuredException);
    		}
    		if (val3.IsSuccessful)
    		{
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.DataExporter, $"Grabbed data exported online. Grabbed page url = {PackedResults.GrabbedPageUrl}");
    		}
    		else
    		{
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.DataExporter, $"Error during online data export. Grabbed page url = {PackedResults.GrabbedPageUrl}, Export url = {ExportURL}");
    		}
    	}
    }
}
