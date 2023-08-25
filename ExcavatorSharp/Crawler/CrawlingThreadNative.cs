// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Crawler.CrawlingThreadNative
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading;
using ExcavatorSharp.Common;
using ExcavatorSharp.ExtensionMethods;
using ExcavatorSharp.Objects;

namespace ExcavatorSharp.Crawler
{
    /// <summary>
    /// Crawling thread for native indexing. Using .NET WebClient for pages crawling.
    /// </summary>
    internal class CrawlingThreadNative : CrawlingThreadBase
    {
    	/// <summary>
    	/// Main event, that will happen after next page been crawled.
    	/// </summary>
    	public override event PageCrawledHandler PageCrawled;

    	/// <summary>
    	/// Creates a new instance of CrawlingThread
    	/// </summary>
    	/// <param name="CrawlingServerEnvironmentLink">Link to parent server</param>
    	public CrawlingThreadNative(CrawlingServer CrawlingServerEnvironmentLink, int ThreadInitialSleepingTime)
    		: base(CrawlingServerEnvironmentLink, ThreadInitialSleepingTime)
    	{
    	}

    	/// <summary>
    	/// Main crawling thread body
    	/// </summary>
    	protected override void ThreadBody()
    	{
    		Thread.Sleep(base.ThreadInitialSleepingTime);
    		while (!DEConfig.ShutDownProgram)
    		{
    			base.CrawlingServerEnvironmentLink.CrawlDelayWait();
    			PageLink result = null;
    			while (!base.CrawlingServerEnvironmentLink.LinksBuffer.LinksToCrawl.TryDequeue(out result))
    			{
    				Thread.Sleep(10);
    			}
    			string text = result.NormalizedOriginalLink;
    			base.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Trying to crawl URL = {text}");
    			DateTime now = DateTime.Now;
    			if (base.CrawlingServerEnvironmentLink.CrawlerProperties.HttpWebRequestMethod == "GET" && base.CrawlingServerEnvironmentLink.CrawlerProperties.CrawlingRequestAdditionalParamsList != null && base.CrawlingServerEnvironmentLink.CrawlerProperties.CrawlingRequestAdditionalParamsList.Count > 0)
    			{
    				text = DEExtensions.AddGETArgsToLink(text, base.CrawlingServerEnvironmentLink.CrawlerProperties.CrawlingRequestAdditionalParamsList);
    			}
    			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(text);
    			WebProxy webProxy = base.ProxyServersAccessor.PeekProxy();
    			if (webProxy != null)
    			{
    				httpWebRequest.Proxy = webProxy;
    			}
    			httpWebRequest.Method = base.CrawlingServerEnvironmentLink.CrawlerProperties.HttpWebRequestMethod;
    			httpWebRequest.Timeout = base.CrawlingServerEnvironmentLink.CrawlerProperties.PageDownloadTimeoutMilliseconds;
    			httpWebRequest.AuthenticationLevel = AuthenticationLevel.None;
    			httpWebRequest.UserAgent = base.CrawlingServerEnvironmentLink.CrawlerProperties.CrawlerUserAgent;
    			httpWebRequest.Referer = base.CrawlingServerEnvironmentLink.WebsiteBaseUrl.ToString();
    			httpWebRequest.AllowAutoRedirect = true;
    			httpWebRequest.MaximumAutomaticRedirections = 10;
    			if (httpWebRequest.ServerCertificateValidationCallback != ServicePointManager.ServerCertificateValidationCallback)
    			{
    				httpWebRequest.ServerCertificateValidationCallback = ServicePointManager.ServerCertificateValidationCallback;
    			}
    			httpWebRequest.CookieContainer = base.CrawlingServerEnvironmentLink.ChildRequestsCookieContainer;
    			if (base.CrawlingServerEnvironmentLink.CrawlerProperties.HttpWebRequestMethod == "POST" && base.CrawlingServerEnvironmentLink.CrawlerProperties.CrawlingRequestAdditionalParamsList != null && base.CrawlingServerEnvironmentLink.CrawlerProperties.CrawlingRequestAdditionalParamsList.Count > 0)
    			{
    				StringBuilder stringBuilder = new StringBuilder();
    				int num = 0;
    				foreach (KeyValuePair<string, string> crawlingRequestAdditionalParams in base.CrawlingServerEnvironmentLink.CrawlerProperties.CrawlingRequestAdditionalParamsList)
    				{
    					stringBuilder.Append(crawlingRequestAdditionalParams.Key).Append('=').Append(crawlingRequestAdditionalParams.Value);
    					if (num + 1 < base.CrawlingServerEnvironmentLink.CrawlerProperties.CrawlingRequestAdditionalParamsList.Count)
    					{
    						stringBuilder.Append('&');
    					}
    					num++;
    				}
    				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
    				httpWebRequest.ContentType = "application/x-www-form-urlencoded";
    				httpWebRequest.ContentLength = bytes.Length;
    				using Stream stream = httpWebRequest.GetRequestStream();
    				stream.Write(bytes, 0, bytes.Length);
    			}
    			string text2 = string.Empty;
    			Exception crawledPageThrownException = null;
    			HttpStatusCode httpStatusCode = (HttpStatusCode)0;
    			int num2 = 0;
    			bool flag = false;
    			while (!flag && num2 < base.CrawlingServerEnvironmentLink.CrawlerProperties.PageDownloadAttempts)
    			{
    				try
    				{
    					HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
    					Stream stream2 = httpWebResponse.GetResponseStream();
    					if (httpWebResponse.ContentEncoding.ToLower().Contains("gzip"))
    					{
    						stream2 = new GZipStream(stream2, CompressionMode.Decompress);
    					}
    					else if (httpWebResponse.ContentEncoding.ToLower().Contains("deflate"))
    					{
    						stream2 = new DeflateStream(stream2, CompressionMode.Decompress);
    					}
    					StreamReader streamReader = new StreamReader(stream2);
    					text2 = streamReader.ReadToEnd();
    					httpStatusCode = httpWebResponse.StatusCode;
    					if (httpStatusCode == HttpStatusCode.OK)
    					{
    						flag = true;
    					}
    					streamReader.Close();
    					streamReader.Dispose();
    					stream2.Close();
    					stream2.Dispose();
    					httpWebResponse.Close();
    					httpWebResponse.Dispose();
    					streamReader = null;
    					stream2 = null;
    					httpWebResponse = null;
    				}
    				catch (Exception ex)
    				{
    					crawledPageThrownException = ex;
    				}
    				finally
    				{
    					num2++;
    				}
    			}
    			httpWebRequest = null;
    			DateTime now2 = DateTime.Now;
    			TimeSpan pageCrawlingTime = now2 - now;
    			List<PageLink> pageLinks = new List<PageLink>();
    			if (base.CrawlingServerEnvironmentLink.CrawlerProperties.CrawlWebsiteLinks)
    			{
    				pageLinks = ParsePageLinks(text2);
    			}
    			FirePageCrawledEvent(new PageCrawlingResultMetaInformation(now2, pageCrawlingTime, httpStatusCode, num2, crawledPageThrownException), result, text2, pageLinks);
    			Thread.Sleep(base.CrawlingServerEnvironmentLink.CrawlerProperties.CrawlingThreadDelayMilliseconds);
    		}
    	}

    	/// <summary>
    	/// Fires page crawled event
    	/// </summary>
    	/// <param name="DownloadedPageUrl"></param>
    	/// <param name="PageHtml"></param>
    	/// <param name="PageLinks"></param>
    	private void FirePageCrawledEvent(PageCrawlingResultMetaInformation CrawledPageMeta, PageLink DownloadedPageUrl, string PageHtml, List<PageLink> PageLinks)
    	{
    		PageCrawled?.Invoke(new PageCrawledCallback(CrawledPageMeta, DownloadedPageUrl, PageHtml, PageLinks, null, null));
    	}
    }
}
