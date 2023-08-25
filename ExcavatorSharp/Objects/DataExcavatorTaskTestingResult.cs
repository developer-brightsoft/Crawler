// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DataExcavatorTaskTestingResult
using System.Collections.Generic;
using ExcavatorSharp.Grabber;

namespace ExcavatorSharp.Objects
{
    /// <summary>
    /// Class for testing DataExcavator tasks
    /// </summary>
    public class DataExcavatorTaskTestingResult
    {
    	/// <summary>
    	/// Testing logs list
    	/// </summary>
    	public List<string> LogsList = new List<string>();

    	/// <summary>
    	/// Is task tested successfullly
    	/// </summary>
    	public bool Success { get; set; }

    	/// <summary>
    	/// Grabbed page callback data
    	/// </summary>
    	public PageGrabbedCallback PageGrabbedResponseData { get; set; }

    	/// <summary>
    	/// Crawled page callback data
    	/// </summary>
    	public PageCrawledCallback PageCrawledResponseData { get; set; }

    	/// <summary>
    	/// Grabbed results
    	/// </summary>
    	public GrabbedDataGroupContainer GrabbedData { get; set; }
    }
}
