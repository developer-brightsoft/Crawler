// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Parser.GrabbingServer
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using ExcavatorSharp.Crawler;
using ExcavatorSharp.Excavator;
using ExcavatorSharp.Exporter;
using ExcavatorSharp.Grabber;
using ExcavatorSharp.Licensing;
using ExcavatorSharp.Objects;

namespace ExcavatorSharp.Parser
{
    /// <summary>
    /// Main class for data grabbing
    /// </summary>
    public class GrabbingServer
    {
    	/// <summary>
    	/// Delegate for PageGrabbed event
    	/// </summary>
    	/// <param name="GrabbedPageData">Grabbed page data</param>
    	public delegate void PageGrabbedHandler(PageGrabbedCallback GrabbedPageData);

    	/// <summary>
    	/// Grabbed pages total count
    	/// </summary>
    	internal volatile int MetricsTotalGrabbedPagesCount;

    	/// <summary>
    	/// Non-empty results total count
    	/// </summary>
    	internal volatile int MetricsNonEmptyResultsGrabbedPagesCount;

    	/// <summary>
    	/// Total count of grabbed files
    	/// </summary>
    	internal volatile int MetricsGrabbedBinaryFilesCount;

    	/// <summary>
    	/// Total size of grabbed data in Kb
    	/// </summary>
    	internal volatile int MetricsGrabbedDataTotalSizeKb;

    	/// <summary>
    	/// Name of parent task
    	/// </summary>
    	public string ParentTaskName { get; private set; }

    	/// <summary>
    	/// Target directory for data saving
    	/// </summary>
    	public string ParentTaskOperatingDirectory { get; private set; }

    	/// <summary>
    	/// Actual server status
    	/// </summary>
    	public GrabbingServerStatus ActualServerStatus { get; private set; }

    	/// <summary>
    	/// Crawling website base url, defining at constructing of class
    	/// </summary>
    	public Uri WebsiteBaseUrl { get; private set; }

    	/// <summary>
    	/// Grabbing properties
    	/// </summary>
    	public GrabbingServerProperties GrabberProperties { get; private set; }

    	/// <summary>
    	/// Actual grabbing patterns
    	/// </summary>
    	public List<DataGrabbingPattern> GrabbingPatterns { get; private set; }

    	/// <summary>
    	/// Link to TaskLogger, hosted at DataExcavatorTask
    	/// </summary>
    	internal DataExcavatorTasksLogger TaskLoggerLink { get; set; }

    	/// <summary>
    	/// Threads for pages crawling
    	/// </summary>
    	private List<GrabbingThread> PageGrabbingThreads { get; set; }

    	/// <summary>
    	/// Grabbig items queue
    	/// </summary>
    	public ConcurrentQueue<DataGrabbingTask> GrabbingQueue { get; private set; }

    	/// <summary>
    	/// Link to crawling server
    	/// </summary>
    	public CrawlingServer CrawlerLink { get; set; }

    	/// <summary>
    	/// Mutex for some IO operations
    	/// </summary>
    	internal Mutex CommonMetaDataSavingMutex { get; private set; }

    	/// <summary>
    	/// Patterns IO mutex
    	/// </summary>
    	internal Mutex PatternsIOMutex { get; private set; }

    	/// <summary>
    	/// Is grabber initialization prevented by some reason
    	/// </summary>
    	public bool IsGrabberInitializationPrevented { get; private set; }

    	/// <summary>
    	/// The main event occurring at the completion of data grabbing from the page
    	/// </summary>
    	public event PageGrabbedHandler PageGrabbed;

