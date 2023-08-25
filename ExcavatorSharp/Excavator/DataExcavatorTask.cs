// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Excavator.DataExcavatorTask
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using ExcavatorSharp.CEF;
using ExcavatorSharp.Common;
using ExcavatorSharp.Crawler;
using ExcavatorSharp.Exporter;
using ExcavatorSharp.Grabber;
using ExcavatorSharp.Licensing;
using ExcavatorSharp.Objects;
using ExcavatorSharp.Parser;

namespace ExcavatorSharp.Excavator
{
    /// <summary>
    /// The "Data Excavator Task" for data crawling and grabbing
    /// </summary>
    public class DataExcavatorTask
    {
    	/// <summary>
    	/// Delegate for PageGrabbed event
    	/// </summary>
    	/// <param name="GrabbedPageData">Grabbed page data</param>
    	public delegate void PageGrabbedHandler(PageGrabbedCallback GrabbedPageData);

    	/// <summary>
    	/// Delegate for PageCrawled event
    	/// </summary>
    	/// <param name="CrawledPageData">Crawled page data</param>
    	public delegate void PageCrawledHandler(PageCrawledCallback CrawledPageData);

    	/// <summary>
    	/// Delegate for LogMessageAdded event
    	/// </summary>
    	/// <param name="Callback"></param>
    	public delegate void LogEntryAdded(DataExcavatorTaskEventCallback Callback);

    	/// <summary>
    	/// Emulated mutes for Start/Stop task functions
    	/// </summary>
    	private bool StartStopTaskMutexEmulator = false;

    	/// <summary>
    	/// A name of the task
    	/// </summary>
    	public string TaskName { get; private set; }

    	/// <summary>
    	/// Target directory for data saving
    	/// </summary>
    	public string TaskOperatingDirectory { get; private set; }

    	/// <summary>
    	/// Website normalized root url address 
    	/// </summary>
    	public string WebsiteRootUrl { get; private set; }

    	/// <summary>
    	/// Website normalized root url without WWW
    	/// </summary>
    	private string WebsiteRootUrlWithoutWWW { get; set; }

    	/// <summary>
    	/// Description of task
    	/// </summary>
    	public string TaskDescription { get; private set; }

    	/// <summary>
    	/// Actual task state
    	/// </summary>
    	public DataExcavatorTaskState TaskState { get; private set; }

    	/// <summary>
    	/// Pattern for defining grabbing data
    	/// </summary>
    	private List<DataGrabbingPattern> GrabbingDataPatterns { get; set; }

    	/// <summary>
    	/// Scanning bot that downloads the site and scans it
    	/// </summary>
    	private CrawlingServer CrawlingBot { get; set; }

    	/// <summary>
    	/// Grabbing bot that analyses downloaded pages
    	/// </summary>
    	private GrabbingServer GrabbingBot { get; set; }

    	/// <summary>
    	/// Entitiy for logging events, messages and any data
    	/// </summary>
    	private DataExcavatorTasksLogger TaskLogger { get; set; }

    	/// <summary>
    	/// Specify logs HDD writing
    	/// </summary>
    	public bool FlushLogsToHDD
    	{
    		get
    		{
    			return TaskLogger.FlushLogsToHDD;
    		}
    		set
    		{
    			TaskLogger.FlushLogsToHDD = value;
    			if (value)
    			{
    				TaskLogger.StartLoggerHDDFlushing();
    			}
    			else
    			{
    				TaskLogger.StopLoggerHDDFlushing();
    			}
    		}
    	}

    	/// <summary>
    	/// Last date when task was started
    	/// </summary>
    	private DateTime TaskStartedLastDate { get; set; }

    	/// <summary>
    	/// Last date when task was stopped
    	/// </summary>
    	private DateTime TaskStoppedLastDate { get; set; }

    	/// <summary>
    	/// Container for task testing
    	/// </summary>
    	private DataExcavatorTaskTestingResult TaskTestingResultsContainer { get; set; }

    	/// <summary>
    	/// Page links observing container
    	/// </summary>
    	private DataExcavatorPageLinksObservingResult PageLinkObservingContainer { get; set; }

    	/// <summary>
    	/// Main event, that will happen after next page been grabbed.
    	/// </summary>
    	public event PageGrabbedHandler PageGrabbed;

    	/// <summary>
    	/// Main event, that will happen after next page been crawled.
    	/// </summary>
    	public event PageCrawledHandler PageCrawled;

    	/// <summary>
    	/// The event that will be triggered every time a new message is added.
    	/// </summary>
    	public event LogEntryAdded LogMessageAdded;

    	/// <summary>
    	/// Create new grabbing task
    	/// </summary>
    	/// <param name="GrabbingPatterns">Pattern for defining grabbing data</param>
    	/// <param name="CrawlingBotProperties">Scanning bot that downloads the site and scans it</param>
    	/// <param name="TaskOperatingDirectory">Target directory for data saving</param>
    	/// <param name="GrabbingBotProperties">Crawling server properties set</param>
    	/// <param name="TaskName">Name of the task</param>
    	/// <param name="WebsiteRootUrl">Target website full root url with sechema, like: http(s)://websitename.domain</param>
    	public DataExcavatorTask(string TaskName, string WebsiteRootUrl, string TaskDescription, List<DataGrabbingPattern> GrabbingPatterns, CrawlingServerProperties CrawlingBotProperties, GrabbingServerProperties GrabbingBotProperties, string TaskOperatingDirectory)
    	{
    		UpdateExcavatorTaskData(TaskName, WebsiteRootUrl, TaskDescription, GrabbingPatterns, CrawlingBotProperties, GrabbingBotProperties, TaskOperatingDirectory);
    		FlushLogsToHDD = true;
    	}

    	/// <summary>
    	/// Update task data
    	/// </summary>
    	/// <param name="GrabbingPatterns">Pattern for defining grabbing data</param>
    	/// <param name="CrawlingBotProperties">Scanning bot that downloads the site and scans it</param>
    	/// <param name="TaskOperatingDirectory">Target directory for data saving</param>
    	/// <param name="GrabbingBotProperties">Crawling server properties set</param>
    	/// <param name="TaskName">Name of the task</param>
    	/// <param name="WebsiteRootUrl">Target website full root url with sechema, like: http(s)://websitename.domain</param>
    	public void UpdateExcavatorTaskData(string TaskName, string WebsiteRootUrl, string TaskDescription, List<DataGrabbingPattern> GrabbingPatterns, CrawlingServerProperties CrawlingBotProperties, GrabbingServerProperties GrabbingBotProperties, string TaskOperatingDirectory)
    	{
    		TaskState = DataExcavatorTaskState.Stopped;
    		this.TaskName = TaskName;
    		this.TaskDescription = TaskDescription;
    		this.TaskOperatingDirectory = TaskOperatingDirectory;
    		this.WebsiteRootUrl = WebsiteRootUrl;
    		WebsiteRootUrlWithoutWWW = this.WebsiteRootUrl.Replace("www.", string.Empty);
    		GrabbingDataPatterns = GrabbingPatterns;
    		TaskLogger = new DataExcavatorTasksLogger(this.TaskOperatingDirectory);
    		TaskLogger.AddLogSessionInitialEntry();
    		CrawlingBot = new CrawlingServer(WebsiteRootUrl, CrawlingBotProperties, TaskLogger, this.TaskName, this.TaskOperatingDirectory);
    		GrabbingBot = new GrabbingServer(WebsiteRootUrl, GrabbingBotProperties, TaskLogger, GrabbingDataPatterns, this.TaskName, this.TaskOperatingDirectory, CrawlingBot);
    		CrawlingBot.PageCrawled += CrawlingBot_PageCrawled;
    		GrabbingBot.PageGrabbed += GrabbingBot_PageGrabbed;
    		TaskLogger.LogMessageAdded += TaskLogger_LogMessageAdded;
    	}

