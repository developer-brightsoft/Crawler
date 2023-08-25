// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DataExcavatorTaskEventCallback
using System;

namespace ExcavatorSharp.Objects
{
    /// <summary>
    /// Data excavator log entry callback
    /// </summary>
    public class DataExcavatorTaskEventCallback
    {
    	/// <summary>
    	/// Log entry parent entity
    	/// </summary>
    	public DataExcavatorTasksLoggerEntityType EventEntity { get; set; }

    	/// <summary>
    	/// Event date and time
    	/// </summary>
    	public DateTime EventDateTime { get; set; }

    	/// <summary>
    	/// Event message text
    	/// </summary>
    	public string EventMessage { get; set; }

    	/// <summary>
    	/// Occured exception data
    	/// </summary>
    	public Exception OccuredException { get; set; }

    	/// <summary>
    	/// Gets assembled event data
    	/// </summary>
    	/// <returns></returns>
    	public string GetEventAssembledText()
    	{
    		string empty = string.Empty;
    		if (OccuredException == null)
    		{
    			return string.Format("[{0}, {1}]: {2}", EventDateTime.ToString("dd.MM.yyyy HH:mm:ss"), EventEntity.ToString(), EventMessage);
    		}
    		return string.Format("[{0}, {1}]: {2}, Exception = {3}", EventDateTime.ToString("dd.MM.yyyy HH:mm:ss"), EventEntity.ToString(), EventMessage, OccuredException.ToString());
    	}

    	/// <summary>
    	/// Creates new instance of DataExcavatorTaskEventCallback
    	/// </summary>
    	/// <param name="EventEntity">Log entry parent entity</param>
    	/// <param name="EventDateTime">Event date and time</param>
    	/// <param name="EventMessage">Event message text</param>
    	/// <param name="OccuredException">Occured exception data</param>
    	public DataExcavatorTaskEventCallback(DataExcavatorTasksLoggerEntityType EventEntity, DateTime EventDateTime, string EventMessage, Exception OccuredException)
    	{
    		this.EventEntity = EventEntity;
    		this.EventDateTime = EventDateTime;
    		this.EventMessage = EventMessage;
    		this.OccuredException = OccuredException;
    	}
    }
}
