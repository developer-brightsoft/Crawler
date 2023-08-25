// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Crawler.DESitemapContentEntry
using System;

namespace ExcavatorSharp.Crawler
{
    /// <summary>
    /// Entry for storing some page location data
    /// </summary>
    public class DESitemapContentEntry
    {
    	/// <summary>
    	/// Sitemap row type
    	/// </summary>
    	public DESitemapContentEntryType LinkType { get; set; }

    	/// <summary>
    	/// Sitemap location address
    	/// </summary>
    	public string Location { get; set; }

    	/// <summary>
    	/// Sitemap last modify date
    	/// </summary>
    	public DateTime? LastMod { get; set; }

    	/// <summary>
    	/// Resource changes frequency
    	/// </summary>
    	public string ChangeFreq { get; set; }

    	/// <summary>
    	/// Resource priority
    	/// </summary>
    	public double Priority { get; set; }
    }
}
