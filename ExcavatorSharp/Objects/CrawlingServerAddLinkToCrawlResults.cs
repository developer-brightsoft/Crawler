// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.CrawlingServerAddLinkToCrawlResults
using System.Collections.Generic;

namespace ExcavatorSharp.Objects
{
    /// <summary>
    /// Results for adding links to crawling handly
    /// </summary>
    public class CrawlingServerAddLinkToCrawlResults
    {
    	/// <summary>
    	/// Count of added links
    	/// </summary>
    	public int AddedLinksCount { get; set; }

    	/// <summary>
    	/// Count of skipped links
    	/// </summary>
    	public int SkippedLinksCount { get; set; }

    	/// <summary>
    	/// Method logs
    	/// </summary>
    	public List<CrawlingServerAddLinkToCrawlingResult> LinksAddingLogs { get; set; }

    	/// <summary>
    	/// Creates new empty instance of CrawlingServerAddLinkToCrawlResults
    	/// </summary>
    	public CrawlingServerAddLinkToCrawlResults()
    	{
    		AddedLinksCount = 0;
    		SkippedLinksCount = 0;
    		LinksAddingLogs = new List<CrawlingServerAddLinkToCrawlingResult>();
    	}
    }
}
