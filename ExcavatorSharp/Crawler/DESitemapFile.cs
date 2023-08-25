// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Crawler.DESitemapFile
using System;
using System.Collections.Generic;
using System.Linq;
using ExcavatorSharp.Interfaces;
using Newtonsoft.Json;

namespace ExcavatorSharp.Crawler
{
    /// <summary>
    /// Class for storing data from some sitemap
    /// </summary>
    public class DESitemapFile : IJSONConvertible<DESitemapFile>
    {
    	/// <summary>
    	/// List of content entries
    	/// </summary>
    	public List<DESitemapContentEntry> ContentEntries = new List<DESitemapContentEntry>();

    	/// <summary>
    	/// List of child sitemaps, if defined
    	/// </summary>
    	public List<DESitemapFile> ChildSitemaps = new List<DESitemapFile>();

    	/// <summary>
    	/// Link to sitemap location
    	/// </summary>
    	public string SitemapLocation { get; set; }

    	/// <summary>
    	/// Is sitemap are sitemap-index file
    	/// </summary>
    	[JsonIgnore]
    	public bool IsIndexFile => ChildSitemaps.Count > 0;

    	/// <summary>
    	/// Date and time of last sitemap reindexation
    	/// </summary>
    	public DateTime LastReindexDateTime { get; set; }

    	public List<string> GetPagesLinks(string[] AllowedUrlsMask = null, string[] DisallowedUrlsMasks = null, int ParallelismDegree = 1)
    	{
    		List<string> list = new List<string>();
    		if (IsIndexFile)
    		{
    			foreach (DESitemapFile childSitemap in ChildSitemaps)
    			{
    				list.AddRange(childSitemap.GetPagesLinks(AllowedUrlsMask, DisallowedUrlsMasks, ParallelismDegree));
    			}
    		}
    		else if (AllowedUrlsMask == null && DisallowedUrlsMasks == null)
    		{
    			list.AddRange(ContentEntries.Select((DESitemapContentEntry item) => item.Location).ToList());
    		}
    		else
    		{
    			list.AddRange((from item in ContentEntries.AsParallel().WithDegreeOfParallelism(ParallelismDegree).Where(delegate(DESitemapContentEntry item)
    				{
    					if (AllowedUrlsMask == null && DisallowedUrlsMasks == null)
    					{
    						return true;
    					}
    					if (DisallowedUrlsMasks != null)
    					{
    						string[] array = DisallowedUrlsMasks;
    						foreach (string value in array)
    						{
    							if (item.Location.IndexOf(value) != -1)
    							{
    								return false;
    							}
    						}
    					}
    					if (AllowedUrlsMask != null)
    					{
    						bool result = false;
    						string[] array2 = AllowedUrlsMask;
    						foreach (string value2 in array2)
    						{
    							if (item.Location.IndexOf(value2) != -1)
    							{
    								result = true;
    								break;
    							}
    						}
    						return result;
    					}
    					return true;
    				})
    				select item.Location).ToList());
    		}
    		return list;
    	}

    	/// <summary>
    	/// Calculate total count of sitemap links
    	/// </summary>
    	/// <returns></returns>
    	public int GetLinksCount()
    	{
    		int num = ContentEntries.Count;
    		for (int i = 0; i < ChildSitemaps.Count; i++)
    		{
    			num += ChildSitemaps[i].GetLinksCount();
    		}
    		return num;
    	}

    	/// <summary>
    	/// Serializes data into JSON string
    	/// </summary>
    	/// <returns>JSON string</returns>
    	public string SerializeToJSON()
    	{
    		return JsonConvert.SerializeObject((object)this, (Formatting)1);
    	}

    	/// <summary>
    	/// Creates new instance of DESitemapFile
    	/// </summary>
    	/// <param name="JSONData">JSON input data</param>
    	/// <returns>New instance of DESitemapFile</returns>
    	public DESitemapFile UnserializeFromJSON(string JSONData)
    	{
    		return JsonConvert.DeserializeObject<DESitemapFile>(JSONData);
    	}
    }
}
