// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DataGrabbingResultItemBinaryAttributeData
using ExcavatorSharp.ExtensionMethods;
using ExcavatorSharp.Interfaces;

/// <summary>
/// Class for storing parsed HTML node attribute data with linked content
/// </summary>
namespace ExcavatorSharp.Objects
{
	public class DataGrabbingResultItemBinaryAttributeData : IApproximateGrabbedDataSizeCalculable
	{
		/// <summary>
		/// Name of the HtmlNode attribute (for example, 'src' attribute for IMG tag)
		/// </summary>
		public string AttributeName { get; set; }

		/// <summary>
		/// Value of the attribute
		/// </summary>
		public string AttributeValue { get; set; }

		/// <summary>
		/// Binary content of the HtmlNode attribute. (for example, image binary data for IMG tag)
		/// </summary>
		public byte[] ResourceContent { get; set; }

		/// <summary>
		/// GUID of downloaded data. Used for saving data and it unique access.
		/// </summary>
		public string AttributeDataGuid { get; set; }

		/// <summary>
		/// Creates a new instance of DataGrabbingResultItemBinaryAttributeData
		/// </summary>
		/// <param name="AttributeName">Name of the HtmlNode attribute (for example, 'src' attribute for IMG tag)</param>
		/// <param name="AttributeValue">Value of the attribute</param>
		/// <param name="ResourceContent">Binary content of the HtmlNode attribute. (for example, image binary data for IMG tag)</param>
		/// <param name="AttributeDataGuid">Downloaded resource unique identifyer</param>
		public DataGrabbingResultItemBinaryAttributeData(string AttributeName, string AttributeValue, byte[] ResourceContent, string AttributeDataGuid)
		{
			this.AttributeName = AttributeName;
			this.AttributeValue = AttributeValue;
			this.ResourceContent = ResourceContent;
			this.AttributeDataGuid = AttributeDataGuid;
		}

		/// <summary>
		/// Calculates approximate size of object
		/// </summary>
		/// <returns></returns>
		public double CalculateApproximateGrabbedDataSize()
		{
			double num = 0.0;
			num += AttributeName.GetStringApproximateSizeInBytes();
			num += AttributeValue.GetStringApproximateSizeInBytes();
			num += AttributeDataGuid.GetStringApproximateSizeInBytes();
			return num + (double)((ResourceContent != null) ? ResourceContent.Length : 0);
		}
	}
}
