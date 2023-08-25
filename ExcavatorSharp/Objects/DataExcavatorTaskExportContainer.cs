// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DataExcavatorTaskExportContainer
using System.Collections.Generic;
using ExcavatorSharp.Excavator;
using ExcavatorSharp.Interfaces;
using Newtonsoft.Json;

namespace ExcavatorSharp.Objects
{
    /// <summary>
    /// Class for storing data before import and export operations
    /// </summary>
    public class DataExcavatorTaskExportContainer : IJSONConvertible<DataExcavatorTaskExportContainer>
    {
    	/// <summary>
    	/// Name of a project
    	/// </summary>
    	public string ProjectName { get; set; }

    	/// <summary>
    	/// Website root url
    	/// </summary>
    	public string WebsiteRootUrl { get; set; }

    	/// <summary>
    	/// Task description text
    	/// </summary>
    	public string ProjectDescription { get; set; }

    	/// <summary>
    	/// Crawling server properties
    	/// </summary>
    	public CrawlingServerProperties CrawlerPeroperties { get; set; }

    	/// <summary>
    	/// Grabbing server properties
    	/// </summary>
    	public GrabbingServerProperties GrabberProperties { get; set; }

    	/// <summary>
    	/// Grabbing patterns
    	/// </summary>
    	public List<DataGrabbingPattern> GrabbingPatterns { get; set; }

    	/// <summary>
    	/// Empty constructor
    	/// </summary>
    	public DataExcavatorTaskExportContainer()
    	{
    	}

    	/// <summary>
    	/// Creates new instance of DataExcavatorTaskExportContainer
    	/// </summary>
    	/// <param name="ProjectName">Name of a project</param>
    	/// <param name="WebsiteRootUrl">Website root url</param>
    	/// <param name="CrawlerProperties">Crawling server properties</param>
    	/// <param name="GrabberProperties">Grabbing server properties</param>
    	/// <param name="GrabbingPatternsList">Grabbing patterns</param>
    	public DataExcavatorTaskExportContainer(string ProjectName, string WebsiteRootUrl, string ProjectDescription, CrawlingServerProperties CrawlerProperties, GrabbingServerProperties GrabberProperties, List<DataGrabbingPattern> GrabbingPatterns)
    	{
    		this.ProjectName = ProjectName;
    		this.WebsiteRootUrl = WebsiteRootUrl;
    		this.ProjectDescription = ProjectDescription;
    		CrawlerPeroperties = CrawlerProperties;
    		this.GrabberProperties = GrabberProperties;
    		this.GrabbingPatterns = GrabbingPatterns;
    	}

    	/// <summary>
    	/// Serializes data to JSON
    	/// </summary>
    	/// <returns>Serialized data</returns>
    	public string SerializeToJSON()
    	{
    		return JsonConvert.SerializeObject((object)this, (Formatting)1);
    	}

    	/// <summary>
    	/// Unserializes data from JSON
    	/// </summary>
    	/// <param name="JSONData">Source data to unserialize</param>
    	/// <returns>Unserialized instance of DataExcavatorTaskExportContainer</returns>
    	public DataExcavatorTaskExportContainer UnserializeFromJSON(string JSONData)
    	{
    		return JsonConvert.DeserializeObject<DataExcavatorTaskExportContainer>(JSONData);
    	}

    	/// <summary>
    	/// Creates task from actual settings set
    	/// </summary>
    	/// <returns></returns>
    	public DataExcavatorTask GetTaskFromProperties(string TaskDirectory)
    	{
    		return new DataExcavatorTask(ProjectName, WebsiteRootUrl, ProjectDescription, GrabbingPatterns, CrawlerPeroperties, GrabberProperties, TaskDirectory);
    	}
    }
}
