// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DataCrawlingType
using System.ComponentModel;

/// <summary>
/// Primary technology to crawl data. By default, we recommend to use NativeCrawling, because most of websites uses data placing without async technologies.
/// </summary>
namespace ExcavatorSharp.Objects
{
	public enum DataCrawlingType
	{
		/// <summary>
		/// Crawl data using .NET HttpWebRequest (.NET Sockets). Fast way, but can crawl only sync native-placed HTML data (no JS, Ajax and other async data)
		/// </summary>
		[Description("Crawl data using .NET HttpWebRequest (.NET Sockets)")]
		NativeCrawling,
		/// <summary>
		/// Crawl data using CEF (Chromium Embedded Framework). Slow way, but can crawl any data, includes JS, Ajax and other data, showed after page load.
		/// </summary>
		[Description("Crawl data using CEF (Chromium Embedded Framework)")]
		CEFCrawling
	}
}
