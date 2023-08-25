// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.PageGrabbedCallback
using System.Collections.Generic;

namespace ExcavatorSharp.Objects
{
    /// <summary>
    /// Grabbed callback, that creates at every page grabbed
    /// </summary>
    public class PageGrabbedCallback
    {
    	/// <summary>
    	/// Initial grabbing task
    	/// </summary>
    	public DataGrabbingTask GrabbingTask { get; private set; }

    	/// <summary>
    	/// Matched patterns count
    	/// </summary>
    	public int MatchedPatternsCount { get; private set; }

    	/// <summary>
    	/// Result of the grabbing
    	/// </summary>
    	public Dictionary<DataGrabbingPattern, DataGrabbingResult> GrabbingResults { get; private set; }

    	/// <summary>
    	/// Creates page grabbed callback
    	/// </summary>
    	/// <param name="GrabbingTask">Initial grabbing task</param>
    	/// <param name="GrabbingResult">Result of the grabbing</param>
    	public PageGrabbedCallback(DataGrabbingTask GrabbingTask, Dictionary<DataGrabbingPattern, DataGrabbingResult> GrabbingResults, int MatchedPatternsCount)
    	{
    		this.GrabbingTask = GrabbingTask;
    		this.GrabbingResults = GrabbingResults;
    		this.MatchedPatternsCount = MatchedPatternsCount;
    	}
    }
}