    	/// <summary>
    	/// Some log entry added
    	/// </summary>
    	/// <param name="Callback"></param>
    	private void TaskLogger_LogMessageAdded(DataExcavatorTaskEventCallback Callback)
    	{
    		this.LogMessageAdded?.Invoke(Callback);
    	}

    	/// <summary>
    	/// The robot makes page grabbing
    	/// </summary>
    	/// <param name="GrabbedPageData">Grabbed page data callback</param>
    	private void GrabbingBot_PageGrabbed(PageGrabbedCallback GrabbedPageData)
    	{
    		bool flag = false;
    		if (GrabbedPageData.GrabbingResults.Count > 0)
    		{
    			foreach (KeyValuePair<DataGrabbingPattern, DataGrabbingResult> grabbingResult in GrabbedPageData.GrabbingResults)
    			{
    				if (!grabbingResult.Value.IsEmptyResultsSet)
    				{
    					flag = true;
    					break;
    				}
    			}
    		}
    		TaskLogger.Log(DataExcavatorTasksLoggerEntityType.GrabbingServer, string.Format("Page scraped. URL = {0}, Has results = {1}", GrabbedPageData.GrabbingTask.PageCrawledCallbackData.DownloadedPageUrl.GetNormalizedOriginalLinkWithBehaviorPostfix(), flag ? "yes" : "no"));
    		this.PageGrabbed?.Invoke(GrabbedPageData);
    	}

    	/// <summary>
    	/// The robot has performed the download and initial analysis of a certain page.
    	/// </summary>
    	/// <param name="CrawledPageData">Crawled page callback</param>
    	private void CrawlingBot_PageCrawled(PageCrawledCallback CrawledPageData)
    	{
    		if (CrawledPageData.CrawledPageMeta.ResponseStatusCode == HttpStatusCode.OK)
    		{
    			TaskLogger.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Page crawled in {CrawledPageData.CrawledPageMeta.PageCrawlingTime} seconds, URL = {CrawledPageData.DownloadedPageUrl.GetNormalizedOriginalLinkWithBehaviorPostfix()}");
    		}
    		else
    		{
    			TaskLogger.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Page not crawled, URL = {CrawledPageData.DownloadedPageUrl.GetNormalizedOriginalLinkWithBehaviorPostfix()}, attempts = {CrawledPageData.CrawledPageMeta.PageDownloadAttemptNumber}, last http status code = {CrawledPageData.CrawledPageMeta.ResponseStatusCode.ToString()}");
    		}
    		this.PageCrawled?.Invoke(CrawledPageData);
    		if (CrawledPageData.PreventPageGrabbing)
    		{
    			TaskLogger.Log(DataExcavatorTasksLoggerEntityType.CommonEntity, $"Page scraping prevented by user. URL = {CrawledPageData.DownloadedPageUrl.GetNormalizedOriginalLinkWithBehaviorPostfix()}");
    		}
    		if (!CrawledPageData.PreventPageGrabbing)
    		{
    			GrabbingBot.AddDataToGrabbing(CrawledPageData);
    		}
    	}

    	/// <summary>
    	/// Returns task actual metrics
    	/// </summary>
    	/// <returns>Task actual metrics at this moment</returns>
    	public DataExcavatorTaskActualMetric GetTaskActualMetrics()
    	{
    		return new DataExcavatorTaskActualMetric
    		{
    			PagesToCrawlQueueLength = CrawlingBot.LinksBuffer.LinksToCrawl.Count(),
    			TotalCrawledPagesCount = CrawlingBot.LinksBuffer.CrawledLinks.Count(),
    			NonEmptyResultsGrabbedPagesCount = Thread.VolatileRead(ref GrabbingBot.MetricsNonEmptyResultsGrabbedPagesCount),
    			TotalGrabbedPagesCount = Thread.VolatileRead(ref GrabbingBot.MetricsTotalGrabbedPagesCount),
    			PagesToGrabQueueLengh = GrabbingBot.GrabbingQueue.Count,
    			SessionWebsiteResponseAverageSpeedSeconds = Math.Round(CrawlingBot.PageCrawlingTimeSessionAverage, 2),
    			SessionCrawlingErrorsCount = Thread.VolatileRead(ref CrawlingBot.MetricsSessionTotalCrawlingErrorsCount),
    			GrabbedBinaryFilesCount = Thread.VolatileRead(ref GrabbingBot.MetricsGrabbedBinaryFilesCount),
    			GrabbedDataTotalSizeKb = Thread.VolatileRead(ref GrabbingBot.MetricsGrabbedDataTotalSizeKb)
    		};
    	}

    	/// <summary>
    	/// Reloads metrics that depended from saved data
    	/// </summary>
    	public void ReloadNativeMetrics()
    	{
    		GrabbingBot.LoadInitialMetrics();
    	}

    	/// <summary>
    	/// Exports grabbed data into simple view
    	/// </summary>
    	/// <param name="TaskPath">Task of the path</param>
    	/// <param name="ExportingFormat">Exporting format - file type to save assembled data</param>
    	/// <param name="ExportStartDate">Start date of parsed data. If null - will be used data from all periods</param>
    	/// <param name="ExportEndDate">End date of parsed data. If null - will be used data from all periods</param>
    	/// <param name="DataSavingDirectory">Target directory for data saving</param>
    	/// <param name="DataExportingProcessCallback">Callback to provide current information on the export process</param>
    	public void ExportAllGrabbedData(string DataSavingDirectory, DataExportingFormat ExportingFormat, DataExportingType ExportingType, string ItemsSequencesSeparator, DateTime ExportStartDate, DateTime ExportEndDate, Action<DataExportingProcessStat> DataExportingProcessCallback = null)
    	{
    		GrabbedDataExporter grabbedDataExporter = new GrabbedDataExporter();
    		grabbedDataExporter.ExportAllGrabbedData(TaskOperatingDirectory, DataSavingDirectory, TaskLogger, ExportingFormat, ExportingType, ItemsSequencesSeparator, ExportStartDate, ExportEndDate, DataExportingProcessCallback);
    	}

    	/// <summary>
    	/// Exports selected grabbed data in a simplifyed format with
    	/// </summary>
    	/// <param name="ExportingFormat">Exporting format - file type to save assembled data</param>
    	/// <param name="DirectoryToExportData">Target directory for exporting data</param>
    	/// <param name="ExportingType">Data exporting type</param>
    	/// <param name="ItemsSequencesSeparator">Separator for separating items sequences.</param>
    	/// <param name="SelectedEntriesToExport">Selected entries to export</param>
    	/// <param name="DataExportingProcessCallback">Callback to provide current information on the export process</param>
    	public void ExportSelectedGrabbedData(string DataSavingDirectory, DataExportingFormat ExportingFormat, DataExportingType ExportingType, string ItemsSequencesSeparator, List<GrabbedPageMetaInformationDataEntry> SelectedEntriesToExport, Action<DataExportingProcessStat> DataExportingProcessCallback = null)
    	{
    		GrabbedDataExporter grabbedDataExporter = new GrabbedDataExporter();
    		grabbedDataExporter.ExportSelectedGrabbedData(TaskOperatingDirectory, DataSavingDirectory, TaskLogger, ExportingFormat, ExportingType, ItemsSequencesSeparator, SelectedEntriesToExport, DataExportingProcessCallback);
    	}

