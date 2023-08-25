// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Exporter.DataExportingProcessStat
/// <summary>
/// Data exporter statistics container
/// </summary>
namespace ExcavatorSharp.Exporter
{
	public class DataExportingProcessStat
	{
		/// <summary>
		/// Total records to export
		/// </summary>
		public int TotalEntriesCount { get; set; }

		/// <summary>
		/// Actual exporting entry nr
		/// </summary>
		public int ActualEntryNr { get; set; }

		/// <summary>
		/// Actual completion percentage
		/// </summary>
		public double ExportCompletionPercentage { get; set; }

		/// <summary>
		/// Unsuccessfully exported records count
		/// </summary>
		public int UnsuccesfullyExportedRecords { get; set; }

		/// <summary>
		/// Creates a new instance of DataExportingProcessStat
		/// </summary>
		/// <param name="TotalEntriesCount">Total records to export</param>
		/// <param name="ActualEntryNr">Actual exporting entry nr</param>
		/// <param name="ExportCompletionPercentage">Actual completion percentage</param>
		/// <param name="UnsuccesfullyExportedRecords">Unsuccessfully exported records count</param>
		public DataExportingProcessStat(int TotalEntriesCount, int ActualEntryNr, double ExportCompletionPercentage, int UnsuccesfullyExportedRecords)
		{
			this.TotalEntriesCount = TotalEntriesCount;
			this.ActualEntryNr = ActualEntryNr;
			this.ExportCompletionPercentage = ExportCompletionPercentage;
			this.UnsuccesfullyExportedRecords = UnsuccesfullyExportedRecords;
		}
	}
}