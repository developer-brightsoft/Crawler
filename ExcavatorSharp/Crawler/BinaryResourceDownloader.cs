// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Crawler.BinaryResourceDownloader
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using ExcavatorSharp.Objects;

namespace ExcavatorSharp.Crawler
{
    /// <summary>
    /// Binary resources downloader
    /// </summary>
    internal class BinaryResourceDownloader
    {
    	/// <summary>
    	/// Protocol types
    	/// </summary>
    	private enum RemoteResourceProtocolType
    	{
    		ftpBased,
    		httpBased
    	}

    	/// <summary>
    	/// Forced proxy server for resource downloading
    	/// </summary>
    	private WebProxy ForcedProxyServer = null;

    	/// <summary>
    	/// Link to a crawling bot
    	/// </summary>
    	private CrawlingServer CrawlingServerEnvironmentLink { get; set; }

    	/// <summary>
    	/// Object for accessing crawling server proxies list
    	/// </summary>
    	private ProxyAccessor ProxyServersAccessor { get; set; }

    	/// <summary>
    	/// Creates a new instance of BinaryResourceDownloader
    	/// </summary>
    	/// <param name="CrawlingServerEnvironmentLink">Link to crawling server object</param>
    	/// <param name="ForcedProxyServer">Enforced proxy server, priored to PeekProxy inner function</param>
    	public BinaryResourceDownloader(CrawlingServer CrawlingServerEnvironmentLink, WebProxy ForcedProxyServer = null)
    	{
    		this.CrawlingServerEnvironmentLink = CrawlingServerEnvironmentLink;
    		ProxyServersAccessor = new ProxyAccessor(CrawlingServerEnvironmentLink.CrawlerProperties, CrawlingServerEnvironmentLink.ProxySequenciveRotationMutex);
    		this.ForcedProxyServer = ForcedProxyServer;
    	}

