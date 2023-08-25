// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Interfaces.IDataContextualExportable
using System.Collections.Generic;
using ExcavatorSharp.Excavator;
using ExcavatorSharp.Exporter;
using ExcavatorSharp.Grabber;
using ExcavatorSharp.Objects;

namespace ExcavatorSharp.Interfaces
{
	/// <summary>
	/// Interface for any exporter that can save data from PatternDataGroup result to some format, like xlsx, csv or others.
	/// </summary>
	public interface IDataContextualExportable
	{
		/// <summary>
		/// Initialize exporter with specified data saving path
		/// </summary>
		/// <param name="FileSavingPath">Path to exporting file</param>
		/// <param name="FoundedDataExportingType">Type of exporting data</param>
		/// <param name="ItemsSequencesSeparator">Separator for separating data sequences, if some data includes many items</param>
		/// <param name="ProjectPatternsList">All project patterns list from first grabbing</param>
		void InitializeExpoter(string FileSavingPath, DataExportingType FoundedDataExportingType, string ItemsSequencesSeparator, List<DataGrabbingPattern> ProjectPatternsList, DataExcavatorTasksLogger TaskLogger);

		/// <summary>
		/// Adds some part of the results set to resulted file
		/// </summary>
		/// <param name="ExportingData">Grabbed data list</param>
		void AddDataIntoExportingFile(List<GrabbedDataGroup> ExportingData);

		/// <summary>
		/// Finishes export and closes exporting file
		/// </summary>
		void FinishExport();
	}

}
