// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Interfaces.IApproximateGrabbedDataSizeCalculable
/// <summary>
/// Interface for calculating data size of any object
/// </summary>
namespace ExcavatorSharp.Interfaces
{
	internal interface IApproximateGrabbedDataSizeCalculable
	{
		/// <summary>
		/// Calculate object complete size in bytes, including size of item fields names converted to strings
		/// </summary>
		/// <returns></returns>
		double CalculateApproximateGrabbedDataSize();
	}
}