    	/// <summary>
    	/// Gets grabbed data overview table
    	/// </summary>
    	/// <returns></returns>
    	public List<GrabbedPageMetaInformationDataEntry> GetGrabbedDataListOverview()
    	{
    		if (!GrabbingBot.GrabberProperties.StoreGrabbedData)
    		{
    			throw new InvalidOperationException("You cannot get an overview of the data because GrabbingServerProperties.StoreGrabbedData is set to false and this task does not save the found data.");
    		}
    		GrabbingDataIO grabbingDataIO = new GrabbingDataIO(GrabbingBot);
    		return grabbingDataIO.GetActualGrabbedPagesMeta();
    	}

    	/// <summary>
    	/// Return some grabbed data entry
    	/// </summary>
    	/// <param name="ParentMetaPointer">Pointer to some grabbed data information</param>
    	/// <returns>Set of grabbed data groups</returns>
    	public List<GrabbedDataGroup> GetGrabbedDataEntryOverview(GrabbedPageMetaInformationDataEntry ParentMetaPointer)
    	{
    		if (!GrabbingBot.GrabberProperties.StoreGrabbedData)
    		{
    			throw new InvalidOperationException("You cannot get an overview of the data because GrabbingServerProperties.StoreGrabbedData is set to false and this task does not save the found data.");
    		}
    		GrabbedDataExporter grabbedDataExporter = new GrabbedDataExporter();
    		return grabbedDataExporter.GetGrabbedDataEntryOverview(ParentMetaPointer);
    	}

    	/// <summary>
    	/// Deletes some grabbed data entry
    	/// </summary>
    	/// <param name="ParentMetaPointer">Pointer to some grabbed data information</param>
    	public void DeleteSpecifiedGrabbedDataEntries(List<GrabbedPageMetaInformationDataEntry> ParentMetaPointers, Action<ActionProcessingContainer> ProcessingCounter = null)
    	{
    		GrabbingDataIO grabbingDataIO = new GrabbingDataIO(GrabbingBot);
    		grabbingDataIO.DeleteSpecifiedGrabbedDataEntries(ParentMetaPointers, ProcessingCounter);
    	}

    	/// <summary>
    	/// Manually adds a set of links for analysis.
    	/// </summary>
    	/// <param name="PagesToCrawlNormalizedLinks">A list of links to analyse</param>
    	public CrawlingServerAddLinkToCrawlResults AddLinksToCrawling(List<string> PagesToCrawlNormalizedLinks)
    	{
    		TaskLogger.Log(DataExcavatorTasksLoggerEntityType.CommonEntity, $"User tried to add {PagesToCrawlNormalizedLinks.Count} link(s) to crawl");
    		_ = CrawlingBot.CrawlerProperties.PagesToCrawlLimit;
    		if (CrawlingBot.CrawlerProperties.PagesToCrawlLimit > 0 && CrawlingBot.LinksBuffer.CrawledLinks.Count + PagesToCrawlNormalizedLinks.Count > CrawlingBot.CrawlerProperties.PagesToCrawlLimit)
    		{
    			TaskLogger.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Crawling limit exceeded. See CrawlingServerProperties.PagesToCrawlLimit");
    			return new CrawlingServerAddLinkToCrawlResults
    			{
    				AddedLinksCount = 0,
    				SkippedLinksCount = PagesToCrawlNormalizedLinks.Count,
    				LinksAddingLogs = new List<CrawlingServerAddLinkToCrawlingResult>
    				{
    					new CrawlingServerAddLinkToCrawlingResult("", IsLinkAddedToCrawling: false, "Crawling limit exceeded. See CrawlingServerProperties.PagesToCrawlLimit")
    				}
    			};
    		}
    		return CrawlingBot.AddLinksToCrawling(PagesToCrawlNormalizedLinks, ForceBufferSavingOnHDD: true);
    	}

    	/// <summary>
    	/// Deletes the selected links from the link buffer. It is important to note that deleted links can be added again if the Crawler finds them on the pages being scanned. 
    	/// To prevent re-indexing, also set the appropriate limiting settings in the Crawler properties. Note that when you call this method, the running task will be stopped and restarted.
    	/// </summary>
    	/// <param name="PagesLinks">List of links to remove</param>
    	public void DeleteLinksFromCrawling(List<string> PagesLinks)
    	{
    		if (TaskState == DataExcavatorTaskState.Stopped)
    		{
    			StartStopTaskMutexEmulator = true;
    			CrawlingBot.RemoveLinksFromLinksBuffer(PagesLinks);
    			StartStopTaskMutexEmulator = false;
    			return;
    		}
    		StopTask(delegate
    		{
    			CrawlingBot.RemoveLinksFromLinksBuffer(PagesLinks);
    			StartStopTaskMutexEmulator = false;
    			StartTask();
    		});
    	}

    	/// <summary>
    	/// Forces specified link recrawling
    	/// </summary>
    	/// <param name="PageLink"></param>
    	public void ForceLinkRecrawling(string PageLink)
    	{
    		CrawlingBot.ForceLinkRecrawling(PageLink);
    	}

    	/// <summary>
    	/// Gets crawling server link buffer copy
    	/// </summary>
    	/// <returns></returns>
    	public CrawlingServerLinksBuffer GetCrawlingServerLinksBufferCopy()
    	{
    		return CrawlingBot.LinksBuffer.Clone() as CrawlingServerLinksBuffer;
    	}

    	/// <summary>
    	/// Saves task data into HDD storage
    	/// </summary>
    	public void SaveTaskToHDD()
    	{
    		DataExcavatorTaskIO dataExcavatorTaskIO = new DataExcavatorTaskIO();
    		dataExcavatorTaskIO.SaveDETask(TaskName, TaskOperatingDirectory, WebsiteRootUrl, TaskDescription, CrawlingBot.CrawlerProperties, GrabbingBot.GrabberProperties, GrabbingDataPatterns);
    		GrabbingDataIO grabbingDataIO = new GrabbingDataIO(GrabbingBot);
    		grabbingDataIO.SaveDataPatternsWithVersionControl(GrabbingDataPatterns);
    	}

    	/// <summary>
    	/// Loads task data from specified directory
    	/// </summary>
    	/// <param name="TaskDirectory">Task target directory</param>
    	/// <returns>New DETask instance</returns>
    	public static DataExcavatorTask ReadTaskFromHDD(string TaskDirectory)
    	{
    		DataExcavatorTaskIO dataExcavatorTaskIO = new DataExcavatorTaskIO();
    		return dataExcavatorTaskIO.CreateTaskFromProjectPath(TaskDirectory);
    	}

    	/// <summary>
    	/// Deletes task from HDD
    	/// </summary>
    	public void DeleteTaskFromHDD()
    	{
    		DataExcavatorTaskIO dataExcavatorTaskIO = new DataExcavatorTaskIO();
    		dataExcavatorTaskIO.DeleteDETask(TaskOperatingDirectory);
    	}