    	/// <summary>
    	/// Creates a new instanse of Grabbing Server
    	/// </summary>
    	/// <param name="WebsiteUrl">Crawling website base url, defining at constructing of class</param>
    	/// <param name="GrabberProperties">Grabbing properties</param>
    	/// <param name="GrabbingPatterns">Actual grabbing patterns</param>
    	/// <param name="ParentTaskOperatingDirectory">Target directory for data saving</param>
    	/// <param name="ParentTaskName">Name of the parent task</param>
    	/// <param name="TaskLoggerLink">Link to parent task logger</param>
    	/// <param name="CrawlerLink">Link to crawling server. Can be null, if you don't want to parse images and other resources, founded into pages.</param>
    	public GrabbingServer(string WebsiteUrl, GrabbingServerProperties GrabberProperties, DataExcavatorTasksLogger TaskLoggerLink, List<DataGrabbingPattern> GrabbingPatterns, string ParentTaskName, string ParentTaskOperatingDirectory, CrawlingServer CrawlerLink = null)
    	{
    		if (GrabberProperties.GrabbingThreadsCount == -1)
    		{
    			PageGrabbingThreads = new List<GrabbingThread>();
    			this.GrabbingPatterns = new List<DataGrabbingPattern>();
    			this.GrabberProperties = new GrabbingServerProperties();
    			IsGrabberInitializationPrevented = true;
    			return;
    		}
    		IsGrabberInitializationPrevented = false;
    		Uri uri = new Uri(WebsiteUrl);
    		WebsiteBaseUrl = new Uri($"{uri.Scheme.ToLower()}://{uri.Authority.ToLower()}");
    		GrabbingQueue = new ConcurrentQueue<DataGrabbingTask>();
    		this.TaskLoggerLink = TaskLoggerLink;
    		this.GrabbingPatterns = GrabbingPatterns;
    		this.GrabberProperties = GrabberProperties;
    		this.CrawlerLink = CrawlerLink;
    		PageGrabbingThreads = new List<GrabbingThread>(GrabberProperties.GrabbingThreadsCount);
    		this.ParentTaskName = ParentTaskName;
    		this.ParentTaskOperatingDirectory = ParentTaskOperatingDirectory;
    		CommonMetaDataSavingMutex = new Mutex();
    		PatternsIOMutex = new Mutex();
    		for (int i = 0; i < this.GrabberProperties.GrabbingThreadsCount; i++)
    		{
    			GrabbingThread grabbingThread = new GrabbingThread(this);
    			PageGrabbingThreads.Add(grabbingThread);
    			grabbingThread.PageGrabbed += NewGrabbingThread_PageGrabbed;
    		}
    		if (this.GrabberProperties.StoreGrabbedData)
    		{
    			LoadInitialMetrics();
    		}
    	}

    	/// <summary>
    	/// Event that happens when one of children threads grabs some page. 
    	/// </summary>
    	/// <param name="GrabbedPageData"></param>
    	private void NewGrabbingThread_PageGrabbed(PageGrabbedCallback GrabbedPageData)
    	{
    		ExecuteChildGrabbedPage(GrabbedPageData);
    	}

    	/// <summary>
    	/// Executes grabbed page from one of the child thread
    	/// </summary>
    	/// <param name="GrabbedPageData">Data of the grabbed page</param>
    	private void ExecuteChildGrabbedPage(PageGrabbedCallback GrabbedPageData)
    	{
    		GrabbedDataGroupContainer grabbedDataGroupContainer = null;
    		if (GrabberProperties.StoreGrabbedData && GrabbedPageData.MatchedPatternsCount > 0)
    		{
    			GrabbingDataIO grabbingDataIO = new GrabbingDataIO(this);
    			grabbingDataIO.SaveDataPatternsWithVersionControl(GrabbingPatterns);
    			try
    			{
    				grabbedDataGroupContainer = grabbingDataIO.SaveGrabbedDataInOriginalFormat(GrabbedPageData);
    			}
    			catch (Exception occuredException)
    			{
    				TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.GrabbingServer, $"[GSE6] An error occurred while saving the data", occuredException);
    			}
    		}
    		if (GrabberProperties.ExportDataOnline && !string.IsNullOrEmpty(GrabberProperties.ExportDataOnlineInvokationLink) && GrabbedPageData.MatchedPatternsCount > 0)
    		{
    			if (grabbedDataGroupContainer == null)
    			{
    				GrabbingDataIO grabbingDataIO2 = new GrabbingDataIO(this);
    				grabbedDataGroupContainer = grabbingDataIO2.ConvertPageGrabbedCallbackToGrabbedDataGroup(GrabbedPageData, SaveBinaryFiles: false, string.Empty);
    				grabbedDataGroupContainer.UpdateDataGroupMetrics();
    			}
    			HTTPExporter hTTPExporter = new HTTPExporter(ParentTaskName, GrabberProperties.ExportDataOnlineInvokationLink, TaskLoggerLink);
    			hTTPExporter.ExportDataViaHTTP(grabbedDataGroupContainer);
    		}
    		if (grabbedDataGroupContainer == null)
    		{
    			GrabbingDataIO grabbingDataIO3 = new GrabbingDataIO(this);
    			grabbedDataGroupContainer = grabbingDataIO3.ConvertPageGrabbedCallbackToGrabbedDataGroup(GrabbedPageData, SaveBinaryFiles: false, string.Empty);
    			grabbedDataGroupContainer.UpdateDataGroupMetrics();
    		}
    		if (GrabbedPageData.MatchedPatternsCount > 0)
    		{
    			Interlocked.Increment(ref MetricsTotalGrabbedPagesCount);
    			bool flag = false;
    			foreach (KeyValuePair<DataGrabbingPattern, DataGrabbingResult> grabbingResult in GrabbedPageData.GrabbingResults)
    			{
    				if (!grabbingResult.Value.IsEmptyResultsSet)
    				{
    					flag = true;
    					break;
    				}
    			}
    			if (flag)
    			{
    				Interlocked.Increment(ref MetricsNonEmptyResultsGrabbedPagesCount);
    			}
    			if (grabbedDataGroupContainer.DataGroupMetrics.GrabbedDataTotalSizeKb > 0.0)
    			{
    				int value = Convert.ToInt32(grabbedDataGroupContainer.DataGroupMetrics.GrabbedDataTotalSizeKb);
    				Interlocked.Add(ref MetricsGrabbedDataTotalSizeKb, value);
    			}
    			if (grabbedDataGroupContainer.DataGroupMetrics.BinaryFilesCount > 0)
    			{
    				Interlocked.Add(ref MetricsGrabbedBinaryFilesCount, grabbedDataGroupContainer.DataGroupMetrics.BinaryFilesCount);
    			}
    		}
    		this.PageGrabbed?.Invoke(GrabbedPageData);
    	}

