// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DERobotsTxtCleanParamInnerRow
using System;
using ExcavatorSharp.Crawler;
using Newtonsoft.Json;
namespace ExcavatorSharp.Objects
{
    /// <summary>
    /// Helper for operating with clean-param directive
    /// </summary>
    public class DERobotsTxtCleanParamInnerRow : DERobotsTXTInnerRow
    {
    	/// <summary>
    	/// List of cleaning parameters
    	/// </summary>
    	public string[] CleaningParams { get; set; }

    	[JsonIgnore]
    	internal string[] CleaningParamsStrReplacePrepared { get; set; }

    	/// <summary>
    	/// Creates a new instance of DERobotsTxtEntry
    	/// </summary>
    	/// <param name="Param">Param name</param>
    	/// <param name="Value">Param value</param>
    	/// <param name="RowOriginalPosition">Row initial position into file</param>
    	/// <param name="UserAgent">Parameter user-agent</param>
    	public DERobotsTxtCleanParamInnerRow(string UserAgent, DERobotsEntryType Param, string Value, int RowOriginalPosition, string RowType)
    		: base(UserAgent, Param, Value, RowOriginalPosition, RowType)
    	{
    		string empty = string.Empty;
    		string value = string.Empty;
    		if (Value.IndexOf(" ") != -1)
    		{
    			string[] array = Value.Split(' ');
    			empty = array[0];
    			value = array[1];
    		}
    		else
    		{
    			empty = Value;
    		}
    		CleaningParams = empty.Split(new string[1] { "&" }, StringSplitOptions.RemoveEmptyEntries);
    		base.Value = value;
    		CleaningParamsStrReplacePrepared = new string[CleaningParams.Length * 3];
    		int num = 0;
    		string[] cleaningParams = CleaningParams;
    		foreach (string arg in cleaningParams)
    		{
    			CleaningParamsStrReplacePrepared[num++] = $"?{arg}=";
    			CleaningParamsStrReplacePrepared[num++] = $"&{arg}=";
    			CleaningParamsStrReplacePrepared[num++] = $"{arg}=";
    		}
    	}

    	/// <summary>
    	/// Creates new instance of DERobotsTxtCleanParamInnerRow from RAW data
    	/// </summary>
    	/// <returns></returns>
    	public static DERobotsTxtCleanParamInnerRow FromRawData(DERobotsTXTInnerRow RawDataObject)
    	{
    		return new DERobotsTxtCleanParamInnerRow(RawDataObject.UserAgent, RawDataObject.ParamName, RawDataObject.Value, RawDataObject.RowOriginalPosition, RawDataObject.RowType);
    	}
    }
}
