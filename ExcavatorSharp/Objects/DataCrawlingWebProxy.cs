// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DataCrawlingWebProxy
using System.Net;
using Newtonsoft.Json;

/// <summary>
/// Storage for IWebProxy, adapted to serializing
/// </summary>
namespace ExcavatorSharp.Objects
{
	public class DataCrawlingWebProxy
	{
		/// <summary>
		/// Proxy full host addres or IP
		/// </summary>
		public string ProxyAddress { get; set; }

		/// <summary>
		/// Proxy port
		/// </summary>
		public int ProxyPort { get; set; }

		/// <summary>
		/// Should proxy credentials be used
		/// </summary>
		public bool UseCredentials { get; set; }

		/// <summary>
		/// Proxy credentials user name. Ignored if UseCredentials = false.
		/// </summary>
		public string CredentialsUserName { get; set; }

		/// <summary>
		/// Proxy credentials password. Ignored if UseCredentials = false.
		/// </summary>
		public string CredentialsPassword { get; set; }

		/// <summary>
		/// Cached proxy object for GetProxyFromDataSet method
		/// </summary>
		[JsonIgnore]
		public WebProxy ProxyServerLink { get; set; }

		/// <summary>
		/// Creates a new instance of EmptyProxy. If you creates new empty proxy, dont forget to use InitializeProxy method!
		/// </summary>
		public DataCrawlingWebProxy()
		{
		}

		/// <summary>
		/// Creates a new instance of DataCrawlingWebProxy
		/// </summary>
		/// <param name="ProxyAddress">Proxy full host addres or IP</param>
		/// <param name="ProxyPort">Proxy port</param>
		/// <param name="UseCredentials">Should proxy credentials be used</param>
		/// <param name="CredentialsUserName">Proxy credentials user name. Ignored if UseCredentials = false.</param>
		/// <param name="CredentialsPassword">Proxy credentials password. Ignored if UseCredentials = false.</param>
		public DataCrawlingWebProxy(string ProxyAddress, int ProxyPort, bool UseCredentials = false, string CredentialsUserName = "", string CredentialsPassword = "")
		{
			this.ProxyAddress = ProxyAddress;
			this.ProxyPort = ProxyPort;
			this.UseCredentials = UseCredentials;
			this.CredentialsUserName = CredentialsUserName;
			this.CredentialsPassword = CredentialsPassword;
			InitializeProxy();
		}

		/// <summary>
		/// Initializes proxy from actual settings
		/// </summary>
		public void InitializeProxy()
		{
			WebProxy webProxy = null;
			if (!UseCredentials)
			{
				webProxy = new WebProxy(ProxyAddress, ProxyPort);
			}
			else
			{
				webProxy = new WebProxy(ProxyAddress, ProxyPort);
				webProxy.Credentials = new NetworkCredential(CredentialsUserName, CredentialsPassword);
			}
			ProxyServerLink = webProxy;
		}
	}
}