    	/// <summary>
    	/// Tests task settings - scans the content of the specified page, parses it according to Grubber's patterns and properties.
    	/// </summary>
    	/// <param name="TestingPageUrl">Testing page URL</param>
    	/// <param name="LogEntryAddedCallback">Callback for logger events</param>
    	/// <returns>Testing result</returns>
    	public DataExcavatorTaskTestingResult TestTask(string TestingPageUrl)
    	{
    		if (!IsLinkRefferedToDomain(TestingPageUrl))
    		{
    			return new DataExcavatorTaskTestingResult
    			{
    				LogsList = new List<string> { "Error - link is not reffered with the task domain name" }
    			};
    		}
    		TaskTestingResultsContainer = new DataExcavatorTaskTestingResult();
    		TaskTestingResultsContainer.Success = false;
    		DataExcavatorTaskIO dataExcavatorTaskIO = new DataExcavatorTaskIO();
    		int crawlingThreadsCount = CrawlingBot.CrawlerProperties.CrawlingThreadsCount;
    		int grabbingThreadsCount = GrabbingBot.GrabberProperties.GrabbingThreadsCount;
    		CrawlingBot.CrawlerProperties.CrawlingThreadsCount = 1;
    		GrabbingBot.GrabberProperties.GrabbingThreadsCount = 1;
    		string jSONData = dataExcavatorTaskIO.ExportDETaskIntoJSON(this);
    		DataExcavatorTask taskFromProperties = dataExcavatorTaskIO.ImportDETaskFromJSON(jSONData).GetTaskFromProperties(TaskOperatingDirectory);
    		CrawlingBot.CrawlerProperties.CrawlingThreadsCount = crawlingThreadsCount;
    		GrabbingBot.GrabberProperties.GrabbingThreadsCount = grabbingThreadsCount;
    		taskFromProperties.FlushLogsToHDD = false;
    		taskFromProperties.CrawlingBot.CrawlerProperties.RespectRobotsTxtFile = false;
    		taskFromProperties.CrawlingBot.CrawlerProperties.SitemapUrl = "";
    		taskFromProperties.CrawlingBot.CrawlerProperties.CrawlWebsiteLinks = false;
    		taskFromProperties.CrawlingBot.CrawlerProperties.StoreCrawledData = false;
    		taskFromProperties.CrawlingBot.CrawlerProperties.StoreCrawledPagesHTMLSource = false;
    		taskFromProperties.CrawlingBot.CrawlerProperties.ReindexCrawledPages = false;
    		if (taskFromProperties.CrawlingBot.CrawlerProperties.PrimaryDataCrawlingWay == DataCrawlingType.CEFCrawling)
    		{
    			taskFromProperties.CrawlingBot.CrawlerProperties.TakeScreenshotAfterPageLoaded = true;
    		}
    		taskFromProperties.CrawlingBot.LinksBuffer.CrawledLinks = new ConcurrentBag<PageLink>();
    		taskFromProperties.CrawlingBot.LinksBuffer.LinksToCrawl = new ConcurrentQueue<PageLink>();
    		taskFromProperties.GrabbingBot.GrabberProperties.StoreGrabbedData = false;
    		taskFromProperties.GrabbingBot.GrabberProperties.StoreOnlyNonEmptyData = false;
    		taskFromProperties.LogMessageAdded -= TaskTestingObject_LogMessageAdded;
    		taskFromProperties.LogMessageAdded += TaskTestingObject_LogMessageAdded;
    		taskFromProperties.PageCrawled -= TaskTestingObject_PageCrawled;
    		taskFromProperties.PageCrawled += TaskTestingObject_PageCrawled;
    		taskFromProperties.PageGrabbed -= TaskTestingObject_PageGrabbed;
    		taskFromProperties.PageGrabbed += TaskTestingObject_PageGrabbed;
    		CrawlingServerAddLinkToCrawlResults crawlingServerAddLinkToCrawlResults = taskFromProperties.AddLinksToCrawling(new List<string> { TestingPageUrl });
    		if (crawlingServerAddLinkToCrawlResults.AddedLinksCount == 0)
    		{
    			TaskTestingResultsContainer.LogsList.Add("Cannot add a link for testing. Crawling of this link is restricted in the Crawler settings.");
    			return TaskTestingResultsContainer;
    		}
    		taskFromProperties.StartTask();
    		DateTime now = DateTime.Now;
    		int num = taskFromProperties.CrawlingBot.CrawlerProperties.PageDownloadTimeoutMilliseconds + 5000;
    		do
    		{
    			Thread.Sleep(100);
    		}
    		while (TaskTestingResultsContainer.PageGrabbedResponseData == null && (DateTime.Now - now).TotalMilliseconds < (double)num);
    		if (TaskTestingResultsContainer.PageGrabbedResponseData == null)
    		{
    			TaskTestingResultsContainer.LogsList.Add($"Warning! Testing stopped by specified timeout ={taskFromProperties.CrawlingBot.CrawlerProperties.PageDownloadTimeoutMilliseconds}+5000 ms. Check Crawler param PageDownloadTimeoutMilliseconds.");
    		}
    		taskFromProperties.StopTask();
    		do
    		{
    			Thread.Sleep(100);
    		}
    		while (taskFromProperties.TaskState != DataExcavatorTaskState.Stopped);
    		if (TaskTestingResultsContainer != null && TaskTestingResultsContainer.PageGrabbedResponseData != null)
    		{
    			GrabbingDataIO grabbingDataIO = new GrabbingDataIO(GrabbingBot);
    			TaskTestingResultsContainer.GrabbedData = grabbingDataIO.ConvertPageGrabbedCallbackToGrabbedDataGroup(TaskTestingResultsContainer.PageGrabbedResponseData, SaveBinaryFiles: false, string.Empty);
    		}
    		DataExcavatorTaskTestingResult taskTestingResultsContainer = TaskTestingResultsContainer;
    		TaskTestingResultsContainer = null;
    		return taskTestingResultsContainer;
    	}

    	/// <summary>
    	/// Some page grabbed during Task testing
    	/// </summary>
    	/// <param name="GrabbedPageData"></param>
    	private void TaskTestingObject_PageGrabbed(PageGrabbedCallback GrabbedPageData)
    	{
    		if (TaskTestingResultsContainer != null)
    		{
    			TaskTestingResultsContainer.PageGrabbedResponseData = GrabbedPageData;
    			TaskTestingResultsContainer.Success = true;
    		}
    	}

    	/// <summary>
    	/// Some page crawled during Task testing
    	/// </summary>
    	/// <param name="CrawledPageData"></param>
    	private void TaskTestingObject_PageCrawled(PageCrawledCallback CrawledPageData)
    	{
    		if (TaskTestingResultsContainer != null)
    		{
    			TaskTestingResultsContainer.PageCrawledResponseData = CrawledPageData;
    		}
    	}

    	/// <summary>
    	/// Log entry added into Task testing container
    	/// </summary>
    	/// <param name="Callback"></param>
    	private void TaskTestingObject_LogMessageAdded(DataExcavatorTaskEventCallback Callback)
    	{
    		if (TaskTestingResultsContainer != null)
    		{
    			TaskTestingResultsContainer.LogsList.Add(Callback.GetEventAssembledText());
    		}
    		TaskLogger_LogMessageAdded(Callback);
    	}

