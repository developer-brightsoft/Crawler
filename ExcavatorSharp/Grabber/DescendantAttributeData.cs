// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Grabber.DescendantAttributeData
/// <summary>
/// Parsed attribute data
/// </summary>
namespace ExcavatorSharp.Grabber
{
	public class DescendantAttributeData
	{
		/// <summary>
		/// Name of attribute
		/// </summary>
		public string AttributeName { get; set; }

		/// <summary>
		/// Value of attribute
		/// </summary>
		public string AttributeValue { get; set; }

		/// <summary>
		/// Is attribute has content
		/// </summary>
		public bool ContentNotNullable { get; set; }

		/// <summary>
		/// Attribute saved file name in binary-data folder
		/// </summary>
		public string AttributeSavedFileName { get; set; }

		/// <summary>
		/// Is attribute data saves correctly
		/// </summary>
		public bool IsFileSuccesfullySaved { get; set; }
	}
}