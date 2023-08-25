// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Licensing.LicenseActivationResponse
/// <summary>
/// Class for storing License activation response data
/// </summary>
namespace ExcavatorSharp.Licensing
{
	public class LicenseActivationResponse
	{
		/// <summary>
		/// Is license activated OK
		/// </summary>
		public bool IsLicenseActivated { get; set; }

		/// <summary>
		/// License activation response text
		/// </summary>
		public string LicenseActivationResponseText { get; set; }

		/// <summary>
		/// Creates new instance of LicenseActivationResponse
		/// </summary>
		/// <param name="IsLicenseActivated">is license activated</param>
		/// <param name="LicenseActivationResponseText">License activation message</param>
		public LicenseActivationResponse(bool IsLicenseActivated, string LicenseActivationResponseText)
		{
			this.IsLicenseActivated = IsLicenseActivated;
			this.LicenseActivationResponseText = LicenseActivationResponseText;
		}
	}
}
