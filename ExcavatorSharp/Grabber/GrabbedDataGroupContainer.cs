// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Grabber.GrabbedDataGroupContainer
using System.Collections.Generic;
using ExcavatorSharp.Objects;

namespace ExcavatorSharp.Grabber
{
    /// <summary>
    /// Class for storing GrabbedDataGroup ans a set of binary files
    /// </summary>
    public class GrabbedDataGroupContainer
    {
    	/// <summary>
    	/// URL of grabbed page
    	/// </summary>
    	public string GrabbedPageUrl { get; set; }

    	/// <summary>
    	/// Set of grabbed data groups
    	/// </summary>
    	public List<GrabbedDataGroup> GrabbedDataGroups { get; set; }

    	/// <summary>
    	/// Set of binary data items, included in groups
    	/// </summary>
    	public List<DataGrabbingResultItemBinaryAttributeData> BinaryDataItems { get; set; }

    	/// <summary>
    	/// Metrics of data group
    	/// </summary>
    	public GrabbedDataContainerMetrics DataGroupMetrics { get; private set; }

    	/// <summary>
    	/// Updates data group metrics
    	/// </summary>
    	internal GrabbedDataContainerMetrics UpdateDataGroupMetrics()
    	{
    		GrabbedDataContainerMetrics grabbedDataContainerMetrics = new GrabbedDataContainerMetrics();
    		grabbedDataContainerMetrics.BinaryFilesCount = BinaryDataItems.Count;
    		if (GrabbedDataGroups.Count == 0)
    		{
    			grabbedDataContainerMetrics.HasResults = false;
    		}
    		for (int i = 0; i < GrabbedDataGroups.Count; i++)
    		{
    			if (!GrabbedDataGroups[i].IsEmptyResultSet)
    			{
    				grabbedDataContainerMetrics.HasResults = true;
    				break;
    			}
    		}
    		double num = 0.0;
    		for (int j = 0; j < BinaryDataItems.Count; j++)
    		{
    			num += BinaryDataItems[j].CalculateApproximateGrabbedDataSize();
    		}
    		for (int k = 0; k < GrabbedDataGroups.Count; k++)
    		{
    			num += GrabbedDataGroups[k].CalculateApproximateGrabbedDataSize();
    		}
    		grabbedDataContainerMetrics.GrabbedDataTotalSizeKb = num;
    		DataGroupMetrics = grabbedDataContainerMetrics;
    		return DataGroupMetrics;
    	}

    	/// <summary>
    	/// Creates new instance of 
    	/// </summary>
    	public GrabbedDataGroupContainer(string GrabbedPageUrl)
    	{
    		GrabbedDataGroups = new List<GrabbedDataGroup>();
    		BinaryDataItems = new List<DataGrabbingResultItemBinaryAttributeData>();
    		this.GrabbedPageUrl = GrabbedPageUrl;
    	}
    }
}
