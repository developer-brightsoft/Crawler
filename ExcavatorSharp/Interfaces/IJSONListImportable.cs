// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Interfaces.IJSONListImportable<T>
using System.Collections.Generic;

/// <summary>
/// Interface for any importable object that can be stored into system JSON format as ListT and fetched to any typed ListT without data loss.
/// </summary>
/// <typeparam name="T">Type of storing object</typeparam>
namespace ExcavatorSharp.Interfaces
{
	public interface IJSONListImportable<T>
	{
		/// <summary>
		/// Unpacks data from JSON array into ListT source-typed object
		/// </summary>
		/// <param name="JSONData">Object sorce data, packed in JSON</param>
		/// <returns>Typed data array</returns>
		List<T> UnserializeListFromJSON(string JSONData);
	}
}
