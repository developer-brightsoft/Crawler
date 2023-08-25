// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DataGrabbingPattern
using System;
using System.Collections.Generic;
using ExcavatorSharp.Interfaces;
using Newtonsoft.Json;

namespace ExcavatorSharp.Objects
{
    /// <summary>
    /// Pattern for grabbing some set of data from page HTML.
    /// Used for defining the set of Unary patterns for data grabbing.
    /// </summary>
    public class DataGrabbingPattern : IJSONConvertible<DataGrabbingPattern>, IJSONListExportable<DataGrabbingPattern>, ICloneable
    {
    	/// <summary>
    	/// Name for pattern
    	/// </summary>
    	public string PatternName { get; set; }

    	/// <summary>
    	/// List of a URL substrings that defines for what page can be applied this pattern. If not set or setted to null - pattern
    	/// will be tried for all crawled pages.
    	/// </summary>
    	public string[] AllowedPageUrlsSubstrings { get; set; }

    	/// <summary>
    	/// Selector for defining some outer block for other child selectors. You must use this 
    	/// property for pages with many parsing items, like a data tables, or data into till blocks.
    	/// You dont need to use this property for standard items pages, where single good located into it's page.
    	/// </summary>
    	public GrabberSelector OuterBlockSelector { get; set; }

    	/// <summary>
    	/// A set of Unary patterns for some elements.
    	/// </summary>
    	public List<DataGrabbingPatternItem> GrabbingItemsPatterns { get; set; }

    	/// <summary>
    	/// Serializes DataGrabbingPattern to JSON string
    	/// </summary>
    	/// <returns>JSON string with data of DataGrabbingPattern</returns>
    	public string SerializeToJSON()
    	{
    		return JsonConvert.SerializeObject((object)this, (Formatting)1);
    	}

    	/// <summary>
    	/// Creates a new instance of DataGrabbingPattern from JSON string
    	/// </summary>
    	/// <param name="JSONData">JSON data with content of some DataGrabbingPattern object</param>
    	/// <returns>New instance of DataGrabbingPattern</returns>
    	public DataGrabbingPattern UnserializeFromJSON(string JSONData)
    	{
    		return JsonConvert.DeserializeObject<DataGrabbingPattern>(JSONData);
    	}

    	/// <summary>
    	/// Get hash code of DataGrabbingPattern
    	/// </summary>
    	/// <returns>Hash code of DataGrabbingPattern</returns>
    	public override int GetHashCode()
    	{
    		int num = 7;
    		int num2 = num * 5 * PatternName.GetHashCode();
    		for (int i = 0; i < AllowedPageUrlsSubstrings.Length; i++)
    		{
    			num2 += AllowedPageUrlsSubstrings[i].GetHashCode();
    		}
    		for (int j = 0; j < GrabbingItemsPatterns.Count; j++)
    		{
    			num2 += GrabbingItemsPatterns[j].GetHashCode();
    		}
    		if (OuterBlockSelector != null)
    		{
    			num2 += OuterBlockSelector.GetHashCode();
    		}
    		if (num2 < 0)
    		{
    			num2 *= -1;
    		}
    		return num2;
    	}

    	/// <summary>
    	/// Serializes list to JSON format
    	/// </summary>
    	/// <param name="ObjectsData">List of objects data</param>
    	/// <returns>Serialized data</returns>
    	public string SerializeListToJSON(List<DataGrabbingPattern> ObjectsData)
    	{
    		return JsonConvert.SerializeObject((object)ObjectsData, (Formatting)1);
    	}

    	/// <summary>
    	/// Clones object
    	/// </summary>
    	/// <returns></returns>
    	public object Clone()
    	{
    		string jSONData = SerializeToJSON();
    		return UnserializeFromJSON(jSONData);
    	}

    	/// <summary>
    	/// Gets default pattern as an example
    	/// </summary>
    	/// <returns></returns>
    	public static DataGrabbingPattern GetDefaultPattern()
    	{
    		DataGrabbingPattern dataGrabbingPattern = new DataGrabbingPattern();
    		dataGrabbingPattern.AllowedPageUrlsSubstrings = new string[1] { "*" };
    		dataGrabbingPattern.OuterBlockSelector = null;
    		dataGrabbingPattern.PatternName = "Example_1";
    		dataGrabbingPattern.GrabbingItemsPatterns = new List<DataGrabbingPatternItem>();
    		dataGrabbingPattern.GrabbingItemsPatterns.Add(new DataGrabbingPatternItem("Example - Page heading H1", new GrabberSelector("h1", DataGrabbingSelectorType.CSS_Selector)));
    		dataGrabbingPattern.GrabbingItemsPatterns.Add(new DataGrabbingPatternItem("Example - Page heading H2", new GrabberSelector("h2", DataGrabbingSelectorType.CSS_Selector)));
    		dataGrabbingPattern.GrabbingItemsPatterns.Add(new DataGrabbingPatternItem("Example - Page description", new GrabberSelector("[itemprop=\"description\"]", DataGrabbingSelectorType.CSS_Selector)));
    		dataGrabbingPattern.GrabbingItemsPatterns.Add(new DataGrabbingPatternItem("Example - Page all images", new GrabberSelector("img", DataGrabbingSelectorType.CSS_Selector), ParseBinaryAttributes: true, new ParsingBinaryAttributePattern[1]
    		{
    			new ParsingBinaryAttributePattern("src", IsAttributeAreLinkToSomeResouce: true, IsWeMustDownloadContentUnderAttributeLink: true)
    		}));
    		return dataGrabbingPattern;
    	}
    }
}