    	/// <summary>
    	/// Adds data to grabbing queue
    	/// </summary>
    	/// <param name="GrabbedPageCallbackData"></param>
    	/// <param name="GrabbingPattern"></param>
    	public void AddDataToGrabbing(PageCrawledCallback GrabbedPageCallbackData)
    	{
    		GrabbingQueue.Enqueue(new DataGrabbingTask(GrabbedPageCallbackData));
    	}

    	/// <summary>
    	/// Starts grabbing
    	/// </summary>
    	public void StartGrabbing()
    	{
    		if (!IsGrabberInitializationPrevented)
    		{
    			if (!LicenseServer.CheckLicenseKeyValid())
    			{
    				throw LicenseValidationException.FromLicenseValidationResponse();
    			}
    			if (LicenseServer.ActualKey.KeyTotalThreadsLimitPerProject != -1 && GrabberProperties.GrabbingThreadsCount > LicenseServer.ActualKey.KeyTotalThreadsLimitPerProject)
    			{
    				throw LicenseValidationException.FromGrabbingServerIfWrongThreadsCountRequested();
    			}
    			for (int i = 0; i < PageGrabbingThreads.Count; i++)
    			{
    				PageGrabbingThreads[i].StartGrabbing();
    			}
    		}
    	}

    	/// <summary>
    	/// Stops grabbing
    	/// </summary>
    	public void StopGrabbing()
    	{
    		if (!IsGrabberInitializationPrevented)
    		{
    			for (int i = 0; i < PageGrabbingThreads.Count; i++)
    			{
    				PageGrabbingThreads[i].StopGrabbing();
    			}
    		}
    	}

    	/// <summary>
    	/// Loads initial metrics
    	/// </summary>
    	internal void LoadInitialMetrics()
    	{
    		Interlocked.Exchange(ref MetricsTotalGrabbedPagesCount, 0);
    		Interlocked.Exchange(ref MetricsNonEmptyResultsGrabbedPagesCount, 0);
    		Interlocked.Exchange(ref MetricsGrabbedBinaryFilesCount, 0);
    		Interlocked.Exchange(ref MetricsGrabbedDataTotalSizeKb, 0);
    		GrabbingDataIO grabbingDataIO = new GrabbingDataIO(this);
    		List<GrabbedPageMetaInformationDataEntry> actualGrabbedPagesMeta = grabbingDataIO.GetActualGrabbedPagesMeta();
    		Interlocked.Add(ref MetricsTotalGrabbedPagesCount, actualGrabbedPagesMeta.Count);
    		int num = 0;
    		int num2 = 0;
    		int num3 = 0;
    		foreach (GrabbedPageMetaInformationDataEntry item in actualGrabbedPagesMeta)
    		{
    			if (item.HasResults)
    			{
    				num++;
    				num2 += item.BinaryFilesCount;
    				num3 += Convert.ToInt32(item.DataSizeKb);
    			}
    		}
    		Interlocked.Add(ref MetricsNonEmptyResultsGrabbedPagesCount, num);
    		Interlocked.Add(ref MetricsGrabbedBinaryFilesCount, num2);
    		Interlocked.Add(ref MetricsGrabbedDataTotalSizeKb, num3);
    	}
    }
}
