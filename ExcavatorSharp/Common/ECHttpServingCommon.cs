// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Common.ECHttpServingCommon
using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace ExcavatorSharp.Common
{
    /// <summary>
    /// Some basic things for serving HTTP connections
    /// </summary>
    public class ECHttpServingCommon
    {
    	/// <summary>
    	/// Configures service point manager for the all Crawling tasks into system.
    	/// </summary>
    	internal static void ConfigureServicePointManager()
    	{
    		ServicePointManager.DefaultConnectionLimit = DEConfig.HttpConnectionsMaxCount;
    		ServicePointManager.Expect100Continue = DEConfig.HttpWebRequest_Expect100Continue;
    		ServicePointManager.CheckCertificateRevocationList = DEConfig.HttpWebRequest_CheckCertificateRevocationList;
    		bool flag = false;
    		try
    		{
    			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
    			flag = true;
    		}
    		catch (Exception)
    		{
    		}
    		if (!flag)
    		{
    			try
    			{
    				ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
    				flag = true;
    			}
    			catch (Exception)
    			{
    			}
    		}
    		if (DEConfig.TrustedHostsGlobal == null)
    		{
    			return;
    		}
    		ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
    		{
    			if (errors == SslPolicyErrors.None)
    			{
    				return true;
    			}
    			return sender is HttpWebRequest httpWebRequest && DEConfig.TrustedHostsGlobal.Contains(httpWebRequest.RequestUri.Host);
    		};
    	}
    }
}
