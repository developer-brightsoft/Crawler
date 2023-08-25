// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Captcha.CR2CaptchaProcessingResponseData
/// <summary>
/// Captcha processing response data
/// </summary>
namespace ExcavatorSharp.Captcha
{
	internal class CR2CaptchaProcessingResponseData
	{
		/// <summary>
		/// Service resolving response
		/// </summary>
		public string ResponseData { get; set; }

		/// <summary>
		/// Is resolved without error
		/// </summary>
		public bool IsResolvedOK { get; set; }

		/// <summary>
		/// Get solved data
		/// </summary>
		public string CaptchaResolvedDataValue
		{
			get
			{
				string[] array = ResponseData.Split('|');
				if (array.Length < 2)
				{
					return string.Empty;
				}
				return array[1];
			}
		}

		/// <summary>
		/// Create new instance of CR2CaptchaProcessingResponseData
		/// </summary>
		/// <param name="IsResolvedOK">Is resolved without error</param>
		/// <param name="ResponseData">Service resolving response</param>
		public CR2CaptchaProcessingResponseData(bool IsResolvedOK, string ResponseData)
		{
			this.IsResolvedOK = IsResolvedOK;
			this.ResponseData = ResponseData;
		}
	}
}
