// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Excavator.DataExcavatorTasksFactory
using System;
using System.Collections.Generic;
using System.Threading;
using ExcavatorSharp.CEF;
using ExcavatorSharp.Common;
using ExcavatorSharp.Licensing;

namespace ExcavatorSharp.Excavator
{
    /// <summary>
    /// Class for storing tasks
    /// </summary>
    public class DataExcavatorTasksFactory
    {
    	/// <summary>
    	/// Data excavator initialization flag
    	/// </summary>
    	private static bool IsExcavatorInitialized = false;

    	/// <summary>
    	/// All existing tasks
    	/// </summary>
    	internal List<DataExcavatorTask> GrabbingTasksList = new List<DataExcavatorTask>();

    	/// <summary>
    	/// Specifies is outer code can invoke AddTask method
    	/// </summary>
    	private bool CanAddNewTask = true;

    	/// <summary>
    	/// Is application launched as a single copy - checked
    	/// </summary>
    	private bool ApplicationSingleCopyLaunchedCheck = false;

    	/// <summary>
    	/// Mutex to control that only one DataExcavatorInstance initialized
    	/// </summary>
    	private Mutex OneApplicationInstanceMutex { get; set; }

    	/// <summary>
    	/// Data excavator initialized public flag
    	/// </summary>
    	public static bool IsExcavatorCoreInitialized => IsExcavatorInitialized;

    	/// <summary>
    	/// Adds new task and starts its execution (optional)
    	/// </summary>
    	/// <param name="NewExcavatorTask">New completed task object</param>
    	public void AddTask(DataExcavatorTask NewExcavatorTask)
    	{
    		if (!CanAddNewTask)
    		{
    			return;
    		}
    		for (int i = 0; i < GrabbingTasksList.Count; i++)
    		{
    			if (GrabbingTasksList[i].TaskName == NewExcavatorTask.TaskName)
    			{
    				throw new Exception("A task with that name already exists.");
    			}
    		}
    		if (LicenseServer.ActualKey == null || LicenseServer.ActualKey.KeyProjectsLimit == -1 || GrabbingTasksList.Count + 1 <= LicenseServer.ActualKey.KeyProjectsLimit)
    		{
    			GrabbingTasksList.Add(NewExcavatorTask);
    		}
    	}

    	/// <summary>
    	/// Removes task from storage
    	/// </summary>
    	public void RemoveTask(DataExcavatorTask DataExcavatorTask)
    	{
    		GrabbingTasksList.Remove(DataExcavatorTask);
    	}

    	/// <summary>
    	/// Clears grabbing tasks list with stopping of each task
    	/// </summary>
    	public void ClearAllTasks()
    	{
    		if (GrabbingTasksList.Count == 0)
    		{
    			return;
    		}
    		CanAddNewTask = false;
    		int StoppedTasksCount = 0;
    		for (int i = 0; i < GrabbingTasksList.Count; i++)
    		{
    			GrabbingTasksList[i].StopTask(delegate
    			{
    				StoppedTasksCount++;
    			});
    		}
    		DateTime now = DateTime.Now;
    		while (StoppedTasksCount < GrabbingTasksList.Count && !DEConfig.ShutDownProgram && !IsAllTasksStopped())
    		{
    			Thread.Sleep(10);
    			if ((DateTime.Now - now).TotalSeconds > 10.0)
    			{
    				break;
    			}
    		}
    		GrabbingTasksList.Clear();
    		CanAddNewTask = true;
    	}

    	/// <summary>
    	/// Is all tasks stopped at the time
    	/// </summary>
    	private bool IsAllTasksStopped()
    	{
    		for (int i = 0; i < GrabbingTasksList.Count; i++)
    		{
    			if (GrabbingTasksList[i].TaskState == DataExcavatorTaskState.Executing)
    			{
    				return false;
    			}
    		}
    		return true;
    	}

    	/// <summary>
    	/// Check application runned in a single copy
    	/// </summary>
    	/// <returns></returns>
    	public bool IsApplicationRunnedInASingleCopy()
    	{
    		if (OneApplicationInstanceMutex == null)
    		{
    			bool createdNew = false;
    			OneApplicationInstanceMutex = new Mutex(initiallyOwned: true, "DataExcavatorOneInstanceMutex", out createdNew);
    			if (!createdNew)
    			{
    				return false;
    			}
    		}
    		ApplicationSingleCopyLaunchedCheck = true;
    		return true;
    	}

    	/// <summary>
    	/// Initialize Data Excavator logic and licensing
    	/// </summary>
    	public void InitializeExcavator(string LicenseKey, bool ForceKeyValidationOnRemoteServer = false)
    	{
    		IOCommon.CheckAppIOPermissions();
    		if (!ApplicationSingleCopyLaunchedCheck && !IsApplicationRunnedInASingleCopy())
    		{
    			IsExcavatorInitialized = false;
    			throw new InvalidOperationException("You cannot run multiple instances of the DataExcavator application");
    		}
    		if (!IsExcavatorInitialized)
    		{
    			DataExcavatorInitializer.Initialize();
    		}
    		IsExcavatorInitialized = true;
    		string text = LicenseServer.LoadLicense(LicenseKey, ForceKeyValidationOnRemoteServer);
    		if (text != "LICENSE_OK")
    		{
    			throw LicenseValidationException.FromLicenseValidationResponse(text);
    		}
    	}

    	/// <summary>
    	/// Releases the resources of the excavator before closing it
    	/// </summary>
    	public void DisposeExcavator()
    	{
    		DEConfig.ShutDownProgram = true;
    		IsExcavatorInitialized = false;
    		CEFSharpFactory.ShutdownCEF();
    	}

    	/// <summary>
    	/// Gets actual license key copy
    	/// </summary>
    	/// <returns></returns>
    	public LicenseKey GetActualLicenseKeyCopy()
    	{
    		if (LicenseServer.ActualKey == null)
    		{
    			return null;
    		}
    		return LicenseServer.ActualKey.Clone() as LicenseKey;
    	}

    	/// <summary>
    	/// Returns tasks list overview
    	/// </summary>
    	public List<DataExcavatorTask> GetTasksList()
    	{
    		List<DataExcavatorTask> list = new List<DataExcavatorTask>();
    		for (int i = 0; i < GrabbingTasksList.Count; i++)
    		{
    			list.Add(GrabbingTasksList[i]);
    		}
    		return list;
    	}

    	/// <summary>
    	/// Returns task by it's name
    	/// </summary>
    	/// <param name="TaskName">Name of the task</param>
    	/// <returns></returns>
    	public DataExcavatorTask GetTaskByName(string TaskName)
    	{
    		for (int i = 0; i < GrabbingTasksList.Count; i++)
    		{
    			if (GrabbingTasksList[i].TaskName == TaskName)
    			{
    				return GrabbingTasksList[i];
    			}
    		}
    		return null;
    	}
    }
}
