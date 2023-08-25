// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.CrawlingServerLinksBuffer
using System;
using System.Collections.Concurrent;
using ExcavatorSharp.Interfaces;
using Newtonsoft.Json;

namespace ExcavatorSharp.Objects
{
    /// <summary>
    /// Buffer for storing links to crawl and crawled links.
    /// </summary>
    public class CrawlingServerLinksBuffer : ICloneable, IJSONConvertible<CrawlingServerLinksBuffer>
    {
    	/// <summary>
    	/// A set of links to be crawl
    	/// </summary>
    	public ConcurrentQueue<PageLink> LinksToCrawl { get; set; }

    	/// <summary>
    	/// A set of already crawled links
    	/// </summary>
    	public ConcurrentBag<PageLink> CrawledLinks { get; set; }

    	/// <summary>
    	/// Creates a new instance of CrawlingServerLinksBuffer
    	/// </summary>
    	public CrawlingServerLinksBuffer()
    	{
    		LinksToCrawl = new ConcurrentQueue<PageLink>();
    		CrawledLinks = new ConcurrentBag<PageLink>();
    	}

    	/// <summary>
    	/// Creates a new instance of CrawlingServerLinksBuffer
    	/// </summary>
    	/// <param name="LinksToCrawl"></param>
    	/// <param name="CrawledLinks"></param>
    	public CrawlingServerLinksBuffer(ConcurrentQueue<PageLink> LinksToCrawl, ConcurrentBag<PageLink> CrawledLinks)
    	{
    		this.LinksToCrawl = LinksToCrawl;
    		this.CrawledLinks = CrawledLinks;
    	}

    	/// <summary>
    	/// Clones CrawlingServerLinksBuffer object
    	/// </summary>
    	/// <returns>Clone of CrawlingServerLinksBuffer</returns>
    	public object Clone()
    	{
    		return new CrawlingServerLinksBuffer
    		{
    			LinksToCrawl = new ConcurrentQueue<PageLink>(LinksToCrawl),
    			CrawledLinks = new ConcurrentBag<PageLink>(CrawledLinks)
    		};
    	}

    	/// <summary>
    	/// Serializes links buffer to JSON array
    	/// </summary>
    	/// <returns>Serialized links buffer</returns>
    	public string SerializeToJSON()
    	{
    		return JsonConvert.SerializeObject((object)this, (Formatting)1);
    	}

    	/// <summary>
    	/// Unserializes links buffer from JSON to new instance of CrawlingServerLinksBuffer
    	/// </summary>
    	/// <param name="JSONData"></param>
    	/// <returns></returns>
    	public CrawlingServerLinksBuffer UnserializeFromJSON(string JSONData)
    	{
    		return JsonConvert.DeserializeObject<CrawlingServerLinksBuffer>(JSONData);
    	}
    }
}
