// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Interfaces.IJSONConvertible<T>
/// <summary>
/// Interface for any exportable object that can be stored into system JSON format and fetched to any type without data loss.
/// </summary>
/// <typeparam name="T">Type of storing object</typeparam>
namespace ExcavatorSharp.Interfaces
{
	public interface IJSONConvertible<T>
	{
		/// <summary>
		/// Packs data to object array, like { a = true, c = 10, d = "FOO" }
		/// </summary>
		/// <returns>Objective presentation of class</returns>
		string SerializeToJSON();

		/// <summary>
		/// Unpacks data from JSON array into source-typed object
		/// </summary>
		/// <param name="JSONData">Object source data, packed in JSON</param>
		/// <returns>Instance of the created type</returns>
		T UnserializeFromJSON(string JSONData);
	}
}