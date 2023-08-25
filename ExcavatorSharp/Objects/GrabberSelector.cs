// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.GrabberSelector
namespace ExcavatorSharp.Objects
{
    /// <summary>
    /// Data grabbing selector, used for defining some targeted block
    /// </summary>
    public class GrabberSelector
    {
    	/// <summary>
    	/// CSS selector or XPath selector that identify item to be grabbed.
    	/// We recommend to use CSS selectors, because its more predictable and simple way.
    	/// For more information about css selectors please read https://www.w3schools.com/cssref/css_selectors.asp
    	/// For example, if you want to get page H1 with class 'heading' use 'h1.heading' Selector.
    	/// </summary>
    	public string Selector { get; set; }

    	/// <summary>
    	/// Type of the used selector in Selector field;
    	/// </summary>
    	public DataGrabbingSelectorType SelectorType { get; set; }

    	/// <summary>
    	/// Creates a new instance of GrabberSelector
    	/// </summary>
    	/// <param name="Selector">CSS selector or XPath selector that identify item to be grabbed. We recommend to use CSS selectors, because its more predictable and simple way. For more information about css selectors please read https://www.w3schools.com/cssref/css_selectors.asp For example, if you want to get page H1 with class 'heading' use 'h1.heading' Selector.</param>
    	/// <param name="SelectorType">Type of selector</param>
    	public GrabberSelector(string Selector, DataGrabbingSelectorType SelectorType)
    	{
    		this.Selector = Selector;
    		this.SelectorType = SelectorType;
    	}

    	/// <summary>
    	/// Calculates hash of the Grabber selector
    	/// </summary>
    	/// <returns>Grabber selector hash</returns>
    	public override int GetHashCode()
    	{
    		return Selector.GetHashCode() + SelectorType.GetHashCode();
    	}
    }
}
