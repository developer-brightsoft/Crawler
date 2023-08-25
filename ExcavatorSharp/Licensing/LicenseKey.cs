// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Licensing.LicenseKey
using System;
using System.Globalization;
using Newtonsoft.Json;

namespace ExcavatorSharp.Licensing
{
    /// <summary>
    /// License key
    /// </summary>
    public class LicenseKey : ICloneable
    {
    	/// <summary>
    	/// Original license key data
    	/// </summary>
    	[JsonIgnore]
    	public string LicenseKeyData { get; set; }

    	/// <summary>
    	/// Applied product identifyer
    	/// </summary>
    	[JsonProperty(PropertyName = "PID")]
    	public string ProductCode { get; set; }

    	/// <summary>
    	/// Key comment
    	/// </summary>
    	[JsonProperty(PropertyName = "KID")]
    	public int KeyID { get; set; }

    	/// <summary>
    	/// Key generation date - objective accessor
    	/// </summary>
    	[JsonIgnore]
    	public DateTime KeyGenerationDate
    	{
    		get
    		{
    			try
    			{
    				return DateTime.ParseExact(KeyGenerationDateOrig, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    			}
    			catch (Exception)
    			{
    				return new DateTime(1500, 15, 15);
    			}
    		}
    	}

    	/// <summary>
    	/// Key expiration date - objective accessor
    	/// </summary>
    	[JsonIgnore]
    	public DateTime KeyExpirationDate
    	{
    		get
    		{
    			try
    			{
    				return DateTime.ParseExact(KeyExpirationDateOrig, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    			}
    			catch (Exception)
    			{
    				return new DateTime(1500, 15, 15);
    			}
    		}
    	}

    	/// <summary>
    	/// Key generation date - original data
    	/// </summary>
    	[JsonProperty(PropertyName = "KGD")]
    	public string KeyGenerationDateOrig { get; set; }

    	/// <summary>
    	/// Key expiration date - original data
    	/// </summary>
    	[JsonProperty(PropertyName = "KED")]
    	public string KeyExpirationDateOrig { get; set; }

    	/// <summary>
    	/// Key type = { 1 =&gt; Demo, 2 =&gt; STD, 3 =&gt; Enterprise, 4 =&gt; StartUp }
    	/// </summary>
    	[JsonProperty(PropertyName = "KAT")]
    	public int KeyType { get; set; }

    	/// <summary>
    	/// Key projects limit per application
    	/// </summary>
    	[JsonProperty(PropertyName = "KPL")]
    	public int KeyProjectsLimit { get; set; }

    	/// <summary>
    	/// Key threads limit per each project
    	/// </summary>
    	[JsonProperty(PropertyName = "KTL")]
    	public int KeyTotalThreadsLimitPerProject { get; set; }

    	/// <summary>
    	/// Key comment
    	/// </summary>
    	[JsonProperty(PropertyName = "KOE")]
    	public string LicenseOwnerEmail { get; set; }

    	/// <summary>
    	/// Clones object
    	/// </summary>
    	/// <returns></returns>
    	public object Clone()
    	{
    		return new LicenseKey
    		{
    			KeyExpirationDateOrig = KeyExpirationDateOrig,
    			KeyGenerationDateOrig = KeyGenerationDateOrig,
    			KeyID = KeyID,
    			KeyProjectsLimit = KeyProjectsLimit,
    			KeyTotalThreadsLimitPerProject = KeyTotalThreadsLimitPerProject,
    			KeyType = KeyType,
    			LicenseOwnerEmail = LicenseOwnerEmail,
    			LicenseKeyData = LicenseKeyData,
    			ProductCode = ProductCode
    		};
    	}

    	/// <summary>
    	/// Gets key type name by key type id
    	/// </summary>
    	/// <returns></returns>
    	public string GetKeyTypeName()
    	{
    		return KeyType switch
    		{
    			1 => "DEMO", 
    			2 => "STANDARD", 
    			3 => "ENTERPRISE", 
    			4 => "STARTUP", 
    			5 => "PROMO", 
    			99 => "CODESTER", 
    			209 => "KERNEL_FREE_KEY", 
    			_ => "NONE", 
    		};
    	}

    	/// <summary>
    	/// Returns that product code is valid
    	/// </summary>
    	/// <returns></returns>
    	public bool IsProductCodeValid()
    	{
    		return ProductCode == "DEAPP2019";
    	}

    	/// <summary>
    	/// Return information that license key valid and active;
    	/// </summary>
    	/// <returns></returns>
    	public bool IsKeyDateValidAndNonOutdated()
    	{
    		return KeyExpirationDate > DateTime.Now;
    	}
    }
}
