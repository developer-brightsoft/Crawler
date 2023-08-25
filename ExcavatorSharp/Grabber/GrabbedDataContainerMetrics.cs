// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Grabber.GrabbedDataContainerMetrics
/// <summary>
/// Class for storing grabbed data metrics
/// </summary>
namespace ExcavatorSharp.Grabber
{
	public class GrabbedDataContainerMetrics
	{
		/// <summary>
		/// Count of saved binary files
		/// </summary>
		public int BinaryFilesCount { get; set; }

		/// <summary>
		/// Is container includes data
		/// </summary>
		public bool HasResults { get; set; }

		/// <summary>
		/// Saved data total size in kilobytes
		/// </summary>
		public double GrabbedDataTotalSizeKb { get; set; }

		/// <summary>
		/// Creates a new instance of GrabbedDataContainerMetrics
		/// </summary>
		public GrabbedDataContainerMetrics()
		{
			BinaryFilesCount = 0;
			GrabbedDataTotalSizeKb = 0.0;
		}
	}
}
