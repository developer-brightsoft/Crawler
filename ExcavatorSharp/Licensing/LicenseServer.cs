// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Licensing.LicenseServer
using System;
using System.Net;
using ExcavatorSharp.Common;
using Microsoft.Win32;
using Newtonsoft.Json;
using RestSharp;

namespace ExcavatorSharp.Licensing
{
    /// <summary>
    /// Licensing server
    /// </summary>
    internal static class LicenseServer
    {
    	/// <summary>
    	/// Licensing model
    	/// </summary>
    	private const string LibBuildModel = "LIB_ONLY";

    	/// <summary>
    	/// Application product code
    	/// </summary>
    	internal const string ProductCode = "DEAPP2019";

    	/// <summary>
    	/// Standart response for license success validation
    	/// </summary>
    	internal const string LicenseOKResponse = "LICENSE_OK";

    	/// <summary>
    	/// Standart response for license wrong key format
    	/// </summary>
    	internal const string LicenseErrorWrongKeyFormatResponse = "ERROR_WRONG_KEY_FORMAT";

    	/// <summary>
    	/// Standart response for license wrong key data
    	/// </summary>
    	internal const string LicenseErrorWrongKeyDataResponse = "ERROR_WRONG_KEY_DATA";

    	/// <summary>
    	/// Standart response for license expired
    	/// </summary>
    	internal const string LicenseErrorKeyExpiredResponse = "ERROR_LICENSE_EXPIRED";

    	/// <summary>
    	/// Standart response for license with activation limit excceeded
    	/// </summary>
    	internal const string LicenseErrorKeyTooMuchActivations = "ERROR_LICENSE_TOOMUCHACTIVATIONS";

    	/// <summary>
    	/// Standart response for deactivated license keys
    	/// </summary>
    	internal const string LicenseErrorKeyLicenseWasDeactivated = "ERROR_LICENSE_IS_DEACTIVATED";

    	/// <summary>
    	/// Standart response for wrong product code
    	/// </summary>
    	internal const string LicenseErrorWrongProductCodeResponse = "ERROR_WRONGPRODUCTCODE";

    	/// <summary>
    	/// Cant activate key remote - some error
    	/// </summary>
    	internal const string LicenseErrorCantActivateKeyRemote = "ERROR_CANTACTIVATEKEYREMOTE";

    	/// <summary>
    	/// Cant activate another one demo key - demo key was already activated
    	/// </summary>
    	internal const string LicenseErrorCantActivateDemoKey = "ERROR_CANTACTIVATEDEMOKEY";

    	/// <summary>
    	/// Error accessing registry
    	/// </summary>
    	internal const string LicenseErrorRegistryKeyProblems = "ERROR_REGISTRYACCESS";

    	/// <summary>
    	/// Error in license applicability
    	/// </summary>
    	internal const string LicenseErrorNotApplicableForThisProduct = "ERROR_LICENSENOTAPPLICABLE";

    	/// <summary>
    	/// Actual license key
    	/// </summary>
    	internal static LicenseKey ActualKey { get; set; }

    	/// <summary>
    	/// Loads license
    	/// </summary>
    	internal static string LoadLicense(string LicenseKey, bool ForceKeyValidationOnRemoteServer = false)
    	{
    		LicensingCryptography licensingCryptography = new LicensingCryptography();
    		string empty = string.Empty;
    		try
    		{
    			empty = licensingCryptography.DecryptString(LicenseKey);
    		}
    		catch (Exception)
    		{
    			return "ERROR_WRONG_KEY_FORMAT";
    		}
    		try
    		{
    			ActualKey = JsonConvert.DeserializeObject<LicenseKey>(empty);
    			ActualKey.LicenseKeyData = LicenseKey;
    		}
    		catch (Exception)
    		{
    			ActualKey = null;
    			return "ERROR_WRONG_KEY_DATA";
    		}
    		if (ActualKey.ProductCode != "DEAPP2019")
    		{
    			ActualKey = null;
    			return "ERROR_WRONGPRODUCTCODE";
    		}
    		DateTime now = DateTime.Now;
    		if (now > ActualKey.KeyExpirationDate)
    		{
    			ActualKey = null;
    			return "ERROR_LICENSE_EXPIRED";
    		}
    		bool flag = false;
    		string text = CheckLicenseKeyOnRemoteServer(LicenseKey, ForceKeyValidationOnRemoteServer);
    		if (text != "LICENSE_OK")
    		{
    			ActualKey = null;
    			return text;
    		}
    		if (ActualKey.KeyID != 163 && ActualKey.KeyType == 1)
    		{
    			if (!CanActivateDemoKey())
    			{
    				ActualKey = null;
    				return "ERROR_CANTACTIVATEDEMOKEY";
    			}
    			SetDemoKeyWasActivated();
    		}
    		return "LICENSE_OK";
    	}

    	/// <summary>
    	/// Checks license key
    	/// </summary>
    	/// <returns></returns>
    	internal static bool CheckLicenseKeyValid()
    	{
    		if (ActualKey == null)
    		{
    			return false;
    		}
    		if (ActualKey.ProductCode != "DEAPP2019")
    		{
    			return false;
    		}
    		DateTime now = DateTime.Now;
    		if (now > ActualKey.KeyExpirationDate)
    		{
    			return false;
    		}
    		return true;
    	}

