// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Crawler.DERobotsEntryType
/// <summary>
/// Robots.txt entries types
/// </summary>
namespace ExcavatorSharp.Crawler
{
	public enum DERobotsEntryType
	{
		/// <summary>
		/// Allows URLs to index
		/// </summary>
		Allow,
		/// <summary>
		/// Disallows URLs to index
		/// </summary>
		Disallow,
		/// <summary>
		/// Link to sitemap file
		/// </summary>
		Sitemap,
		/// <summary>
		/// Clean params from URLs
		/// </summary>
		CleanParam,
		/// <summary>
		/// Crawling delay
		/// </summary>
		CrawlDelay
	}
}
