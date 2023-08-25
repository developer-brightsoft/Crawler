// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Captcha.CaptchaType
using System.ComponentModel;

/// <summary>
/// Supportec captchas types
/// </summary>
namespace ExcavatorSharp.Captcha
{
	public enum CaptchaType
	{
		/// <summary>
		/// No captcha used
		/// </summary>
		[Description("No captcha used")]
		NoCaptcha,
		/// <summary>
		/// hCaptcha used
		/// </summary>
		[Description("hCaptcha")]
		HCaptcha,
		/// <summary>
		/// ReCaptcha V2 used
		/// </summary>
		[Description("ReCaptchaV2")]
		RecaptchaV2,
		/// <summary>
		/// ReCaptcha V3 used
		/// </summary>
		[Description("ReCaptchaV3")]
		RecaptchaV3,
		/// <summary>
		/// Image captcha used (image with text)
		/// </summary>
		[Description("Image captcha - any image with text")]
		ImageCaptcha,
		/// <summary>
		/// Text captcha used (block with any text competition)
		/// </summary>
		[Description("Text captcha - any text competition")]
		TextCaptcha,
		/// <summary>
		/// User-defined captcha
		/// </summary>
		[Description("User-defined captcha")]
		UserDefinedCaptcha,
		/// <summary>
		/// Auto-detect captcha
		/// </summary>
		[Description("Unknown captcha - auto detect")]
		UnknownCaptchaAutoDetect
	}
}
