// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Grabber.GrabbedDataGroup
using System;
using System.Collections.Generic;
using System.Linq;
using ExcavatorSharp.Exporter;
using ExcavatorSharp.ExtensionMethods;
using ExcavatorSharp.Interfaces;
using Newtonsoft.Json;

namespace ExcavatorSharp.Grabber
{
    /// <summary>
    /// For each pattern you define into task, system can create a GrabbedDataGroup.
    /// It organized with using Group, because you can define multiple patterns for
    /// grabbing page, and can separately parse data for each pattern you defined.
    /// </summary>
    public class GrabbedDataGroup : IJSONConvertible<GrabbedDataGroup>, IJSONListImportable<GrabbedDataGroup>, IApproximateGrabbedDataSizeCalculable
    {
    	/// <summary>
    	/// Name of used pattern
    	/// </summary>
    	public string PatternName { get; set; }

    	/// <summary>
    	/// Hash code of used pattern
    	/// </summary>
    	public int PatternHash { get; set; }

    	/// <summary>
    	/// Url of grabbed page
    	/// </summary>
    	public string GrabbedPageUrl { get; set; }

    	/// <summary>
    	/// Is there is no founded data by any pattern you define
    	/// </summary>
    	public bool IsEmptyResultSet { get; set; }

    	/// <summary>
    	/// Date and time when page was grabbed
    	/// </summary>
    	public DateTime PageGrabbedDateTime { get; set; }

    	/// <summary>
    	/// Result of the grabbing - organized as a two-dimensional Array.  
    	/// First dimension - its a sub-groups, fetched with DataGrabbingPattern.OuterBlockSelector. 
    	/// Second dimension - its a results founded into groups of the first dimension. For example, you must 
    	/// grab data from a some website, thats organized as set of the same-blocks, like a tile (f.e., website 
    	/// pages contains N blocks with data you want to grab). In this situation you can define outer selector for 
    	/// block (DataGrabbingPattern.OuterBlockSelector), and all data be packed correctly by using two-simensional 
    	/// array. If you grab data from simple typical page, like a page of e-commerce website, this array will
    	/// presented as Array( 0 =&gt; Array ( 0 =&gt; GroupedDataItem, 1 =&gt; GroupedDataItem, ...) );
    	/// </summary>
    	public List<List<GroupedDataItem>> GrabbingResults { get; set; }

    	/// <summary>
    	/// Calculates object approximate size in bytes
    	/// </summary>
    	/// <returns></returns>
    	public double CalculateApproximateGrabbedDataSize()
    	{
    		double num = 0.0;
    		num += GrabbedPageUrl.GetStringApproximateSizeInBytes();
    		foreach (List<GroupedDataItem> grabbingResult in GrabbingResults)
    		{
    			foreach (GroupedDataItem item in grabbingResult)
    			{
    				num += item.DataGrabbingPatternItemElementName.GetStringApproximateSizeInBytes();
    				GroupedDataItemDescendant[] grabbedItemsData = item.GrabbedItemsData;
    				foreach (GroupedDataItemDescendant groupedDataItemDescendant in grabbedItemsData)
    				{
    					if (groupedDataItemDescendant.ElementOuterHtml != null)
    					{
    						num += groupedDataItemDescendant.ElementOuterHtml.GetStringApproximateSizeInBytes();
    					}
    					if (groupedDataItemDescendant.ElementInnerText != null)
    					{
    						num += groupedDataItemDescendant.ElementInnerText.GetStringApproximateSizeInBytes();
    					}
    				}
    			}
    		}
    		return num;
    	}

    	/// <summary>
    	/// Serializes item data into JSON
    	/// </summary>
    	/// <returns>JSON presentation of GroupedDataItem</returns>
    	public string SerializeToJSON()
    	{
    		return JsonConvert.SerializeObject((object)this, (Formatting)1);
    	}

    	/// <summary>
    	/// Deserialized instance of GroupedDataItem from JSON
    	/// </summary>
    	/// <param name="JSONData">JSON data</param>
    	/// <returns>New GroupedDataItem instance</returns>
    	public GrabbedDataGroup UnserializeFromJSON(string JSONData)
    	{
    		return JsonConvert.DeserializeObject<GrabbedDataGroup>(JSONData);
    	}

    	/// <summary>
    	/// Returns a set of PatternDataGroup from JSON object
    	/// </summary>
    	/// <param name="JSONData">Array JSON data</param>
    	/// <returns>List of PatternDataGroup objects</returns>
    	public List<GrabbedDataGroup> UnserializeListFromJSON(string JSONData)
    	{
    		return (from item in JsonConvert.DeserializeObject<List<GrabbedDataGroup>>(JSONData)
    			where item != null
    			select item).ToList();
    	}

    	/// <summary>
    	/// Presents grabbed data as exported JSON string
    	/// </summary>
    	/// <param name="GrabbedDataGroups">List of grabed data groups</param>
    	/// <returns>Packed JSON string</returns>
    	public static string PresentAsExportedJSON(List<GrabbedDataGroup> GrabbedDataGroups, DataExportingType ExportingType)
    	{
    		JSONExporter jSONExporter = new JSONExporter();
    		jSONExporter.InitializeExpoter(string.Empty, ExportingType, ",", null, null);
    		List<object> list = jSONExporter.PackExportingDataIntoJSONPreparedObjects(GrabbedDataGroups);
    		return JsonConvert.SerializeObject((object)list, (Formatting)1);
    	}
    }
}
