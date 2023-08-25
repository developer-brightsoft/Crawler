// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DataExcavatorPageLinksObservingResult
using System.Collections.Generic;

namespace ExcavatorSharp.Objects
{
    /// <summary>
    /// Result of some page observing
    /// </summary>
    public class DataExcavatorPageLinksObservingResult
    {
    	/// <summary>
    	/// Page observing logs
    	/// </summary>
    	public List<string> PageObservingLogs = new List<string>();

    	/// <summary>
    	/// Success of page links obvserving
    	/// </summary>
    	public bool Success { get; set; }

    	/// <summary>
    	/// List of page links
    	/// </summary>
    	public List<PageLink> PageLinks
    	{
    		get
    		{
    			if (PageCrawledResponseData != null && PageCrawledResponseData.PageLinks != null)
    			{
    				return PageCrawledResponseData.PageLinks;
    			}
    			return new List<PageLink>();
    		}
    	}

    	/// <summary>
    	/// Page crawling information
    	/// </summary>
    	public PageCrawledCallback PageCrawledResponseData { get; set; }
    }
}
