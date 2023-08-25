// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.GrabbingServerProperties
using System;
using ExcavatorSharp.Interfaces;
using Newtonsoft.Json;

namespace ExcavatorSharp.Objects
{
    /// <summary>
    /// Container for crawler properties
    /// </summary>
    public class GrabbingServerProperties : IJSONConvertible<GrabbingServerProperties>, ICloneable
    {
    	/// <summary>
    	/// The number of grabbing threads count. Should be setted ONLY AT INITIALIZATION. No effect if changed from the initialized GrabbingServer object.
    	/// </summary>
    	public int GrabbingThreadsCount { get; set; }

    	/// <summary>
    	/// Delay of grabbing thread, in milliseconds.
    	/// </summary>
    	public int GrabbingThreadDelayMilliseconds { get; set; }

    	/// <summary>
    	/// Is it necessary to store grabbed data on HDD and uses it during analysing process.
    	/// If you will set this value to FALSE, all grabbed data will be transferred ONLY into PageGrabbed event of GrabbingServer.
    	/// </summary>
    	public bool StoreGrabbedData { get; set; }

    	/// <summary>
    	/// Store only non-empty data in results folder
    	/// </summary>
    	public bool StoreOnlyNonEmptyData { get; set; }

    	/// <summary>
    	/// Is it necessary to export data online after taking data from each page?
    	/// </summary>
    	public bool ExportDataOnline { get; set; }

    	/// <summary>
    	/// Callback link to export data. Params will be passed in $_POST array
    	/// </summary>
    	public string ExportDataOnlineInvokationLink { get; set; }

    	/// <summary>
    	/// Initialize a new properties container
    	/// </summary>
    	/// <param name="GrabbingThreadsCount">The number of grabbing threads count</param>
    	/// <param name="GrabbingThreadDelayMilliseconds">Delay of grabbing thread, in milliseconds.</param>
    	/// <param name="StoreGrabbedData">Is it necessary to store grabbed data on HDD and uses it during analysing process. If you will set this value to FALSE, all grabbed data will be transferred ONLY into PageGrabbed event of GrabbingServer.</param>
    	/// <param name="ExportDataOnline">Is it necessary to export data online after taking data from each page?</param>
    	/// <param name="ExportDataOnlineInvokationLink">Callback link to export data. Params will be passed in $_POST array</param>
    	/// <param name="StoreOnlyNonEmptyData">Store only non-empty data in results folder</param>
    	public GrabbingServerProperties(int GrabbingThreadsCount = 2, int GrabbingThreadDelayMilliseconds = 5000, bool StoreGrabbedData = true, bool StoreOnlyNonEmptyData = true, bool ExportDataOnline = false, string ExportDataOnlineInvokationLink = "")
    	{
    		this.GrabbingThreadsCount = GrabbingThreadsCount;
    		this.GrabbingThreadDelayMilliseconds = GrabbingThreadDelayMilliseconds;
    		this.StoreGrabbedData = StoreGrabbedData;
    		this.StoreOnlyNonEmptyData = StoreOnlyNonEmptyData;
    		this.ExportDataOnline = ExportDataOnline;
    		this.ExportDataOnlineInvokationLink = ExportDataOnlineInvokationLink;
    	}

    	/// <summary>
    	/// Serializes data to JSON
    	/// </summary>
    	/// <returns></returns>
    	public string SerializeToJSON()
    	{
    		return JsonConvert.SerializeObject((object)this, (Formatting)1);
    	}

    	/// <summary>
    	/// Unserializes data from JSON
    	/// </summary>
    	/// <param name="JSONData">Input JSON data</param>
    	/// <returns>New instance of GrabbingServerProperties</returns>
    	public GrabbingServerProperties UnserializeFromJSON(string JSONData)
    	{
    		return JsonConvert.DeserializeObject<GrabbingServerProperties>(JSONData);
    	}

    	/// <summary>
    	/// Clones DataGrabbingServerProperties
    	/// </summary>
    	/// <returns></returns>
    	public object Clone()
    	{
    		string jSONData = SerializeToJSON();
    		return UnserializeFromJSON(jSONData);
    	}
    }
}