    	/// <summary>
    	/// Observes some page links
    	/// </summary>
    	/// <param name="PageUrl">Target page URL</param>
    	/// <returns>Observing links list</returns>
    	public DataExcavatorPageLinksObservingResult ObservePageLinks(string PageUrl)
    	{
    		if (!IsLinkRefferedToDomain(PageUrl))
    		{
    			return new DataExcavatorPageLinksObservingResult
    			{
    				PageObservingLogs = new List<string> { "Error - link not reffered with Task domain name" }
    			};
    		}
    		PageLinkObservingContainer = new DataExcavatorPageLinksObservingResult();
    		PageLinkObservingContainer.Success = false;
    		DataExcavatorTaskIO dataExcavatorTaskIO = new DataExcavatorTaskIO();
    		int crawlingThreadsCount = CrawlingBot.CrawlerProperties.CrawlingThreadsCount;
    		int grabbingThreadsCount = GrabbingBot.GrabberProperties.GrabbingThreadsCount;
    		CrawlingBot.CrawlerProperties.CrawlingThreadsCount = 1;
    		GrabbingBot.GrabberProperties.GrabbingThreadsCount = -1;
    		string jSONData = dataExcavatorTaskIO.ExportDETaskIntoJSON(this);
    		DataExcavatorTask taskFromProperties = dataExcavatorTaskIO.ImportDETaskFromJSON(jSONData).GetTaskFromProperties(TaskOperatingDirectory);
    		taskFromProperties.TaskOperatingDirectory = TaskOperatingDirectory;
    		CrawlingBot.CrawlerProperties.CrawlingThreadsCount = crawlingThreadsCount;
    		GrabbingBot.GrabberProperties.GrabbingThreadsCount = grabbingThreadsCount;
    		taskFromProperties.FlushLogsToHDD = false;
    		taskFromProperties.CrawlingBot.CrawlerProperties.RespectRobotsTxtFile = false;
    		taskFromProperties.CrawlingBot.CrawlerProperties.SitemapUrl = "";
    		taskFromProperties.CrawlingBot.CrawlerProperties.CrawlWebsiteLinks = true;
    		taskFromProperties.CrawlingBot.CrawlerProperties.StoreCrawledData = false;
    		taskFromProperties.CrawlingBot.CrawlerProperties.StoreCrawledPagesHTMLSource = false;
    		taskFromProperties.CrawlingBot.CrawlerProperties.ReindexCrawledPages = false;
    		taskFromProperties.CrawlingBot.LinksBuffer.CrawledLinks = new ConcurrentBag<PageLink>();
    		taskFromProperties.CrawlingBot.LinksBuffer.LinksToCrawl = new ConcurrentQueue<PageLink>();
    		taskFromProperties.LogMessageAdded -= TaskObservingObject_LogMessageAdded;
    		taskFromProperties.LogMessageAdded += TaskObservingObject_LogMessageAdded;
    		taskFromProperties.PageCrawled -= TaskObservingObject_PageCrawled;
    		taskFromProperties.PageCrawled += TaskObservingObject_PageCrawled;
    		taskFromProperties.TaskOperatingDirectory = TaskOperatingDirectory;
    		CrawlingServerAddLinkToCrawlResults crawlingServerAddLinkToCrawlResults = taskFromProperties.AddLinksToCrawling(new List<string> { PageUrl });
    		if (crawlingServerAddLinkToCrawlResults.AddedLinksCount == 0)
    		{
    			PageLinkObservingContainer.PageObservingLogs.Add("Cannot observe page under specified link. Check Crawler settings.");
    			return PageLinkObservingContainer;
    		}
    		taskFromProperties.StartTask();
    		DateTime now = DateTime.Now;
    		int num = taskFromProperties.CrawlingBot.CrawlerProperties.PageDownloadTimeoutMilliseconds + 5000;
    		do
    		{
    			Thread.Sleep(50);
    		}
    		while (PageLinkObservingContainer.PageCrawledResponseData == null && (DateTime.Now - now).TotalMilliseconds < (double)num);
    		if (PageLinkObservingContainer.PageCrawledResponseData == null)
    		{
    			PageLinkObservingContainer.PageObservingLogs.Add($"Warning! Observing stopped by specified timeout = {taskFromProperties.CrawlingBot.CrawlerProperties.PageDownloadTimeoutMilliseconds}+5000 ms. Check Crawler param PageDownloadTimeoutMilliseconds.");
    		}
    		taskFromProperties.StopTask();
    		do
    		{
    			Thread.Sleep(100);
    		}
    		while (taskFromProperties.TaskState != DataExcavatorTaskState.Stopped);
    		DataExcavatorPageLinksObservingResult pageLinkObservingContainer = PageLinkObservingContainer;
    		PageLinkObservingContainer = null;
    		return pageLinkObservingContainer;
    	}

    	/// <summary>
    	/// Handle observing method Crawling callback
    	/// </summary>
    	/// <param name="CrawledPageData"></param>
    	private void TaskObservingObject_PageCrawled(PageCrawledCallback CrawledPageData)
    	{
    		if (PageLinkObservingContainer != null)
    		{
    			PageLinkObservingContainer.PageCrawledResponseData = CrawledPageData;
    			CrawledPageData.PreventPageGrabbing = true;
    			PageLinkObservingContainer.Success = true;
    		}
    	}

    	/// <summary>
    	/// Handle observing method LogMessageAdded callback
    	/// </summary>
    	/// <param name="Callback"></param>
    	private void TaskObservingObject_LogMessageAdded(DataExcavatorTaskEventCallback Callback)
    	{
    		if (PageLinkObservingContainer != null)
    		{
    			PageLinkObservingContainer.PageObservingLogs.Add(Callback.GetEventAssembledText());
    		}
    		TaskLogger_LogMessageAdded(Callback);
    	}

    	/// <summary>
    	/// Возвращает информацию о том, что некоторая ссылка относится к некоторому домену
    	/// </summary>
    	/// <param name="Link">Проверяемая ссылка</param>
    	/// <returns>Результат соотносимости с доменом</returns>
    	public bool IsLinkRefferedToDomain(string Link)
    	{
    		return Link.IndexOf(WebsiteRootUrlWithoutWWW) != -1 || Link.IndexOf(WebsiteRootUrl) != -1;
    	}

