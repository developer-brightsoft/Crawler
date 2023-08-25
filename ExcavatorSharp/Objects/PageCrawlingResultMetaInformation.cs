// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.PageCrawlingResultMetaInformation
using System;
using System.Net;

/// <summary>
/// Page crawling results information
/// </summary>
namespace ExcavatorSharp.Objects
{
	public class PageCrawlingResultMetaInformation
	{
		/// <summary>
		/// Date and time when page was downloaded
		/// </summary>
		public DateTime PageCrawledDateTime { get; private set; }

		/// <summary>
		/// The time interval from the start date of loading the page to the end date of loading the page.
		/// </summary>
		public TimeSpan PageCrawlingTime { get; private set; }

		/// <summary>
		/// HTTP response status code
		/// </summary>
		public HttpStatusCode ResponseStatusCode { get; private set; }

		/// <summary>
		/// A number of the attempt in which the page was loaded, from 1 to N
		/// </summary>
		public int PageDownloadAttemptNumber { get; private set; }

		/// <summary>
		/// An exception that may have occurred during the page loading
		/// </summary>
		public Exception CrawledPageThrownException { get; private set; }

		public PageCrawlingResultMetaInformation(DateTime PageCrawledDateTime, TimeSpan PageCrawlingTime, HttpStatusCode ResponseStatusCode, int PageDownloadAttemptNumber, Exception CrawledPageThrownException = null)
		{
			this.PageCrawledDateTime = PageCrawledDateTime;
			this.PageCrawlingTime = PageCrawlingTime;
			this.ResponseStatusCode = ResponseStatusCode;
			this.PageDownloadAttemptNumber = PageDownloadAttemptNumber;
			this.CrawledPageThrownException = CrawledPageThrownException;
		}
	}
}
