// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DataExcavatorTaskActualMetric
/// <summary>
/// Metric for observing actual task counters
/// </summary>
namespace ExcavatorSharp.Objects
{
	public class DataExcavatorTaskActualMetric
	{
		/// <summary>
		/// Size of total crawled pages at this moment
		/// </summary>
		public int TotalCrawledPagesCount { get; set; }

		/// <summary>
		/// Lenght of pages to crawl queue
		/// </summary>
		public int PagesToCrawlQueueLength { get; set; }

		/// <summary>
		/// Session errors count thru the crawling
		/// </summary>
		public int SessionCrawlingErrorsCount { get; set; }

		/// <summary>
		/// Total grabbed pages count
		/// </summary>
		public int TotalGrabbedPagesCount { get; set; }

		/// <summary>
		/// Pages to grab queue length
		/// </summary>
		public int PagesToGrabQueueLengh { get; set; }

		/// <summary>
		/// Non-empty results grabbed pages count
		/// </summary>
		public int NonEmptyResultsGrabbedPagesCount { get; set; }

		/// <summary>
		/// Total count of binary files in grabbed results
		/// </summary>
		public int GrabbedBinaryFilesCount { get; set; }

		/// <summary>
		/// Total size of grabbed data
		/// </summary>
		public int GrabbedDataTotalSizeKb { get; set; }

		/// <summary>
		/// Average website response time in milliseconds
		/// </summary>
		public double SessionWebsiteResponseAverageSpeedSeconds { get; set; }
	}
}