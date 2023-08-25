// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Captcha.CR2CaptchaFirstResponseData
using Newtonsoft.Json;

/// <summary>
/// Captcha first response data
/// </summary>

namespace ExcavatorSharp.Captcha
{
	internal class CR2CaptchaFirstResponseData
	{
		/// <summary>
		/// Response status
		/// </summary>
		[JsonProperty("status")]
		public int Status { get; set; }

		/// <summary>
		/// Request code
		/// </summary>
		[JsonProperty("request")]
		public string RequestCode { get; set; }
	}
}
