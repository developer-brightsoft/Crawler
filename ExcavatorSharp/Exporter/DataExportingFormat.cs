// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Exporter.DataExportingFormat
using System.ComponentModel;

/// <summary>
/// Types for data export
/// </summary>
namespace ExcavatorSharp.Exporter
{
	public enum DataExportingFormat
	{
		/// <summary>
		/// Exports data into .csv format, separated with ";" character
		/// </summary>
		[Description(".CSV")]
		CSV,
		/// <summary>
		/// Exports data into .xlsx format, using EPPlus library. 
		/// </summary>
		[Description(".XLSX")]
		XLSX,
		/// <summary>
		/// Exports data into .json format - inner format used.
		/// </summary>
		[Description(".JSON")]
		JSON,
		/// <summary>
		/// Exports data into .sql file (Create table macro + insert macro)
		/// </summary>
		[Description(".SQL (MySQL)")]
		MySQL
	}
}