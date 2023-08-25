// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Excavator.DataExcavatorTasksLogger
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using ExcavatorSharp.Common;
using ExcavatorSharp.ExtensionMethods;
using ExcavatorSharp.Objects;

namespace ExcavatorSharp.Excavator
{
    /// <summary>
    /// Class for storing project logs
    /// </summary>
    public class DataExcavatorTasksLogger
    {
    	/// <summary>
    	/// Delegate for LogMessageAdded event
    	/// </summary>
    	/// <param name="Callback"></param>
    	public delegate void LogEntryAdded(DataExcavatorTaskEventCallback Callback);

    	/// <summary>
    	/// Separator for logs sessions
    	/// </summary>
    	private string LogsSessionsSeparator = "---------------------------------------------------------------\r\n";

    	/// <summary>
    	/// Max entries for one HDD flush itteration
    	/// </summary>
    	private int MaxLogsFlushChainEntriesCount = 100;

    	/// <summary>
    	/// Repeat time for logs flushing on HDD
    	/// </summary>
    	private const int FlushLogsOnHDDOneCycleTimeMilliseconds = 1000;

    	/// <summary>
    	/// Name of folder with logs files
    	/// </summary>
    	private const string TaskLogFolderName = "de-task-logs";

    	/// <summary>
    	/// Name of file for storing task logs
    	/// </summary>
    	public const string TaskLogFileName = "logs{0}.txt";

    	/// <summary>
    	/// List of messages to log on HDD
    	/// </summary>
    	private ConcurrentQueue<string> MessagesToLog = new ConcurrentQueue<string>();

    	/// <summary>
    	/// Target directory for data saving
    	/// </summary>
    	private string TaskOperatingDirectory { get; set; }

    	/// <summary>
    	/// Thread for saving data to HDD
    	/// </summary>
    	private Thread LogFileIOThread { get; set; }

    	/// <summary>
    	/// Logs HDD flushing state
    	/// </summary>
    	internal bool FlushLogsToHDD { get; set; }

    	/// <summary>
    	/// Specify is logs flushing started
    	/// </summary>
    	private bool LogsFlushingStarted { get; set; }

    	/// <summary>
    	/// The event that will be triggered every time a new message is added.
    	/// </summary>
    	public event LogEntryAdded LogMessageAdded;

    	/// <summary>
    	/// Creates new instance of DETasksLogger
    	/// </summary>
    	/// <param name="TaskOperatingDirectory"></param>
    	public DataExcavatorTasksLogger(string TaskOperatingDirectory)
    	{
    		this.TaskOperatingDirectory = TaskOperatingDirectory;
    		LogsFlushingStarted = false;
    	}

    	/// <summary>
    	/// Logs that logger initialized
    	/// </summary>
    	internal void AddLogSessionInitialEntry()
    	{
    		MessagesToLog.Enqueue(LogsSessionsSeparator);
    		MessagesToLog.Enqueue(PackLogMessage(DateTime.Now, DataExcavatorTasksLoggerEntityType.CommonEntity, "Task logger initialized"));
    	}

    	/// <summary>
    	/// Logs message from some task
    	/// </summary>
    	/// <param name="EventEntity">Type of entity</param>
    	/// <param name="Message">Message you want to log</param>
    	internal void Log(DataExcavatorTasksLoggerEntityType EventEntity, string Message, Exception OccuredException = null)
    	{
    		DateTime now = DateTime.Now;
    		if (FlushLogsToHDD)
    		{
    			MessagesToLog.Enqueue(PackLogMessage(now, EventEntity, Message, OccuredException));
    		}
    		this.LogMessageAdded?.Invoke(new DataExcavatorTaskEventCallback(EventEntity, now, Message, OccuredException));
    	}

    	/// <summary>
    	/// Packs log message into inner log format
    	/// </summary>
    	/// <param name="EntityType">Type of entity that invokes log</param>
    	/// <param name="Message">Logging message</param>
    	/// <param name="OccuredException">Exception, it it thrown</param>
    	/// <param name="EventDate">Event occured date</param>
    	/// <returns>Packed log message</returns>
    	private string PackLogMessage(DateTime EventDate, DataExcavatorTasksLoggerEntityType EntityType, string Message, Exception OccuredException = null)
    	{
    		string empty = string.Empty;
    		if (OccuredException == null)
    		{
    			return string.Format("[{0}, {1}]: {2};\r\n", EventDate.ToString("dd.MM.yyyy HH:mm:ss"), EntityType.ToString(), Message);
    		}
    		return string.Format("[{0}, {1}]: {2}, Exception data = {3};\r\n", EventDate.ToString("dd.MM.yyyy HH:mm:ss"), EntityType.ToString(), Message, OccuredException.ToString());
    	}

