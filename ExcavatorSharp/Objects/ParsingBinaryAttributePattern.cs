// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.ParsingBinaryAttributePattern
/// <summary>
/// Class for storing data about how to parse some binary attribute
/// </summary>
namespace ExcavatorSharp.Objects
{
	public class ParsingBinaryAttributePattern
	{
		/// <summary>
		/// Attribute name we want to parse
		/// </summary>
		public string AttributeName { get; set; }

		/// <summary>
		/// Is the attribute a link to some binary resource, like 'http://link-to-resource-file.com/file1.jpg'?
		/// </summary>
		public bool IsAttributeAreLinkToSomeResouce { get; set; }

		/// <summary>
		/// It is necessary to download the attribute value by the attribute link, if attribute contains a link
		/// </summary>
		public bool IsWeMustDownloadContentUnderAttributeLink { get; set; }

		/// <summary>
		/// Creates a new instance of ParsingBinaryAttributePattern
		/// </summary>
		/// <param name="AttributeName">Attribute name we want to parse</param>
		/// <param name="IsAttributeAreLinkToSomeResouce">Is the attribute a link to some binary resource, like 'http://link-to-resource-file.com/file1.jpg'?</param>
		/// <param name="IsWeMustDownloadContentUnderAttributeLink">It is necessary to download the attribute value by the attribute link, if attribute contains a link</param>
		public ParsingBinaryAttributePattern(string AttributeName, bool IsAttributeAreLinkToSomeResouce, bool IsWeMustDownloadContentUnderAttributeLink)
		{
			this.AttributeName = AttributeName;
			this.IsAttributeAreLinkToSomeResouce = IsAttributeAreLinkToSomeResouce;
			this.IsWeMustDownloadContentUnderAttributeLink = IsWeMustDownloadContentUnderAttributeLink;
		}

		/// <summary>
		/// Returns hash code of Parsing Binary Attribute
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return AttributeName.GetHashCode() + IsAttributeAreLinkToSomeResouce.GetHashCode() + IsWeMustDownloadContentUnderAttributeLink.GetHashCode();
		}
	}
}