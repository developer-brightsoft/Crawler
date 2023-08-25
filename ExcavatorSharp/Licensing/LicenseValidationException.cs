// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Licensing.LicenseValidationException
using System;

namespace ExcavatorSharp.Licensing
{
    /// <summary>
    /// License validation exception
    /// </summary>
    public class LicenseValidationException : Exception
    {
    	public LicenseValidationException(string Message)
    		: base(Message)
    	{
    	}

    	/// <summary>
    	/// Creates new instance of LicenseValidationException from LicenseServer.LoadLicense response code
    	/// </summary>
    	/// <param name="LicenseValidationResponse"></param>
    	/// <returns></returns>
    	public static LicenseValidationException FromLicenseValidationResponse(string LicenseValidationResponse = "")
    	{
    		return LicenseValidationResponse switch
    		{
    			"ERROR_WRONG_KEY_FORMAT" => new LicenseValidationException("License validation error: wrong key format"), 
    			"ERROR_WRONG_KEY_DATA" => new LicenseValidationException("License validation error: bad key data"), 
    			"ERROR_LICENSE_EXPIRED" => new LicenseValidationException("License validation error: license key expired"), 
    			"ERROR_WRONGPRODUCTCODE" => new LicenseValidationException("License validation error: wrong product code"), 
    			"ERROR_LICENSE_TOOMUCHACTIVATIONS" => new LicenseValidationException("License validation error: activations limit excceeded"), 
    			"ERROR_LICENSE_IS_DEACTIVATED" => new LicenseValidationException("License validation error: license was deactivated"), 
    			"ERROR_CANTACTIVATEKEYREMOTE" => new LicenseValidationException("License validation error: can't activate key on remote server"), 
    			"ERROR_CANTACTIVATEDEMOKEY" => new LicenseValidationException("License validation error: you can't activate another one demo key on this machine"), 
    			"ERROR_REGISTRYACCESS" => new LicenseValidationException("License validation error: can't access to Win registry"), 
    			"ERROR_LICENSENOTAPPLICABLE" => new LicenseValidationException("License key is not applicable for this product"), 
    			_ => new LicenseValidationException("License error"), 
    		};
    	}

    	/// <summary>
    	/// Creates new instance of LicenseValidationException if user tries to specify more threads count that specifie in used license key
    	/// </summary>
    	/// <returns></returns>
    	public static LicenseValidationException FromCrawlingServerIfWrongThreadsCountRequested()
    	{
    		return new LicenseValidationException("License error: too much threads count for your license key. Section = CrawlingServerProperties.CrawlingThreadsCount");
    	}

    	/// <summary>
    	/// Creates new instance of LicenseValidationException if user tries to specify more threads count that specifie in used license key
    	/// </summary>
    	/// <returns></returns>
    	public static LicenseValidationException FromGrabbingServerIfWrongThreadsCountRequested()
    	{
    		return new LicenseValidationException("License error: too much threads count for your license key. Section = GrabbingServerProperties.GrabbingThreadsCount");
    	}

    	/// <summary>
    	/// Creates new instance of LicenseValidationException is user tries to add more projects then allowed in actual license
    	/// </summary>
    	/// <returns></returns>
    	public static LicenseValidationException FromDETaskIfWrongTotalTasksCountRequested()
    	{
    		return new LicenseValidationException("License error: too much projects for your license key. Section = DataExcavatorTasksFactory.AddTask");
    	}
    }
}
