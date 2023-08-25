// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Exporter.DataExportingType
using System.ComponentModel;

/// <summary>
/// Type of exporting data
/// </summary>
namespace ExcavatorSharp.Exporter
{
	public enum DataExportingType
	{
		/// <summary>
		/// Export inner text of each founded element
		/// </summary>
		[Description("Grabbed inner text")]
		InnerText,
		/// <summary>
		/// Export outer HTML of each founded element
		/// </summary>
		[Description("Grabbed outer html")]
		OuterHTML
	}
}