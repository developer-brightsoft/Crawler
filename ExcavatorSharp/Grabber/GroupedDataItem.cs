// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Grabber.GroupedDataItem
namespace ExcavatorSharp.Grabber
{
    /// <summary>
    /// Results of grabbing of some data with specified css or x-path selector.
    /// </summary>
    public class GroupedDataItem
    {
    	/// <summary>
    	/// The name of the selector
    	/// </summary>
    	public string DataGrabbingPatternItemElementName { get; set; }

    	/// <summary>
    	/// Results of grabing with used selector. There is a lot of situations, when needed to parse many 
    	/// items with some selector. Thats why we used List instead to single item.
    	/// </summary>
    	public GroupedDataItemDescendant[] GrabbedItemsData { get; set; }
    }
}
