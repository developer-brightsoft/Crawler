// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DataGrabbingResult
using System;
using System.Collections.Generic;

namespace ExcavatorSharp.Objects
{
    /// <summary>
    /// Result of the grabbing for some page
    /// </summary>
    public class DataGrabbingResult
    {
    	/// <summary>
    	/// Lazy variable for property IsEmptyResultsSet
    	/// </summary>
    	private bool? IsEmptyResultSetLazy = null;

    	/// <summary>
    	/// Date and time when the page was grabbed
    	/// </summary>
    	public DateTime PageGrabbedDateTime { get; set; }

    	/// <summary>
    	/// Pattern name, parent for grabbed result
    	/// </summary>
    	public string PatternName { get; set; }

    	/// <summary>
    	/// Hash code of used pattern
    	/// </summary>
    	public int PatternHashCode { get; set; }

    	/// <summary>
    	/// URL of the grabbed page
    	/// </summary>
    	public PageLink GrabbedPageSourceUrl { get; set; }

    	/// <summary>
    	/// Page source HTML code
    	/// </summary>
    	public string PageSourceHtml { get; set; }

    	/// <summary>
    	/// Page grabbing results
    	/// </summary>
    	public List<List<KeyValuePair<DataGrabbingPatternItem, DataGrabbingResultItem>>> GrabbingResults { get; set; }

    	/// <summary>
    	/// Checks that results set is empty. Lazy works, can be invoked multiple times. Really calculating on first call.
    	/// </summary>
    	public bool IsEmptyResultsSet
    	{
    		get
    		{
    			if (!IsEmptyResultSetLazy.HasValue)
    			{
    				bool flag = false;
    				foreach (List<KeyValuePair<DataGrabbingPatternItem, DataGrabbingResultItem>> grabbingResult in GrabbingResults)
    				{
    					foreach (KeyValuePair<DataGrabbingPatternItem, DataGrabbingResultItem> item in grabbingResult)
    					{
    						if (item.Value.ResultedNodes.Count > 0)
    						{
    							flag = true;
    							break;
    						}
    					}
    					if (flag)
    					{
    						break;
    					}
    				}
    				IsEmptyResultSetLazy = !flag;
    			}
    			return IsEmptyResultSetLazy.Value;
    		}
    	}
    }
}
