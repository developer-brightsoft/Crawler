// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Exporter.CSVExportingFilePtr
/// <summary>
/// Pointer to exporting CSV file
/// </summary>
namespace ExcavatorSharp.Exporter
{
	internal class CSVExportingFilePtr
	{
		/// <summary>
		/// Sequence index of file
		/// </summary>
		public int FileIndex { get; set; }

		/// <summary>
		/// File name
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// Creates a new instance of CSVExportingFilePtr
		/// </summary>
		/// <param name="FileIndex">Sequence index of file</param>
		/// <param name="FileName">File name</param>
		public CSVExportingFilePtr(int FileIndex, string FileName)
		{
			this.FileIndex = FileIndex;
			this.FileName = FileName;
		}
	}
}