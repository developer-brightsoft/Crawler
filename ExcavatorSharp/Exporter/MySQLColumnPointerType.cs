// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Exporter.MySQLColumnPointerType
/// <summary>
/// Types of MySQLColumnPointer
/// </summary>
namespace ExcavatorSharp.Exporter
{
	internal enum MySQLColumnPointerType
	{
		/// <summary>
		/// Pointer to parsed content
		/// </summary>
		ContentData,
		/// <summary>
		/// Pointer to parsed content attribute
		/// </summary>
		AttributeData,
		/// <summary>
		/// Pointer to saved attribute binary data
		/// </summary>
		AttributeFileName
	}
}