    	/// <summary>
    	/// Downloads binary resource by link
    	/// </summary>
    	/// <param name="ResourceLink"></param>
    	/// <returns></returns>
    	public BinaryResourceDownloadingResult DownloadResource(PageLink ResourceLink)
    	{
    		DateTime now = DateTime.Now;
    		if (ResourceLink == null)
    		{
    			CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CommonEntity, $"Error: cannot download binary resource - ResourceLink is null");
    			return new BinaryResourceDownloadingResult(new byte[0], HttpStatusCode.BadRequest, 0, new TimeSpan(1L), new Exception("Wrong resource link, link is null"));
    		}
    		RemoteResourceProtocolType remoteResourceProtocolType = RemoteResourceProtocolType.ftpBased;
    		WebRequest webRequest = null;
    		try
    		{
    			if (ResourceLink.NormalizedOriginalLink.StartsWith("http"))
    			{
    				webRequest = (HttpWebRequest)WebRequest.Create(ResourceLink.NormalizedOriginalLink);
    				(webRequest as HttpWebRequest).UserAgent = CrawlingServerEnvironmentLink.CrawlerProperties.CrawlerUserAgent;
    				(webRequest as HttpWebRequest).AllowAutoRedirect = true;
    				(webRequest as HttpWebRequest).MaximumAutomaticRedirections = 3;
    				webRequest.Method = "GET";
    				webRequest.AuthenticationLevel = AuthenticationLevel.None;
    				webRequest.Timeout = CrawlingServerEnvironmentLink.CrawlerProperties.PageDownloadTimeoutMilliseconds;
    				remoteResourceProtocolType = RemoteResourceProtocolType.httpBased;
    			}
    			else if (ResourceLink.NormalizedOriginalLink.StartsWith("ftp"))
    			{
    				webRequest = (FtpWebRequest)WebRequest.Create(ResourceLink.NormalizedOriginalLink);
    				webRequest.Timeout = CrawlingServerEnvironmentLink.CrawlerProperties.PageDownloadTimeoutMilliseconds;
    				remoteResourceProtocolType = RemoteResourceProtocolType.ftpBased;
    			}
    			else
    			{
    				webRequest = (HttpWebRequest)WebRequest.Create(ResourceLink.NormalizedOriginalLink);
    				(webRequest as HttpWebRequest).UserAgent = CrawlingServerEnvironmentLink.CrawlerProperties.CrawlerUserAgent;
    				webRequest.Method = "GET";
    				webRequest.AuthenticationLevel = AuthenticationLevel.None;
    				webRequest.Timeout = CrawlingServerEnvironmentLink.CrawlerProperties.PageDownloadTimeoutMilliseconds;
    				remoteResourceProtocolType = RemoteResourceProtocolType.httpBased;
    			}
    		}
    		catch (Exception ex)
    		{
    			CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CommonEntity, $"It is impossible to create an HttpWebRequest object. PageLink data = {ResourceLink.SerializeToJSON()}", ex);
    			return new BinaryResourceDownloadingResult(new byte[0], HttpStatusCode.BadRequest, 0, new TimeSpan(1L), ex);
    		}
    		if (webRequest == null)
    		{
    			CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CommonEntity, $"Error: cannot download binary resource - HttpWebRequest is null, PageLink data = {ResourceLink.SerializeToJSON()}");
    			return new BinaryResourceDownloadingResult(new byte[0], HttpStatusCode.BadRequest, 0, new TimeSpan(1L), new Exception("HttpWebRequest object is null"));
    		}
    		if (ForcedProxyServer != null)
    		{
    			webRequest.Proxy = ForcedProxyServer;
    		}
    		else
    		{
    			WebProxy webProxy = ProxyServersAccessor.PeekProxy();
    			if (webProxy != null)
    			{
    				webRequest.Proxy = webProxy;
    			}
    		}
    		byte[] resourceData = null;
    		Exception resourceDownloadException = null;
    		HttpStatusCode httpStatusCode = (HttpStatusCode)0;
    		int num = 0;
    		bool flag = false;
    		while (!flag && num < CrawlingServerEnvironmentLink.CrawlerProperties.PageDownloadAttempts)
    		{
    			try
    			{
    				WebResponse response = webRequest.GetResponse();
    				Stream responseStream = response.GetResponseStream();
    				BinaryReader binaryReader = new BinaryReader(responseStream);
    				switch (remoteResourceProtocolType)
    				{
    				case RemoteResourceProtocolType.httpBased:
    					httpStatusCode = ((HttpWebResponse)response).StatusCode;
    					break;
    				case RemoteResourceProtocolType.ftpBased:
    				{
    					FtpStatusCode statusCode = ((FtpWebResponse)response).StatusCode;
    					if (statusCode == FtpStatusCode.CommandOK || statusCode == FtpStatusCode.FileActionOK || statusCode == FtpStatusCode.OpeningData || statusCode == FtpStatusCode.DataAlreadyOpen)
    					{
    						httpStatusCode = HttpStatusCode.OK;
    						break;
    					}
    					httpStatusCode = HttpStatusCode.InternalServerError;
    					throw new Exception($"Ftp access exception; ftp response status code = {statusCode.ToString()}");
    				}
    				}
    				if (httpStatusCode == HttpStatusCode.OK)
    				{
    					flag = true;
    					List<byte[]> list = new List<byte[]>();
    					int num2 = 0;
    					do
    					{
    						byte[] array = new byte[1024];
    						num2 = responseStream.Read(array, 0, array.Length);
    						if (num2 > 0)
    						{
    							if (num2 == 1024)
    							{
    								list.Add(array);
    								continue;
    							}
    							byte[] array2 = new byte[num2];
    							Array.Copy(array, array2, num2);
    							list.Add(array2);
    						}
    					}
    					while (num2 > 0);
    					resourceData = list.SelectMany((byte[] chunk) => chunk).ToArray();
    				}
    				binaryReader.Close();
    				binaryReader.Dispose();
    				responseStream.Close();
    				responseStream.Dispose();
    				response.Close();
    				response.Dispose();
    				binaryReader = null;
    				responseStream = null;
    				response = null;
    			}
    			catch (Exception ex2)
    			{
    				resourceDownloadException = ex2;
    			}
    			finally
    			{
    				num++;
    			}
    		}
    		webRequest = null;
    		DateTime now2 = DateTime.Now;
    		TimeSpan resourceDownloadTime = now2 - now;
    		return new BinaryResourceDownloadingResult(resourceData, httpStatusCode, num, resourceDownloadTime, resourceDownloadException);
    	}
    }
}
