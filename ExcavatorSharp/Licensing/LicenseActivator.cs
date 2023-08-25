// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Licensing.LicenseActivator
using System;
using System.IO;
using ExcavatorSharp.Common;
using ExcavatorSharp.Excavator;

namespace ExcavatorSharp.Licensing
{
    /// <summary>
    /// Licenses activator and storage
    /// </summary>
    public class LicenseActivator
    {
    	/// <summary>
    	/// File for storing license data
    	/// </summary>
    	internal const string LicenseFileName = "license.key";

    	/// <summary>
    	/// Links to main tasks factory object
    	/// </summary>
    	private DataExcavatorTasksFactory TasksFactoryLink { get; set; }

    	/// <summary>
    	/// Creates a new instance of LicenseActivator
    	/// </summary>
    	/// <param name="FactoryLink"></param>
    	public LicenseActivator(DataExcavatorTasksFactory TasksFactoryLink)
    	{
    		this.TasksFactoryLink = TasksFactoryLink;
    	}

    	/// <summary>
    	/// Tries to read actual license on program loading
    	/// </summary>
    	public void TryToReadLicenseOnload(string LicensePredefined = "")
    	{
    		string path = string.Format("{0}/{1}", IOCommon.GetDataExcavatorCommonIOPath(), "license.key");
    		string text = string.Empty;
    		if (LicensePredefined != string.Empty)
    		{
    			try
    			{
    				File.WriteAllText(path, LicensePredefined);
    				text = LicensePredefined;
    			}
    			catch (Exception)
    			{
    			}
    		}
    		if (File.Exists(path))
    		{
    			try
    			{
    				text = File.ReadAllText(path);
    			}
    			catch (Exception)
    			{
    			}
    		}
    		if (text != string.Empty)
    		{
    			try
    			{
    				TasksFactoryLink.InitializeExcavator(text);
    			}
    			catch (LicenseValidationException)
    			{
    			}
    		}
    	}

    	/// <summary>
    	/// Activates some product key
    	/// </summary>
    	/// <param name="Key"></param>
    	public LicenseActivationResponse TryToActivateNewKey(string Key)
    	{
    		try
    		{
    			TasksFactoryLink.InitializeExcavator(Key, ForceKeyValidationOnRemoteServer: true);
    			string path = string.Format("{0}/{1}", IOCommon.GetDataExcavatorCommonIOPath(), "license.key");
    			File.WriteAllText(path, Key);
    			return new LicenseActivationResponse(IsLicenseActivated: true, string.Format("License activated and valid till {0}", TasksFactoryLink.GetActualLicenseKeyCopy().KeyExpirationDate.ToString("yyyy-MM-dd")));
    		}
    		catch (LicenseValidationException ex)
    		{
    			return new LicenseActivationResponse(IsLicenseActivated: false, ex.Message);
    		}
    		catch (Exception)
    		{
    			return new LicenseActivationResponse(IsLicenseActivated: false, "License activation IO error - license not saved");
    		}
    	}
    }
}
