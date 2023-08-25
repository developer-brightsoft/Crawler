// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Crawler.CrawlingServerStatus
/// <summary>
/// Statuses of the crawling server
/// </summary>
namespace ExcavatorSharp.Crawler
{
	public enum CrawlingServerStatus
	{
		/// <summary>
		/// Waiting for commands
		/// </summary>
		Waiting,
		/// <summary>
		/// Crawling in process
		/// </summary>
		Crawling,
		/// <summary>
		/// Stopping service
		/// </summary>
		Stopping
	}
}