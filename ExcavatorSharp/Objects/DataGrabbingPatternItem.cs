// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DataGrabbingPatternItem
using System;
using ExcavatorSharp.Interfaces;
using Newtonsoft.Json;

namespace ExcavatorSharp.Objects
{
    /// <summary>
    /// Pattern for grabbing some unary item from page HTML.
    /// 'Unary' means that DataGrabbingPatternItem contains data for grabbing only single item of any type, like: ItemName or ItemManufacturer or ItemPrice or something other.
    /// </summary>
    public class DataGrabbingPatternItem : IJSONConvertible<DataGrabbingPatternItem>, ICloneable
    {
    	/// <summary>
    	/// Any element name for saving grabbed data
    	/// </summary>
    	public string ElementName { get; set; }

    	/// <summary>
    	/// Target selector for defining a fetching element from the page HTML
    	/// </summary>
    	public GrabberSelector DataSelector { get; set; }

    	/// <summary>
    	/// Is it necessary to download included binary files, like images, .pdf-files, docs and someother
    	/// </summary>
    	public bool ParseBinaryAttributes { get; set; }

    	/// <summary>
    	/// The list of allowed for parsing binary attributes, like src= data-src= or someother
    	/// </summary>
    	public ParsingBinaryAttributePattern[] ParsingBinaryAttributes { get; set; }

    	/// <summary>
    	/// Get hash code of DataGrabbingPatternItem
    	/// </summary>
    	/// <returns>Hash code of DataGrabbingPatternItem</returns>
    	public override int GetHashCode()
    	{
    		int num = DataSelector.Selector.GetHashCode() + ElementName.GetHashCode() + ParseBinaryAttributes.GetHashCode();
    		if (ParsingBinaryAttributes != null)
    		{
    			for (int i = 0; i < ParsingBinaryAttributes.Length; i++)
    			{
    				num += ParsingBinaryAttributes[i].GetHashCode();
    			}
    		}
    		return num;
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
    	/// Serializes DataGrabbingPatternItem to JSON string
    	/// </summary>
    	/// <returns>JSON string with data of DataGrabbingPattern</returns>
    	public string SerializeToJSON()
    	{
    		return JsonConvert.SerializeObject((object)this, (Formatting)1);
    	}

    	/// <summary>
    	/// Creates a new instance of DataGrabbingPatternItem from JSON string
    	/// </summary>
    	/// <param name="JSONData">JSON data with content of some DataGrabbingPatternItem object</param>
    	/// <returns>New instance of DataGrabbingPatternItem</returns>
    	public DataGrabbingPatternItem UnserializeFromJSON(string JSONData)
    	{
    		return JsonConvert.DeserializeObject<DataGrabbingPatternItem>(JSONData);
    	}

    	/// <summary>
    	/// Creates data grabbing pattern for some Unary item.
    	/// </summary>
    	/// <param name="ElementName">Any element name for saving grabbed data, keeping pattern structure and other.</param>
    	/// <param name="Selector"> CSS selector or XPath selector that identify item to be grabbed.</param>
    	/// <param name="ParseBinaryAttributes">Is it necessary to download included binary files, like images, .pdf-files, docs and someother</param>
    	/// <param name="ParsingBinaryAttributes"></param>
    	/// <param name="SelectorType">Type of the used selector in Selector field;</param>
    	public DataGrabbingPatternItem(string ElementName, GrabberSelector DataSelector, bool ParseBinaryAttributes = false, ParsingBinaryAttributePattern[] ParsingBinaryAttributes = null)
    	{
    		this.ElementName = ElementName;
    		this.DataSelector = DataSelector;
    		this.ParseBinaryAttributes = ParseBinaryAttributes;
    		this.ParsingBinaryAttributes = ParsingBinaryAttributes;
    	}
    }
}
