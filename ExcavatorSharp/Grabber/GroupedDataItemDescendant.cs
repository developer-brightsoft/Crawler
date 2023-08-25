// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Grabber.GroupedDataItemDescendant
namespace ExcavatorSharp.Grabber
{
    /// <summary>
    /// Some founded data element
    /// </summary>
    public class GroupedDataItemDescendant
    {
    	/// <summary>
    	/// Sequence number of element 
    	/// </summary>
    	public int ElementSequenceNr { get; set; }

    	/// <summary>
    	/// Element outer html data (Element HTML + inner data)
    	/// </summary>
    	public string ElementOuterHtml { get; set; }

    	/// <summary>
    	/// Element inner text without any HTML
    	/// </summary>
    	public string ElementInnerText { get; set; }

    	/// <summary>
    	/// Set of attributes if attributes parsing was requested
    	/// </summary>
    	public DescendantAttributeData[] ElementAttributes { get; set; }
    }
}
