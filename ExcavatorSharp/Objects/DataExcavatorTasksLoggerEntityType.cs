// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DataExcavatorTasksLoggerEntityType
using System.ComponentModel;

/// <summary>
/// Task entities
/// </summary>
namespace ExcavatorSharp.Objects
{
	public enum DataExcavatorTasksLoggerEntityType
	{
		/// <summary>
		/// Grabbing server - entity for data grabbing
		/// </summary>
		[Description("GrabbingServer")]
		GrabbingServer,
		/// <summary>
		/// Crawling server - entity for data crawling
		/// </summary>
		[Description("CrawlingServer")]
		CrawlingServer,
		/// <summary>
		/// Data exporter - entity for data export
		/// </summary>
		[Description("DataExporter")]
		DataExporter,
		/// <summary>
		/// Common entitiy for common events
		/// </summary>
		[Description("CommonEntity")]
		CommonEntity
	}
}
