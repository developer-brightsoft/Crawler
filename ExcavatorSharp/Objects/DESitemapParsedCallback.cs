// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DESitemapParsedCallback
using ExcavatorSharp.Crawler;

/// <summary>
/// Website sitemap parsed callback
/// </summary>
namespace ExcavatorSharp.Objects
{
	public class DESitemapParsedCallback
	{
		/// <summary>
		/// Link to parsed sitemap data
		/// </summary>
		public DESitemapFile SitemapData { get; set; }

		/// <summary>
		/// Is file parsed ok
		/// </summary>
		public bool IsParsedSuccessfully { get; set; }

		/// <summary>
		/// Results of sitemap parsing or information comment
		/// </summary>
		public string ParsingResultsInformation { get; set; }

		/// <summary>
		/// Creates new instance of DESitemapParsedCallback
		/// </summary>
		/// <param name="SitemapData">Link to parsed sitemap data</param>
		/// <param name="IsParsedSuccessfully">Is file parsed ok</param>
		/// <param name="ParsingResultsInformation">Results of sitemap parsing or information comment</param>
		public DESitemapParsedCallback(DESitemapFile SitemapData, bool IsParsedSuccessfully, string ParsingResultsInformation)
		{
			this.SitemapData = SitemapData;
			this.IsParsedSuccessfully = IsParsedSuccessfully;
			this.ParsingResultsInformation = ParsingResultsInformation;
		}
	}
}