// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DataGrabbingSelectorType
using System.ComponentModel;

/// <summary>
/// Possibly selector types.
/// CSS selectors: https://www.w3schools.com/cssref/css_selectors.asp
/// XPath selector: https://www.w3schools.com/xml/xpath_syntax.asp
/// </summary>
namespace ExcavatorSharp.Objects
{
	public enum DataGrabbingSelectorType
	{
		/// <summary>
		/// Definition for CSS-selector
		/// </summary>
		[Description("CSS selector")]
		CSS_Selector,
		/// <summary>
		/// Definition for XPath selector
		/// </summary>
		[Description("XPath expression")]
		XPath_Selector
	}
}