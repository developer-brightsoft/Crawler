// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Common.HTTPDataURLContent
/// <summary>
/// Parsed DataUrl entry content
/// </summary>
namespace ExcavatorSharp.Common
{
	public class HTTPDataURLContent
	{
		/// <summary>
		/// Entry value
		/// </summary>
		public string base64Data { get; set; }

		/// <summary>
		/// Entry full data type
		/// </summary>
		public string contentType { get; set; }

		/// <summary>
		/// Detailed ormat, if data contains '/' character
		/// </summary>
		public string contentDetails { get; set; }
	}
}