    	/// <summary>
    	/// Checks task settings for validity
    	/// </summary>
    	private void CheckTaskSettings()
    	{
    		if (TaskName == null || TaskName == string.Empty)
    		{
    			throw new NullReferenceException("Wrong task name");
    		}
    		if (TaskOperatingDirectory == null || TaskOperatingDirectory == string.Empty)
    		{
    			throw new NullReferenceException("Wrong task directory");
    		}
    		if (WebsiteRootUrl == null || WebsiteRootUrl == string.Empty)
    		{
    			throw new NullReferenceException("Wrong task website root url");
    		}
    		if (CrawlingBot.CrawlerProperties == null)
    		{
    			throw new NullReferenceException("CrawlingServerProperties properties cannot be null");
    		}
    		if (CrawlingBot.CrawlerProperties.ConcurrentCollectionsParallelismQuantity < 1)
    		{
    			throw new ArgumentOutOfRangeException("Error in CrawlingServerProperties, param ConcurrentCollectionsParallelismQuantity cannot be negative");
    		}
    		if (CrawlingBot.CrawlerProperties.CrawlerUserAgent == null || CrawlingBot.CrawlerProperties.CrawlerUserAgent == string.Empty)
    		{
    			throw new NullReferenceException("Error in CrawlingServerProperties, param CrawlerUserAgent cannot be null or empty");
    		}
    		if (CrawlingBot.CrawlerProperties.CrawlingThreadDelayMilliseconds < 1)
    		{
    			throw new ArgumentOutOfRangeException("Error in CrawlingServerProperties, param CrawlingThreadDelayMilliseconds cannot be negative");
    		}
    		if (CrawlingBot.CrawlerProperties.CrawlingThreadsCount < 1)
    		{
    			throw new ArgumentOutOfRangeException("Error in CrawlingServerProperties, param CrawlingThreadsCount cannot be negative");
    		}
    		if (CrawlingBot.CrawlerProperties.HttpWebRequestMethod == null || (CrawlingBot.CrawlerProperties.HttpWebRequestMethod != "GET" && CrawlingBot.CrawlerProperties.HttpWebRequestMethod != "POST"))
    		{
    			throw new Exception("Error in CrawlingServerProperties, param HttpWebRequestMethod must be setted to GET or POST");
    		}
    		if (CrawlingBot.CrawlerProperties.LinksBufferHDDAutoSavingMilliseconds < 1)
    		{
    			throw new ArgumentOutOfRangeException("Error in CrawlingServerProperties, param LinksBufferHDDAutoSavingMilliseconds cannot be negative");
    		}
    		if (CrawlingBot.CrawlerProperties.PageDownloadAttempts < 1)
    		{
    			throw new ArgumentOutOfRangeException("Error in CrawlingServerProperties, param PageDownloadAttempts cannot be negative");
    		}
    		if (CrawlingBot.CrawlerProperties.PageDownloadTimeoutMilliseconds < 1)
    		{
    			throw new ArgumentOutOfRangeException("Error in CrawlingServerProperties, param PageDownloadTimeoutMilliseconds cannot be negative");
    		}
    		if (CrawlingBot.CrawlerProperties.PagesToCrawlLimit < -1)
    		{
    			throw new ArgumentOutOfRangeException("Error in CrawlingServerProperties, param PagesToCrawlLimit cannot be less then -1");
    		}
    		if (CrawlingBot.CrawlerProperties.ReindexCrawledPagesAfterSpecifiedMinutesInterval < 1)
    		{
    			throw new ArgumentOutOfRangeException("Error in CrawlingServerProperties, param ReindexCrawledPagesAfterSpecifiedMinutesInterval cannot be negative");
    		}
    		if (CrawlingBot.CrawlerProperties.RobotsTxtReindexTimeDays < -1)
    		{
    			throw new ArgumentOutOfRangeException("Error in CrawlingServerProperties, param RobotsTxtReindexTimeDays cannot be negative");
    		}
    		if (CrawlingBot.CrawlerProperties.SitemapReindexTimeDays < -1)
    		{
    			throw new ArgumentOutOfRangeException("Error in CrawlingServerProperties, param SitemapReindexTimeDays cannot be negative");
    		}
    		if (CrawlingBot.CrawlerProperties.TakeScreenshotAfterPageLoaded && CrawlingBot.CrawlerProperties.PrimaryDataCrawlingWay == DataCrawlingType.NativeCrawling)
    		{
    			throw new Exception("Can't apply automatic screenshot flag (CrawlerProperties.TakeScreenshotAfterPageLoaded = true) with PrimaryDataCrawlingWay=NativeCrawling");
    		}
    		if (CrawlingBot.CrawlerProperties.ExpandPageFrames && CrawlingBot.CrawlerProperties.PrimaryDataCrawlingWay == DataCrawlingType.NativeCrawling)
    		{
    			throw new Exception("Can't apply framex auto expand (CrawlerProperties.ExpandPageFrames = true) flag with PrimaryDataCrawlingWay=NativeCrawling");
    		}
    		if (CrawlingBot.CrawlerProperties.CEFCrawlingBehaviors != null && CrawlingBot.CrawlerProperties.CEFCrawlingBehaviors.Count > 0)
    		{
    			if (CrawlingBot.CrawlerProperties.PrimaryDataCrawlingWay == DataCrawlingType.NativeCrawling)
    			{
    				throw new ArgumentOutOfRangeException("You can use CEFCrawlingBehaviors only in PrimaryDataCrawlingWay=CEFCrawling mode");
    			}
    			for (int i = 0; i < CrawlingBot.CrawlerProperties.CEFCrawlingBehaviors.Count; i++)
    			{
    				if (CrawlingBot.CrawlerProperties.CEFCrawlingBehaviors[i].PageUrlSubstringPattern == null || CrawlingBot.CrawlerProperties.CEFCrawlingBehaviors[i].PageUrlSubstringPattern.Trim().Length == 0)
    				{
    					throw new ArgumentOutOfRangeException("CEFCrawlingBehavior: allowed page url is not specified");
    				}
    			}
    		}
    		if (CrawlingBot.CrawlerProperties.CEFWebsiteAuthenticationBehavior != null)
    		{
    			if (CrawlingBot.CrawlerProperties.PrimaryDataCrawlingWay == DataCrawlingType.NativeCrawling)
    			{
    				throw new ArgumentOutOfRangeException("You can use CEFWebsiteAuthenticationBehavior only in PrimaryDataCrawlingWay=CEFCrawling mode");
    			}
    			if ((CrawlingBot.CrawlerProperties.CEFWebsiteAuthenticationBehavior.PagesURLSubstringsCheckRestrictions == null && CrawlingBot.CrawlerProperties.CEFWebsiteAuthenticationBehavior.PagesURLSubstringsToCheckLogin == null) || (CrawlingBot.CrawlerProperties.CEFWebsiteAuthenticationBehavior.PagesURLSubstringsCheckRestrictions.Length == 0 && CrawlingBot.CrawlerProperties.CEFWebsiteAuthenticationBehavior.PagesURLSubstringsToCheckLogin.Length == 0))
    			{
    				throw new ArgumentOutOfRangeException("Substring parameters are not defined for both allowed and denied URLs.");
    			}
    			if (CrawlingBot.CrawlerProperties.CEFWebsiteAuthenticationBehavior.WebsiteLoginPageURL == null || CrawlingBot.CrawlerProperties.CEFWebsiteAuthenticationBehavior.WebsiteLoginPageURL.Length == 0)
    			{
    				throw new Exception("Wrong address of login page");
    			}
    			if (CrawlingBot.CrawlerProperties.CEFWebsiteAuthenticationBehavior.CheckUserLoggedInDocumentHTMLSubstring == null || CrawlingBot.CrawlerProperties.CEFWebsiteAuthenticationBehavior.WebsiteLoginPageURL.Length == 0)
    			{
    				throw new Exception("Wrong substring for 'is-logged-on' check");
    			}
    		}
    		if (!GrabbingBot.IsGrabberInitializationPrevented)
    		{
    			if (GrabbingBot.GrabberProperties == null)
    			{
    				throw new NullReferenceException("GrabbingServerProperties cannot be null");
    			}
    			if (GrabbingBot.GrabberProperties.ExportDataOnline && (GrabbingBot.GrabberProperties.ExportDataOnlineInvokationLink == null || GrabbingBot.GrabberProperties.ExportDataOnlineInvokationLink == string.Empty))
    			{
    				throw new NullReferenceException("Error in GrabbingServerProperties, param ExportDataOnline setted to true, but ExportDataOnlineInvokationLink not specified");
    			}
    			if (GrabbingBot.GrabberProperties.GrabbingThreadDelayMilliseconds < 1)
    			{
    				throw new Exception("Error in GrabbingServerProperties, param GrabbingThreadDelayMilliseconds cannot be nagative");
    			}
    			if (GrabbingBot.GrabberProperties.GrabbingThreadsCount < 1)
    			{
    				throw new Exception("Error in GrabbingServerProperties, param GrabbingThreadsCount cannot be negative");
    			}
    		}
    		if (CrawlingBot.CrawlerProperties.CaptchaSettings != null)
    		{
    			if (CrawlingBot.CrawlerProperties.CaptchaSettings.ForceSpecifiedCaptchaDetectionSubstring == string.Empty)
    			{
    				throw new Exception("Error in CrawlingServerProperties -> CAPTCHA settings. The ForceSpecifiedCaptchaDetectionSubstring parameter cannot be empty.");
    			}
    			_ = CrawlingBot.CrawlerProperties.CaptchaSettings.CaptchaSolvingAttempts;
    			if (CrawlingBot.CrawlerProperties.CaptchaSettings.CaptchaSolvingAttempts < 1)
    			{
    				throw new Exception("Error in CrawlingServerProperties -> CAPTCHA settings. The CaptchaSolvingAttempts parameter cannot be less than 1.");
    			}
    			_ = CrawlingBot.CrawlerProperties.CaptchaSettings.CaptchaSolvingTimeoutSeconds;
    			if (CrawlingBot.CrawlerProperties.CaptchaSettings.CaptchaSolvingTimeoutSeconds < 20)
    			{
    				throw new Exception("Error in CrawlingServerProperties -> CAPTCHA settings. The CaptchaSolvingAttempts parameter cannot be less than 20.");
    			}
    			_ = CrawlingBot.CrawlerProperties.CaptchaSettings.ForceSpecifiedCaptchaType;
    			if (false)
    			{
    				throw new Exception("Error in CrawlingServerProperties -> CAPTCHA settings. The ForceSpecifiedCaptchaType parameter cannot be null.");
    			}
    			_ = CrawlingBot.CrawlerProperties.CaptchaSettings.SolvingService;
    			if (false)
    			{
    				throw new Exception("Error in CrawlingServerProperties -> CAPTCHA settings. The SolvingService parameter cannot be null.");
    			}
    		}
    		if (!GrabbingBot.IsGrabberInitializationPrevented)
    		{
    			if (GrabbingBot.GrabbingPatterns == null || GrabbingBot.GrabbingPatterns.Count == 0)
    			{
    				throw new NullReferenceException("Error in GrabbingBot. Grabbing patterns list cannot be null or empty");
    			}
    			foreach (DataGrabbingPattern grabbingPattern in GrabbingBot.GrabbingPatterns)
    			{
    				if (grabbingPattern.PatternName == null || grabbingPattern.PatternName == string.Empty)
    				{
    					throw new NullReferenceException("Error in DataGrabbingPattern. Name cannot be null or empty");
    				}
    				if (grabbingPattern.GrabbingItemsPatterns == null || grabbingPattern.GrabbingItemsPatterns.Count == 0)
    				{
    					throw new NullReferenceException("Error in DataGrabbingPattern. GrabbingItemsPatterns cannot be null or empty");
    				}
    				foreach (DataGrabbingPatternItem grabbingItemsPattern in grabbingPattern.GrabbingItemsPatterns)
    				{
    					if (grabbingItemsPattern.ElementName == null || grabbingItemsPattern.ElementName == string.Empty)
    					{
    						throw new NullReferenceException("Error in DataGrabbingPatternItem, param ElementName cannot be null or empty");
    					}
    					if (grabbingItemsPattern.DataSelector == null)
    					{
    						throw new NullReferenceException("Error in DataGrabbingPatternItem, param DataSelector cannot be null");
    					}
    					if (grabbingItemsPattern.DataSelector.Selector == null || grabbingItemsPattern.DataSelector.Selector == string.Empty)
    					{
    						throw new NullReferenceException("Error in DataGrabbingPatternItem, param DataSelector.Selector cannot be null or empty");
    					}
    				}
    			}
    		}
    		foreach (DataGrabbingPattern grabbingPattern2 in GrabbingBot.GrabbingPatterns)
    		{
    			if (grabbingPattern2.OuterBlockSelector != null && !DOMSelectorsTester.TestSelector(grabbingPattern2.OuterBlockSelector))
    			{
    				throw new FormatException(string.Format("Error in DataGrabbingPattern.OuterBlockSelector - wrong or unsupported selector. Selector value = {0};", (grabbingPattern2.OuterBlockSelector.Selector != null && grabbingPattern2.OuterBlockSelector.Selector.Length > 0) ? grabbingPattern2.OuterBlockSelector.Selector : "null"));
    			}
    			foreach (DataGrabbingPatternItem grabbingItemsPattern2 in grabbingPattern2.GrabbingItemsPatterns)
    			{
    				if (grabbingItemsPattern2.DataSelector == null)
    				{
    					throw new NullReferenceException("Error in DataGrabbingPattern.GrabbingItemsPatterns[N]. Param DataSelector cannot be null");
    				}
    				if (!DOMSelectorsTester.TestSelector(grabbingItemsPattern2.DataSelector))
    				{
    					throw new FormatException(string.Format("Error in DataGrabbingPattern.GrabbingItemsPatterns[N].DataSelector - wrong or unsupported selector. Selector value = {0};", (grabbingItemsPattern2.DataSelector.Selector != null && grabbingItemsPattern2.DataSelector.Selector.Length > 0) ? grabbingItemsPattern2.DataSelector.Selector : "null"));
    				}
    			}
    		}
    		if (CrawlingBot.CrawlerProperties.PrimaryDataCrawlingWay == DataCrawlingType.CEFCrawling && !CEFSharpFactory.CEFInitialized)
    		{
    			throw new Exception("Error while trying to use CEF crawling. You must invoke CEFSharpFactory.InitializeCEFBrowser() before any actions with CEF");
    		}
    		if (CrawlingBot.CrawlerProperties.RespectOnlySpecifiedUrls != null)
    		{
    			for (int j = 0; j < CrawlingBot.CrawlerProperties.RespectOnlySpecifiedUrls.Length; j++)
    			{
    				CrawlingBot.CrawlerProperties.RespectOnlySpecifiedUrls[j] = CrawlingBot.CrawlerProperties.RespectOnlySpecifiedUrls[j].ToLower();
    			}
    		}
    		if (CrawlingBot.CrawlerProperties.UrlSubstringsRestrictions != null)
    		{
    			for (int k = 0; k < CrawlingBot.CrawlerProperties.UrlSubstringsRestrictions.Length; k++)
    			{
    				CrawlingBot.CrawlerProperties.UrlSubstringsRestrictions[k] = CrawlingBot.CrawlerProperties.UrlSubstringsRestrictions[k].ToLower();
    			}
    		}
    		for (int l = 0; l < GrabbingDataPatterns.Count; l++)
    		{
    			for (int m = 0; m < GrabbingDataPatterns[l].AllowedPageUrlsSubstrings.Length; m++)
    			{
    				GrabbingDataPatterns[l].AllowedPageUrlsSubstrings[m] = GrabbingDataPatterns[l].AllowedPageUrlsSubstrings[m].ToLower();
    			}
    		}
    	}

