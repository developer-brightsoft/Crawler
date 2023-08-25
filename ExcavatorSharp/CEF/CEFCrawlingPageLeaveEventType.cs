// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.CEF.CEFCrawlingPageLeaveEventType
using System.ComponentModel;

/// <summary>
/// CEF behavior for page leaving
/// </summary>
namespace ExcavatorSharp.CEF
{
	public enum CEFCrawlingPageLeaveEventType
	{
		/// <summary>
		/// Leave page after it was indexed
		/// </summary>
		[Description("Execute behavior (JS + iddling) and leave page")]
		LeavePageAfterIndexing,
		/// <summary>
		/// Leave crawling page after N seconds
		/// </summary>
		[Description("Re-crawl page until some time will be iddled")]
		LeavePageAfterSomeTimeSpentInSeconds,
		/// <summary>
		/// Leave page after some JS function returns some value
		/// </summary>
		[Description("Re-crawl page in cycle until JS-script-2 returns some result")]
		LeavePageAfterJSEventReturnsSomeResult,
		/// <summary>
		/// Leave page when we got N links from page
		/// </summary>
		[Description("Re-crawl page until N links will be fetched from the page")]
		LeavePageAfterNLinksParsed,
		/// <summary>
		/// Leave page when no new links parsed from HTML content
		/// </summary>
		[Description("Re-crawl page until no new links will be found")]
		LeavePageAfterNoNewLinksParsed
	}

}
