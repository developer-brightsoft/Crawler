// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.CrawlingServerAddLinkToCrawlingResult
/// <summary>
/// Information about the results of adding a certain link to the crawling queue
/// </summary>

namespace ExcavatorSharp.Objects
{
	public class CrawlingServerAddLinkToCrawlingResult
	{
		/// <summary>
		/// Original link address
		/// </summary>
		public string Link { get; set; }

		/// <summary>
		/// Is link added to crawling
		/// </summary>
		public bool IsLinkAddedToCrawling { get; set; }

		/// <summary>
		/// Link adding results
		/// </summary>
		public string LinkAddingResultMessage { get; set; }

		/// <summary>
		/// Creates a new instance of CrawlingServerAddLinkToCrawlingResult
		/// </summary>
		/// <param name="Link">Original link address</param>
		/// <param name="IsLinkAddedToCrawling">Is link added to crawling</param>
		/// <param name="LinkAddingResultMessage">Link adding results</param>
		public CrawlingServerAddLinkToCrawlingResult(string Link, bool IsLinkAddedToCrawling, string LinkAddingResultMessage)
		{
			this.Link = Link;
			this.IsLinkAddedToCrawling = IsLinkAddedToCrawling;
			this.LinkAddingResultMessage = LinkAddingResultMessage;
		}
	}
}