    	/// <summary>
    	/// Starts task async
    	/// </summary>
    	public void StartTask(Action TaskStartedCallback = null)
    	{
    		CheckTaskSettings();
    		if (!LicenseServer.CheckLicenseKeyValid())
    		{
    			throw LicenseValidationException.FromLicenseValidationResponse();
    		}
    		if (LicenseServer.ActualKey.KeyTotalThreadsLimitPerProject != -1 && (CrawlingBot.CrawlerProperties.CrawlingThreadsCount > LicenseServer.ActualKey.KeyTotalThreadsLimitPerProject || GrabbingBot.GrabberProperties.GrabbingThreadsCount > LicenseServer.ActualKey.KeyTotalThreadsLimitPerProject))
    		{
    			throw LicenseValidationException.FromCrawlingServerIfWrongThreadsCountRequested();
    		}
    		if (!DataExcavatorTasksFactory.IsExcavatorCoreInitialized)
    		{
    			throw new InvalidOperationException("DataExcavator is not initialized. Use method DataExcavatorTasksFactory.InitializeExcavator for initialization.");
    		}
    		if (!StartStopTaskMutexEmulator)
    		{
    			StartStopTaskMutexEmulator = true;
    			DataExcavatorStartStopTaskArg dataExcavatorStartStopTaskArg = new DataExcavatorStartStopTaskArg();
    			dataExcavatorStartStopTaskArg.DataExcavatorStartedCallback = TaskStartedCallback;
    			dataExcavatorStartStopTaskArg.StarterLink = new Thread(StartTaskAsyncThreadBody);
    			dataExcavatorStartStopTaskArg.StarterLink.Start(dataExcavatorStartStopTaskArg);
    		}
    	}

