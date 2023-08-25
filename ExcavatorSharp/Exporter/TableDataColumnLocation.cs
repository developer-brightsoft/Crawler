// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Exporter.TableDataColumnLocation
using System.Collections.Generic;
using System.Linq;

namespace ExcavatorSharp.Exporter
{
    /// <summary>
    /// Class for storing data in tables, that bufferes information about some field location into Excel file.
    /// </summary>
    public class TableDataColumnLocation
    {
    	/// <summary>
    	/// GUID of some pattern
    	/// </summary>
    	public int PatternHash { get; set; }

    	/// <summary>
    	/// Name of a some column on some sheet we want to locate
    	/// </summary>
    	public string ColumnName { get; set; }

    	/// <summary>
    	/// Pattern sheet index
    	/// </summary>
    	public int CoordsSheetIndex { get; set; }

    	/// <summary>
    	/// Pattern column index
    	/// </summary>
    	public int CoordsColumnIndex { get; set; }

    	/// <summary>
    	/// Creates a new instance of TableDataColumnLocation
    	/// </summary>
    	/// <param name="PatternGUID">GUID of some pattern</param>
    	/// <param name="ColumnName">Name of a some column on some sheet we want to locate</param>
    	/// <param name="CoordsSheetIndex">Pattern sheet index</param>
    	/// <param name="CoordsColumnIndex">Pattern column index</param>
    	public TableDataColumnLocation(int PatternGUID, string ColumnName, int CoordsSheetIndex, int CoordsColumnIndex)
    	{
    		PatternHash = PatternGUID;
    		this.ColumnName = ColumnName;
    		this.CoordsSheetIndex = CoordsSheetIndex;
    		this.CoordsColumnIndex = CoordsColumnIndex;
    	}

    	/// <summary>
    	/// Returns sheet index by pattern hash code
    	/// </summary>
    	/// <param name="KnownAssociations">Known columns associations</param>
    	/// <param name="PatternHash">Pattern hash</param>
    	/// <returns>Sheet index</returns>
    	public static int GetSheetIndexByPatternHash(int PatternHash, List<TableDataColumnLocation> KnownAssociations)
    	{
    		return KnownAssociations.Where((TableDataColumnLocation item) => item.PatternHash == PatternHash).FirstOrDefault()?.CoordsSheetIndex ?? (-1);
    	}

    	/// <summary>
    	/// Returns data column index by data name and PatternHash
    	/// </summary>
    	/// <param name="PatternHash">Pattern hash</param>
    	/// <param name="DataName">Name fo searching data</param>
    	/// <param name="KnownAssociations">Known columns associations</param>
    	/// <returns>Column index</returns>
    	public static int GetColumnIndexByItemDataName(int PatternHash, string DataName, List<TableDataColumnLocation> KnownAssociations)
    	{
    		return KnownAssociations.Where((TableDataColumnLocation item) => item.PatternHash == PatternHash && item.ColumnName == DataName).FirstOrDefault()?.CoordsColumnIndex ?? (-1);
    	}
    }
}
