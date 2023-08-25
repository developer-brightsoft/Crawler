// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Common.DOMSelectorsTester
using System;
using System.Collections.Generic;
using System.Linq;
using ExcavatorSharp.Objects;
using HtmlAgilityPack;

/// <summary>
/// Class for testing CSS and XPath Selectors
/// </summary>
namespace ExcavatorSharp.Common
{
	public class DOMSelectorsTester
	{
		/// <summary>
		/// Templates HTML document
		/// </summary>
		public const string TemplatedDOMDocument = "<!DOCTYPE html><html><head><title>Hello there!</title></head><body><div id=\"fooid\">foocontent</div></body></html>";

		/// <summary>
		/// Tests some selector and returns testing results
		/// </summary>
		/// <param name="Selector"></param>
		/// <param name="SelectorType"></param>
		/// <returns></returns>
		public static bool TestSelector(string Selector, DataGrabbingSelectorType SelectorType)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			HtmlDocument val = new HtmlDocument();
			val.LoadHtml("<!DOCTYPE html><html><head><title>Hello there!</title></head><body><div id=\"fooid\">foocontent</div></body></html>");
			try
			{
				List<HtmlNode> list = null;
				switch (SelectorType)
				{
				case DataGrabbingSelectorType.CSS_Selector:
					list = HapCssExtensionMethods.QuerySelectorAll(val, Selector).ToList();
					break;
				case DataGrabbingSelectorType.XPath_Selector:
					list = ((IEnumerable<HtmlNode>)val.DocumentNode.SelectNodes(Selector)).ToList();
					break;
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Make selector testing
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		public static bool TestSelector(GrabberSelector selector)
		{
			_ = selector.SelectorType;
			if (selector.Selector == null || selector.Selector == string.Empty)
			{
				return false;
			}
			return TestSelector(selector.Selector, selector.SelectorType);
		}
	}
}