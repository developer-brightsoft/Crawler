// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Exporter.ExportingDataFieldsNamingLib
/// <summary>
/// Library for storing exporting data objects names
/// </summary>
namespace ExcavatorSharp.Exporter
{
	public static class ExportingDataFieldsNamingLib
	{
		/// <summary>
		/// Name for cell or field, that stores grabbed data
		/// </summary>
		public const string GrabbedDataContentName = "GrabbedData";

		/// <summary>
		/// Name for cell or field, that stores grabbed attributes list
		/// </summary>
		public const string AttributesListContentName = "Attributes";

		/// <summary>
		/// Name for heading cell or field, that stores grabbed page url
		/// </summary>
		public const string CommonDataPageUrl = "PageURL";

		/// <summary>
		/// Name for heading cell or field, that stores grabbed data date and time
		/// </summary>
		public const string CommonDataPageGrabDateTime = "GrabDateTime";

		/// <summary>
		/// Name for cell or field, that stores PatternName block
		/// </summary>
		public const string ParsedDataPatternName = "PatternName";

		/// <summary>
		/// Name for cell or field, that stores PatternItemName block
		/// </summary>
		public const string ParsedDataPatternItemName = "PatternItemName";

		/// <summary>
		/// Name for heading cell, that stores some data
		/// </summary>
		public const string ParsedDataCellName = "{0}-Data";

		/// <summary>
		/// Name for heading cell or field, that stores some attribute name
		/// </summary>
		public const string ParsedAttributeName = "{0}-{1}-AttrName";

		/// <summary>
		/// Name for heading cell, that stores some attribute value
		/// </summary>
		public const string ParsedAttributeValue = "{0}-{1}-AttrValue";

		/// <summary>
		/// Name for heading cell, that stores attribute saved file name, if it was requested
		/// </summary>
		public const string ParsedAttributeFileLink = "{0}-{1}-AttrFileName";
	}
}