    	/// <summary>
    	/// Starts task async, inner thread body
    	/// </summary>
    	private void StartTaskAsyncThreadBody(object Arg)
    	{
    		DataExcavatorStartStopTaskArg dataExcavatorStartStopTaskArg = Arg as DataExcavatorStartStopTaskArg;
    		if (TaskState == DataExcavatorTaskState.Stopped)
    		{
    			TaskStartedLastDate = DateTime.Now;
    			if (!Directory.Exists(TaskOperatingDirectory))
    			{
    				Directory.CreateDirectory(TaskOperatingDirectory);
    			}
    			if (FlushLogsToHDD)
    			{
    				TaskLogger.StartLoggerHDDFlushing();
    			}
    			CrawlingBot.StartCrawling();
    			TaskLogger.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Crawling server started");
    			GrabbingBot.StartGrabbing();
    			TaskLogger.Log(DataExcavatorTasksLoggerEntityType.GrabbingServer, "Scraping server started");
    			TaskLogger.Log(DataExcavatorTasksLoggerEntityType.CommonEntity, "Task started");
    			TaskLogger.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Links buffer size: Crawled links = {CrawlingBot.LinksBuffer.CrawledLinks.Count}, Links to crawl = {CrawlingBot.LinksBuffer.LinksToCrawl.Count}");
    			TaskState = DataExcavatorTaskState.Executing;
    		}
    		if (dataExcavatorStartStopTaskArg.DataExcavatorStartedCallback != null)
    		{
    			dataExcavatorStartStopTaskArg.DataExcavatorStartedCallback();
    		}
    		StartStopTaskMutexEmulator = false;
    		dataExcavatorStartStopTaskArg.StarterLink.Abort();
    	}

    	/// <summary>
    	/// Stops task async
    	/// </summary>
    	public void StopTask(Action TaskStoppedCallback = null)
    	{
    		if (!StartStopTaskMutexEmulator)
    		{
    			StartStopTaskMutexEmulator = true;
    			DataExcavatorStartStopTaskArg dataExcavatorStartStopTaskArg = new DataExcavatorStartStopTaskArg();
    			dataExcavatorStartStopTaskArg.DataExcavatorStartedCallback = TaskStoppedCallback;
    			dataExcavatorStartStopTaskArg.StarterLink = new Thread(StopTaskAsyncThreadBody);
    			dataExcavatorStartStopTaskArg.StarterLink.Start(dataExcavatorStartStopTaskArg);
    		}
    	}

    	/// <summary>
    	/// Stops task async, inner thread body
    	/// </summary>
    	/// <param name="Arg"></param>
    	private void StopTaskAsyncThreadBody(object Arg)
    	{
    		DataExcavatorStartStopTaskArg dataExcavatorStartStopTaskArg = Arg as DataExcavatorStartStopTaskArg;
    		if (TaskState == DataExcavatorTaskState.Executing)
    		{
    			TaskStoppedLastDate = DateTime.Now;
    			CrawlingBot.StopCrawling();
    			TaskLogger.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Crawling server stopped");
    			GrabbingBot.StopGrabbing();
    			TaskLogger.Log(DataExcavatorTasksLoggerEntityType.GrabbingServer, "Scraping server stopped");
    			TimeSpan timeSpan = TaskStoppedLastDate - TaskStartedLastDate;
    			TaskLogger.Log(DataExcavatorTasksLoggerEntityType.CommonEntity, "Task stopped");
    			TaskLogger.Log(DataExcavatorTasksLoggerEntityType.CommonEntity, string.Format("Task total working time from start to stop, H:m:s = {0}", timeSpan.ToString("hh\\:mm\\:ss")));
    			TaskLogger.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Links buffer size: Crawled links = {CrawlingBot.LinksBuffer.CrawledLinks.Count}, Links to crawl = {CrawlingBot.LinksBuffer.LinksToCrawl.Count}");
    			TaskLogger.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Session average page crawling time: {Math.Round(CrawlingBot.PageCrawlingTimeSessionAverage, 2)} seconds");
    			TaskLogger.StopLoggerHDDFlushing();
    			TaskState = DataExcavatorTaskState.Stopped;
    		}
    		if (dataExcavatorStartStopTaskArg.DataExcavatorStartedCallback != null)
    		{
    			dataExcavatorStartStopTaskArg.DataExcavatorStartedCallback();
    		}
    		StartStopTaskMutexEmulator = false;
    		dataExcavatorStartStopTaskArg.StarterLink.Abort();
    	}

    	/// <summary>
    	/// Gets crawling server properties copy
    	/// </summary>
    	/// <returns></returns>
    	public CrawlingServerProperties GetCrawlingServerPropertiesCopy()
    	{
    		return CrawlingBot.CrawlerProperties.Clone() as CrawlingServerProperties;
    	}

    	/// <summary>
    	/// Gets grabbing server properties copy
    	/// </summary>
    	/// <returns></returns>
    	public GrabbingServerProperties GetGrabbingServerPropertiesCopy()
    	{
    		return GrabbingBot.GrabberProperties.Clone() as GrabbingServerProperties;
    	}

    	/// <summary>
    	/// Gets data grabbing patterns copy
    	/// </summary>
    	/// <returns></returns>
    	public List<DataGrabbingPattern> GetDataGrabbingPatternsCopy()
    	{
    		DataGrabbingPattern dataGrabbingPattern = new DataGrabbingPattern();
    		List<DataGrabbingPattern> list = new List<DataGrabbingPattern>();
    		for (int i = 0; i < GrabbingDataPatterns.Count; i++)
    		{
    			string jSONData = GrabbingDataPatterns[i].SerializeToJSON();
    			list.Add(dataGrabbingPattern.UnserializeFromJSON(jSONData));
    		}
    		return list;
    	}

    	/// <summary>
    	/// Gets hash code of actual task
    	/// </summary>
    	/// <returns></returns>
    	public override int GetHashCode()
    	{
    		return TaskName.GetHashCode() + WebsiteRootUrl.GetHashCode();
    	}
    }
}