    	/// <summary>
    	/// Checks license key at first activation
    	/// </summary>
    	/// <param name="LicenseKey"></param>
    	/// <returns></returns>
    	private static string CheckLicenseKeyOnRemoteServer(string LicenseKey, bool ForceKeyValidationOnRemoteServer = false)
    	{
    		RegistryKey registryKey = RegistryOperations.GetApplicationRegistryKeyNode();
    		if (!ForceKeyValidationOnRemoteServer && registryKey != null)
    		{
    			try
    			{
    				object value = registryKey.GetValue("kcos", null);
    				if (value != null)
    				{
    					string text = value.ToString();
    					if (text == "LICENSE_OK")
    					{
    						RegistryOperations.CloseRegistryKey(registryKey);
    						return text;
    					}
    				}
    			}
    			catch (Exception)
    			{
    				return "ERROR_REGISTRYACCESS";
    			}
    		}
    		if (registryKey == null)
    		{
    			registryKey = RegistryOperations.CreateApplicationRegistryKey();
    		}
    		string text2 = TryToActivateLicenseKeyOnRemoteServer(LicenseKey);
    		try
    		{
    			registryKey.SetValue("kcos", text2);
    			RegistryOperations.CloseRegistryKey(registryKey);
    		}
    		catch (Exception)
    		{
    			return "ERROR_REGISTRYACCESS";
    		}
    		return text2;
    	}

    	/// <summary>
    	/// Checks license key validity on remote license server
    	/// </summary>
    	/// <param name="LicenseKey"></param>
    	/// <returns></returns>
    	internal static string TryToActivateLicenseKeyOnRemoteServer(string LicenseKey)
    	{
    		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
    		//IL_000e: Expected O, but got Unknown
    		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
    		//IL_0015: Expected O, but got Unknown
    		string text = "https://data-excavator.com/de-licensing/api.php";
    		RestClient val = new RestClient(text);
    		RestRequest val2 = new RestRequest((Method)1);
    		val2.AddParameter("action", (object)"verify-license-key");
    		val2.AddParameter("key", (object)LicenseKey);
    		try
    		{
    			IRestResponse val3 = val.Execute((IRestRequest)(object)val2);
    			if (val3.StatusCode != HttpStatusCode.OK)
    			{
    				return "ERROR_CANTACTIVATEKEYREMOTE";
    			}
    			return val3.Content switch
    			{
    				"LICENSE_OK" => "LICENSE_OK", 
    				"ERROR_LICENSE_TOOMUCHACTIVATIONS" => "ERROR_LICENSE_TOOMUCHACTIVATIONS", 
    				"ERROR_WRONG_KEY_DATA" => "ERROR_WRONG_KEY_DATA", 
    				"ERROR_LICENSE_EXPIRED" => "ERROR_LICENSE_EXPIRED", 
    				"ERROR_LICENSE_IS_DEACTIVATED" => "ERROR_LICENSE_IS_DEACTIVATED", 
    				_ => "ERROR_CANTACTIVATEKEYREMOTE", 
    			};
    		}
    		catch (Exception)
    		{
    			return "ERROR_CANTACTIVATEKEYREMOTE";
    		}
    	}

    	/// <summary>
    	/// Check can activate demo key on this machine
    	/// </summary>
    	/// <returns></returns>
    	private static bool CanActivateDemoKey()
    	{
    		bool flag = false;
    		RegistryKey applicationRegistryKeyNode = RegistryOperations.GetApplicationRegistryKeyNode();
    		if (applicationRegistryKeyNode != null)
    		{
    			try
    			{
    				object value = applicationRegistryKeyNode.GetValue("dka", null);
    				if (value == null)
    				{
    					flag = true;
    				}
    				else
    				{
    					flag = false;
    				}
    				RegistryOperations.CloseRegistryKey(applicationRegistryKeyNode);
    			}
    			catch (Exception)
    			{
    				flag = true;
    			}
    		}
    		else
    		{
    			flag = true;
    		}
    		return true;
    	}

    	/// <summary>
    	/// Sets that demo-key was activated
    	/// </summary>
    	/// <returns></returns>
    	private static void SetDemoKeyWasActivated()
    	{
    		try
    		{
    			RegistryKey registryKey = RegistryOperations.CreateApplicationRegistryKey();
    			if (registryKey != null)
    			{
    				registryKey.SetValue("dka", 1, RegistryValueKind.DWord);
    				RegistryOperations.CloseRegistryKey(registryKey);
    			}
    		}
    		catch (Exception)
    		{
    		}
    	}

    	/// <summary>
    	/// Packs license key into string
    	/// </summary>
    	/// <param name="key"></param>
    	/// <returns></returns>
    	[Obsolete("REMOVE IN RELEASE")]
    	public static string PackLicenseObject(LicenseKey key)
    	{
    		string plainText = JsonConvert.SerializeObject((object)key);
    		LicensingCryptography licensingCryptography = new LicensingCryptography();
    		return licensingCryptography.EncryptString(plainText);
    	}
    }
}
