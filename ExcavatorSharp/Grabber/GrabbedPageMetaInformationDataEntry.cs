// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Grabber.GrabbedPageMetaInformationDataEntry
using System;
using System.Collections.Generic;
using System.Linq;
using ExcavatorSharp.Interfaces;
using Newtonsoft.Json;

namespace ExcavatorSharp.Grabber
{
    /// <summary>
    /// Structure for storing binary data of some grabbed page. Structure using for IO and Exporting data.
    /// </summary>
    public class GrabbedPageMetaInformationDataEntry : IJSONConvertible<GrabbedPageMetaInformationDataEntry>, IJSONListImportable<GrabbedPageMetaInformationDataEntry>
    {
    	/// <summary>
    	/// Date and time when page was grabbed
    	/// </summary>
    	public DateTime PageGrabbedDateTime { get; set; }

    	/// <summary>
    	/// Url of the grabbed page
    	/// </summary>
    	public string GrabbedPageUrl { get; set; }

    	/// <summary>
    	/// Is page contained some data than was fetched using patterns
    	/// </summary>
    	public bool HasResults { get; set; }

    	/// <summary>
    	/// Link to a folder with grabbing results
    	/// </summary>
    	public string ResultsFolderLink { get; set; }

    	/// <summary>
    	/// Size of the grabbed data in Kb (data + binary files)
    	/// </summary>
    	public double DataSizeKb { get; set; }

    	/// <summary>
    	/// Count of the binary files
    	/// </summary>
    	public int BinaryFilesCount { get; set; }

    	/// <summary>
    	/// Creates a new instance with empty data. Usually, used for creatimg empty class for using UnserializeFromJSON method.
    	/// </summary>
    	public GrabbedPageMetaInformationDataEntry()
    	{
    	}

    	/// <summary>
    	/// Creates a new instance with grabbed page meta. Prepare data to saving or export.
    	/// </summary>
    	/// <param name="PageGrabbedDateTime">Date and time when page was grabbed</param>
    	/// <param name="GrabbedPageUrl">Url of the grabbed page</param>
    	/// <param name="HasResults">Is page contained some data than was fetched using patterns</param>
    	/// <param name="ResultsFolderLink">Link to a folder with grabbing results</param>
    	/// <param name="DataSizeKb">Size of the grabbed data in Kb (data + binary files)</param>
    	/// <param name="BinaryFilesCount">Count of the binary files</param>
    	public GrabbedPageMetaInformationDataEntry(DateTime PageGrabbedDateTime, string GrabbedPageUrl, bool HasResults, string ResultsFolderLink, double DataSizeKb, int BinaryFilesCount)
    	{
    		this.PageGrabbedDateTime = PageGrabbedDateTime;
    		this.GrabbedPageUrl = GrabbedPageUrl;
    		this.HasResults = HasResults;
    		this.ResultsFolderLink = ResultsFolderLink;
    		this.DataSizeKb = DataSizeKb;
    		this.BinaryFilesCount = BinaryFilesCount;
    	}

    	/// <summary>
    	/// Serializes object to JSON using Newtonsoft.JSON
    	/// </summary>
    	/// <returns>Object JSON presentation</returns>
    	public string SerializeToJSON()
    	{
    		return JsonConvert.SerializeObject((object)this, (Formatting)1);
    	}

    	/// <summary>
    	/// Unpacks new instance of GrabbedPageMetaInformationDataEntry from JSON string
    	/// </summary>
    	/// <param name="JSONData">Object JSON source data</param>
    	/// <returns>New object instance</returns>
    	public GrabbedPageMetaInformationDataEntry UnserializeFromJSON(string JSONData)
    	{
    		return JsonConvert.DeserializeObject<GrabbedPageMetaInformationDataEntry>(JSONData);
    	}

    	/// <summary>
    	/// Unpacks new List of instances of GrabbedPageMetaInformationDataEntry from JSOn string
    	/// </summary> 
    	/// <param name="JSONData">List JSON source data</param>
    	/// <returns>List of instances of GrabbedPageMetaInformationDataEntry</returns>
    	public List<GrabbedPageMetaInformationDataEntry> UnserializeListFromJSON(string JSONData)
    	{
    		return (from item in JsonConvert.DeserializeObject<List<GrabbedPageMetaInformationDataEntry>>(JSONData)
    			where item != null
    			select item).ToList();
    	}

    	/// <summary>
    	/// Returns binary files folder full link
    	/// </summary>
    	/// <returns></returns>
    	public string GetBinaryFolderFolderLink()
    	{
    		return string.Format("{0}/{1}/{2}", "grabbed-data", ResultsFolderLink, "binary-files");
    	}

    	/// <summary>
    	/// Returns full path to file with parsed data
    	/// </summary>
    	/// <returns></returns>
    	public string GetParsedDataFileLink()
    	{
    		return string.Format("{0}/{1}/{2}", "grabbed-data", ResultsFolderLink, "parsed-data.json");
    	}

    	internal object GetActualGrabbedPagesMeta()
    	{
    		throw new NotImplementedException();
    	}
    }
}