    	/// <summary>
    	/// Writes data to HDD physically
    	/// </summary>
    	/// <param name="DataToWrite">Block of writing data</param>
    	private void WriteToLogFile(string DataToWrite)
    	{
    		if (DataToWrite == string.Empty)
    		{
    			return;
    		}
    		string text = string.Format("{0}/{1}", TaskOperatingDirectory, "de-task-logs");
    		if (!Directory.Exists(text))
    		{
    			Directory.CreateDirectory(text);
    		}
    		string arg = string.Format("logs{0}.txt", DateTime.Now.ToString("yyyyMMdd"));
    		string path = $"{text}/{arg}";
    		if (!File.Exists(path))
    		{
    			File.WriteAllText(path, PackLogMessage(DateTime.Now, DataExcavatorTasksLoggerEntityType.CommonEntity, "New log file created"));
    		}
    		int num = 0;
    		bool flag = false;
    		do
    		{
    			try
    			{
    				File.AppendAllText(path, DataToWrite);
    				flag = true;
    			}
    			catch (Exception)
    			{
    				Thread.Sleep(50);
    				num++;
    			}
    		}
    		while (num < 20 && !DEConfig.ShutDownProgram && !flag && LogsFlushingStarted);
    	}

    	/// <summary>
    	/// Data saving thread main function
    	/// </summary>
    	private void FlushLogsToHDDThreadBody()
    	{
    		StringBuilder stringBuilder = new StringBuilder();
    		while (!DEConfig.ShutDownProgram && LogsFlushingStarted)
    		{
    			int num = 0;
    			string result = string.Empty;
    			while (MessagesToLog.TryDequeue(out result) && !DEConfig.ShutDownProgram && LogsFlushingStarted && num < MaxLogsFlushChainEntriesCount)
    			{
    				stringBuilder.Append(result);
    				num++;
    			}
    			if (stringBuilder.Length > 0)
    			{
    				WriteToLogFile(stringBuilder.ToString());
    			}
    			stringBuilder.Clear();
    			Thread.Sleep(1000);
    		}
    	}

    	/// <summary>
    	/// Starts logger
    	/// </summary>
    	internal void StartLoggerHDDFlushing()
    	{
    		if (!LogsFlushingStarted)
    		{
    			if (LogFileIOThread == null || LogFileIOThread.IsThreadMustBeReloadedBeforeStart())
    			{
    				LogFileIOThread = new Thread(FlushLogsToHDDThreadBody);
    			}
    			Log(DataExcavatorTasksLoggerEntityType.CommonEntity, "Logs HDD flushing started");
    			LogsFlushingStarted = true;
    			LogFileIOThread.Start();
    		}
    	}

    	/// <summary>
    	/// Stops logger
    	/// </summary>
    	internal void StopLoggerHDDFlushing()
    	{
    		if (LogsFlushingStarted)
    		{
    			LogsFlushingStarted = false;
    			if (LogFileIOThread != null)
    			{
    				LogFileIOThread.Abort();
    			}
    			StringBuilder stringBuilder = new StringBuilder();
    			string result = string.Empty;
    			while (MessagesToLog.TryDequeue(out result))
    			{
    				stringBuilder.Append(result);
    			}
    			WriteToLogFile(stringBuilder.ToString());
    			stringBuilder.Clear();
    			WriteToLogFile(PackLogMessage(DateTime.Now, DataExcavatorTasksLoggerEntityType.CommonEntity, "Logs HDD flushing stopped"));
    			WriteToLogFile(LogsSessionsSeparator);
    		}
    	}

    	/// <summary>
    	/// Returns list of DataExcavator log files
    	/// </summary>
    	/// <returns></returns>
    	public List<string> GetLogFilesList()
    	{
    		string path = string.Format("{0}/{1}", TaskOperatingDirectory, "de-task-logs");
    		if (!Directory.Exists(path))
    		{
    			Directory.CreateDirectory(path);
    		}
    		string[] files = Directory.GetFiles(path, string.Format("logs{0}.txt", "*"));
    		return files.ToList();
    	}
    }
}
