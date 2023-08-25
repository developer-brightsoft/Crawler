// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Captcha.CaptchaSolverBase
using System.Net;
using ExcavatorSharp.Crawler;

namespace ExcavatorSharp.Captcha
{
    /// <summary>
    /// Abstract class for Captcha resolver
    /// </summary>
    internal abstract class CaptchaSolverBase
    {
    	/// <summary>
    	/// Captcha resolver settings
    	/// </summary>
    	protected CAPTCHASolverSettings CaptchaResolverSettings { get; set; }

    	/// <summary>
    	/// Link to parent crawling thread
    	/// </summary>
    	protected CrawlingThreadCEF CrawlingThreadLink { get; set; }

    	/// <summary>
    	/// Creates new instance of CaptchaResolverBase
    	/// </summary>
    	/// <param name="ResolverSettings"></param>
    	public CaptchaSolverBase(CrawlingThreadCEF CrawlingThreadLink)
    	{
    		CaptchaResolverSettings = CrawlingThreadLink.CrawlingServerEnvironmentLink.CrawlerProperties.CaptchaSettings;
    		this.CrawlingThreadLink = CrawlingThreadLink;
    	}

    	/// <summary>
    	/// Try to resolve captcha and return resolving results
    	/// </summary>
    	/// <returns></returns>
    	internal abstract CaptchaResolveResults TryToSolveCaptcha(string PageURL, string PageHTMLSource, WebProxy UsingProxy);

    	/// <summary>
    	/// Dedect captcha - is captcha showed on the page
    	/// </summary>
    	/// <returns></returns>
    	internal abstract bool IsCaptchaShown(string PageURL, string PageHTMLSource);
    }
}
