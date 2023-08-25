// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DataGrabbingResultItem
using System.Collections.Generic;
using HtmlAgilityPack;

namespace ExcavatorSharp.Objects
{
    /// <summary>
    /// Result for the parsing of some element
    /// </summary>
    public class DataGrabbingResultItem
    {
    	/// <summary>
    	/// Depended nodes, founded by some pattern
    	/// </summary>
    	public List<HtmlNode> ResultedNodes { get; set; }

    	/// <summary>
    	/// Depended attributes content, if attributes parsing was requested
    	/// </summary>
    	public List<KeyValuePair<HtmlNode, DataGrabbingResultItemBinaryAttributeData>> NodeAttributesContent { get; set; }
    }
}
