// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.PageLink
using System;
using System.Collections.Generic;
using System.Globalization;
using ExcavatorSharp.Crawler;
using ExcavatorSharp.Interfaces;
using Newtonsoft.Json;

namespace ExcavatorSharp.Objects
{
    /// <summary>
    /// Container for storing a link, parsed from any page
    /// </summary>
    public class PageLink : IJSONConvertible<PageLink>, ICloneable
    {
    	/// <summary>
    	/// Link owner
    	/// </summary>
    	public PageLinkOwnerType LinkOwnerType = PageLinkOwnerType.NotAnalysedYet;

    	/// <summary>
    	/// Link resource type
    	/// </summary>
    	public PageLinkResourceType LinkResourceType = PageLinkResourceType.NotAnalysedYet;

    	/// <summary>
    	/// Marks page as "Recrawling in process".
    	/// Using in crawling server for pages reindexation. Protects link from parallel indexation with many threads.
    	/// </summary>
    	[JsonIgnore]
    	internal volatile bool PageAtRecrawlingProcess = false;

    	/// <summary>
    	/// Link in original format
    	/// </summary>
    	public string OriginalLink { get; set; }

    	/// <summary>
    	/// Link in lowercase format
    	/// </summary>
    	public string OriginalLinkLowercase { get; set; }

    	/// <summary>
    	/// Normalized original link with scheme and website domain
    	/// </summary>
    	public string NormalizedOriginalLink { get; set; }

    	/// <summary>
    	/// Last date and time of link scan
    	/// </summary>
    	public DateTime LinkLastCrawlingDateAndTime { get; set; }

    	/// <summary>
    	/// Last date and time of successful link scan
    	/// </summary>
    	public DateTime LinkLastCrawlingSuccessDateTime { get; set; }

    	/// <summary>
    	/// Determines that NormalizedOriginalLink is well-formatted and absolute to resourse.
    	/// If value is false, you don't need to use this link, because it's broken or it is not URL.
    	/// </summary>
    	public bool IsLinkResolvedAndWellFormattedToAbsolutePathUrl { get; set; }

    	/// <summary>
    	/// Extension of the resource file, if linked to file.
    	/// </summary>
    	public string LinkResourceExtension { get; set; }

    	/// <summary>
    	/// Special postfix used for data excavator inner algorhytms, like CEF crawling with behaviors. Not a part of crawling link.
    	/// </summary>
    	[JsonIgnore]
    	internal string DEBehaviorLinkPoxtfix { get; set; }

    	/// <summary>
    	/// Special flag for link cycled indexation. Can be used with CEF indexation.
    	/// </summary>
    	[JsonIgnore]
    	internal bool DEBehaviorCycledIndexationProcess { get; set; }

    	/// <summary>
    	/// Creates an instance of PageLink with initialization from OriginalLink
    	/// </summary>
    	/// <param name="OriginalLink">Originlal page link</param>
    	public PageLink(string OriginalLink)
    	{
    		this.OriginalLink = OriginalLink;
    		OriginalLinkLowercase = OriginalLink.ToLower(CultureInfo.InvariantCulture);
    		PageAtRecrawlingProcess = false;
    		DEBehaviorLinkPoxtfix = string.Empty;
    		DEBehaviorCycledIndexationProcess = false;
    	}

    	/// <summary>
    	/// Makes typed list of links from generic list of strings.
    	/// </summary>
    	/// <param name="LinksList">List of pages links</param>
    	/// <returns>Typed list of links</returns>
    	public static List<PageLink> PackToTypedListFromListOfStrings(List<string> LinksList)
    	{
    		List<PageLink> list = new List<PageLink>(LinksList.Count);
    		foreach (string Links in LinksList)
    		{
    			list.Add(new PageLink(Links));
    		}
    		return list;
    	}

    	/// <summary>
    	/// Returns simple list of links from typed list of links
    	/// </summary>
    	/// <param name="PageLinks"></param>
    	/// <returns></returns>
    	public static List<string> GetOriginalLinksFromTypedListOfLinks(List<PageLink> PageLinks)
    	{
    		List<string> list = new List<string>();
    		for (int i = 0; i < PageLinks.Count; i++)
    		{
    			list.Add(PageLinks[i].NormalizedOriginalLink);
    		}
    		return list;
    	}

    	/// <summary>
    	/// Clones object
    	/// </summary>
    	/// <returns></returns>
    	public object Clone()
    	{
    		PageLink pageLink = new PageLink(OriginalLink);
    		pageLink.OriginalLinkLowercase = OriginalLinkLowercase;
    		pageLink.NormalizedOriginalLink = NormalizedOriginalLink;
    		pageLink.LinkOwnerType = LinkOwnerType;
    		pageLink.LinkResourceType = LinkResourceType;
    		pageLink.LinkLastCrawlingDateAndTime = LinkLastCrawlingDateAndTime;
    		pageLink.LinkLastCrawlingSuccessDateTime = LinkLastCrawlingSuccessDateTime;
    		pageLink.IsLinkResolvedAndWellFormattedToAbsolutePathUrl = IsLinkResolvedAndWellFormattedToAbsolutePathUrl;
    		pageLink.LinkResourceExtension = LinkResourceExtension;
    		pageLink.PageAtRecrawlingProcess = PageAtRecrawlingProcess;
    		return pageLink;
    	}

    	/// <summary>
    	/// Returns assembled link with behavior postfix, if it specified. Used for logging and data presenting.
    	/// </summary>
    	/// <returns>Assembled link with behavior postfix</returns>
    	internal string GetNormalizedOriginalLinkWithBehaviorPostfix()
    	{
    		if (DEBehaviorLinkPoxtfix == null || DEBehaviorLinkPoxtfix == string.Empty)
    		{
    			return NormalizedOriginalLink;
    		}
    		if (NormalizedOriginalLink == null || NormalizedOriginalLink == string.Empty)
    		{
    			return string.Empty;
    		}
    		if (NormalizedOriginalLink.IndexOf('#') != -1)
    		{
    			return $"{NormalizedOriginalLink}&{DEBehaviorLinkPoxtfix}";
    		}
    		return $"{NormalizedOriginalLink}#{DEBehaviorLinkPoxtfix}";
    	}

    	/// <summary>
    	/// Serialize PageLink to JSON format
    	/// </summary>
    	/// <returns></returns>
    	public string SerializeToJSON()
    	{
    		return JsonConvert.SerializeObject((object)this, (Formatting)1);
    	}

    	/// <summary>
    	/// Unserialize PageLink from JSON format
    	/// </summary>
    	/// <param name="JSONData"></param>
    	/// <returns></returns>
    	public PageLink UnserializeFromJSON(string JSONData)
    	{
    		return JsonConvert.DeserializeObject<PageLink>(JSONData);
    	}
    }
}
