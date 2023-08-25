// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Captcha.CAPTCHASolverSettings
using System.Collections.Generic;

namespace ExcavatorSharp.Captcha
{
    /// <summary>
    /// Settings for captha resolving
    /// </summary>
    public class CAPTCHASolverSettings
    {
    	/// <summary>
    	/// Service used for CAPTCHA recognition
    	/// </summary>
    	public CaptchaSolvingService SolvingService { get; set; }

    	/// <summary>
    	/// Force specified capcha type, ignore other captchas - for "Force specified captcha flag"
    	/// </summary>
    	public CaptchaType ForceSpecifiedCaptchaType { get; set; }

    	/// <summary>
    	/// Captcha detection substring - for "Force specified captcha flag"
    	/// </summary>
    	public string ForceSpecifiedCaptchaDetectionSubstring { get; set; }

    	/// <summary>
    	/// Wait after captcha was resolved, it's recommended to set this value to 5
    	/// </summary>
    	public int WaitAfterCaptchaWasSolvedSeconds { get; set; }

    	/// <summary>
    	/// Captcha resolving attempts, it's recommended to set this value to 3
    	/// </summary>
    	public int CaptchaSolvingAttempts { get; set; }

    	/// <summary>
    	/// Captcha resolving timeout. It is recommended to set to 50-60 secongs for ReCaptcha and HCaptcha.
    	/// </summary>
    	public int CaptchaSolvingTimeoutSeconds { get; set; }

    	/// <summary>
    	/// Additional JavaScript code to run after the captcha was completed
    	/// </summary>
    	public string AdditionalJavaScriptAfterCaptchaPassed { get; set; }

    	/// <summary>
    	/// Connection params to captcha resolving service
    	/// </summary>
    	public Dictionary<string, string> CaptchaSolverAdditionalParams { get; set; }

    	/// <summary>
    	/// Create new instance of CAPTCHAResolverSettings
    	/// </summary>
    	/// <param name="ForceSpecifiedCaptchaType">Force specified capcha type, ignore other captchas - for "Force specified captcha flag"</param>
    	/// <param name="ForceSpecifiedCaptchaDetectionSubstring">Captcha detection substring - for "Force specified captcha flag"</param>
    	/// <param name="WaitAfterCaptchaWasSolvedSeconds">Wait after captcha was resolved, it's recommended to set this value to 5</param>
    	/// <param name="CaptchaSolvingAttempts">Captcha resolving attempts, it's recommended to set this value to 3</param>
    	/// <param name="CaptchaSolvingTimeoutSeconds">Captcha resolving timeout. It is recommended to set to 50-60 secongs for ReCaptcha and HCaptcha.</param>
    	/// <param name="AdditionalJavaScriptAfterCaptchaPassed">Additional JavaScript code to run after the captcha was completed</param>
    	/// <param name="CaptchaSolverAdditionalParams">Connection params to captcha resolving service</param>
    	public CAPTCHASolverSettings(CaptchaSolvingService SolvingService, CaptchaType ForceSpecifiedCaptchaType, string ForceSpecifiedCaptchaDetectionSubstring, int WaitAfterCaptchaWasSolvedSeconds, int CaptchaSolvingAttempts, int CaptchaSolvingTimeoutSeconds, string AdditionalJavaScriptAfterCaptchaPassed, Dictionary<string, string> CaptchaSolverAdditionalParams)
    	{
    		this.SolvingService = SolvingService;
    		this.ForceSpecifiedCaptchaType = ForceSpecifiedCaptchaType;
    		this.ForceSpecifiedCaptchaDetectionSubstring = ForceSpecifiedCaptchaDetectionSubstring;
    		this.WaitAfterCaptchaWasSolvedSeconds = WaitAfterCaptchaWasSolvedSeconds;
    		this.CaptchaSolvingAttempts = CaptchaSolvingAttempts;
    		this.CaptchaSolvingTimeoutSeconds = CaptchaSolvingTimeoutSeconds;
    		this.AdditionalJavaScriptAfterCaptchaPassed = AdditionalJavaScriptAfterCaptchaPassed;
    		this.CaptchaSolverAdditionalParams = CaptchaSolverAdditionalParams;
    	}
    }
}
