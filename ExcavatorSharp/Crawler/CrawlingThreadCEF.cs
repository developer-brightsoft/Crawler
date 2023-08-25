// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Crawler.CrawlingThreadCEF
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using ExcavatorSharp.Captcha;
using ExcavatorSharp.CEF;
using ExcavatorSharp.Common;
using ExcavatorSharp.ExtensionMethods;
using ExcavatorSharp.Objects;

namespace ExcavatorSharp.Crawler
{
    /// <summary>
    /// Crawling thread for CEF-used indexing. Using Chromium Embedded Framework (Chromium engine) for crawling.
    /// </summary>
    internal class CrawlingThreadCEF : CrawlingThreadBase
    {
    	/// <summary>
    	/// Is it necessary to take screenshot after page loaded
    	/// </summary>
    	private bool TakeScreenshotAfterPageLoaded = false;

    	/// <summary>
    	/// CEFSharp RequextContext, used for browser ctor
    	/// </summary>
    	private RequestContext CEFRequestContext { get; set; }

    	/// <summary>
    	/// Chromium browser itself
    	/// </summary>
    	private ChromiumWebBrowser CEFBrowser { get; set; }

    	/// <summary>
    	/// Main event, that will happen after next page been crawled.
    	/// </summary>
    	public override event PageCrawledHandler PageCrawled;

    	/// <summary>
    	/// Creates a new instance of CrawlingThread
    	/// </summary>
    	/// <param name="CrawlingServerEnvironmentLink">Link to parent server</param>
    	public CrawlingThreadCEF(CrawlingServer CrawlingServerEnvironmentLink, int ThreadInitialSleepingTime, bool TakeScreenshotAfterPageLoaded)
    		: base(CrawlingServerEnvironmentLink, ThreadInitialSleepingTime)
    	{
    		this.TakeScreenshotAfterPageLoaded = true;
    	}

    	/// <summary>
    	/// Main crawling thread body
    	/// </summary>
    	protected override void ThreadBody()
    	{
    		Thread.Sleep(base.ThreadInitialSleepingTime);
    		while (!CEFBrowser.IsBrowserInitialized && DEConfig.ShutDownProgram)
    		{
    			Thread.Sleep(50);
    		}
    		while (!DEConfig.ShutDownProgram)
    		{
    			base.CrawlingServerEnvironmentLink.CrawlDelayWait();
    			PageLink result = null;
    			while (!base.CrawlingServerEnvironmentLink.LinksBuffer.LinksToCrawl.TryDequeue(out result))
    			{
    				Thread.Sleep(10);
    			}
    			string NextUrlToCrawl = result.NormalizedOriginalLink;
    			base.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Trying to crawl URL = {NextUrlToCrawl}");
    			if (base.CrawlingServerEnvironmentLink.CrawlerProperties.HttpWebRequestMethod == "GET" && base.CrawlingServerEnvironmentLink.CrawlerProperties.CrawlingRequestAdditionalParamsList != null && base.CrawlingServerEnvironmentLink.CrawlerProperties.CrawlingRequestAdditionalParamsList.Count > 0)
    			{
    				NextUrlToCrawl = DEExtensions.AddGETArgsToLink(NextUrlToCrawl, base.CrawlingServerEnvironmentLink.CrawlerProperties.CrawlingRequestAdditionalParamsList);
    			}
    			DateTime now = DateTime.Now;
    			string text = string.Empty;
    			Exception crawledPageThrownException = null;
    			HttpStatusCode httpStatusCode = (HttpStatusCode)0;
    			int num = 0;
    			bool flag = false;
    			DateTime dateTime = default(DateTime);
    			TimeSpan timeSpan = default(TimeSpan);
    			List<PageLink> list = null;
    			Bitmap pageScreenshot = null;
    			Dictionary<string, string> pageFrames = null;
    			WebProxy webProxy = null;
    			CaptchaSolverBase captchaSolverBase = null;
    			int num2 = 0;
    			int num3 = 0;
    			List<PageLink> list2 = null;
    			int num4 = 0;
    			bool flag2 = false;
    			bool flag3 = false;
    			bool flag4 = false;
    			while (!flag && (num < base.CrawlingServerEnvironmentLink.CrawlerProperties.PageDownloadAttempts || flag2))
    			{
    				try
    				{
    					if (base.CrawlingServerEnvironmentLink.CrawlerProperties.HTTPWebRequestProxiesList != null && base.CrawlingServerEnvironmentLink.CrawlerProperties.HTTPWebRequestProxiesList.Count > 0)
    					{
    						webProxy = base.ProxyServersAccessor.PeekProxy();
    						string login = ((webProxy.Credentials != null) ? (webProxy.Credentials as NetworkCredential).UserName : string.Empty);
    						string password = ((webProxy.Credentials != null) ? (webProxy.Credentials as NetworkCredential).Password : string.Empty);
    						CEFSetProxy(webProxy.Address.AbsoluteUri, webProxy.Address.Port, login, password);
    					}
    					CEFCrawlingBehavior currentPageCrawlingBehavior = GetCurrentPageCrawlingBehavior(NextUrlToCrawl);
    					if (currentPageCrawlingBehavior != null && currentPageCrawlingBehavior.LeavePageRule != 0)
    					{
    						flag3 = true;
    					}
    					if (!flag2)
    					{
    						if (base.CrawlingServerEnvironmentLink.CrawlerProperties.HttpWebRequestMethod == "GET")
    						{
    							AsyncHelpers.RunSync(() => LoadPageWithGETAsync(NextUrlToCrawl));
    						}
    						else if (base.CrawlingServerEnvironmentLink.CrawlerProperties.HttpWebRequestMethod == "POST")
    						{
    							LoadPageWithPOSTSync(NextUrlToCrawl, base.CrawlingServerEnvironmentLink.CrawlerProperties.CrawlingRequestAdditionalParamsList);
    						}
    					}
    					if (captchaSolverBase != null)
    					{
    						text = GetPageActualSourceCode();
    						if (captchaSolverBase.IsCaptchaShown(NextUrlToCrawl, text))
    						{
    							CaptchaResolveResults captchaResolveResults = captchaSolverBase.TryToSolveCaptcha(NextUrlToCrawl, text, webProxy);
    							if (captchaResolveResults == CaptchaResolveResults.CaptchaNotResolved)
    							{
    								base.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Captcha doesn't resolved; Trying to crawl page as-is.");
    							}
    						}
    					}
    					if (currentPageCrawlingBehavior != null)
    					{
    						if (currentPageCrawlingBehavior.WaitAfterPageLoaded_InSeconds_Step1 != 0)
    						{
    							Thread.Sleep(currentPageCrawlingBehavior.WaitAfterPageLoaded_InSeconds_Step1 * 1000);
    						}
    						if (currentPageCrawlingBehavior.JSScriptToExecute_Step2 != null && currentPageCrawlingBehavior.JSScriptToExecute_Step2.Length > 0)
    						{
    							EvaluateScriptAndGetResult(currentPageCrawlingBehavior.JSScriptToExecute_Step2);
    						}
    						if (currentPageCrawlingBehavior.WaitAfterpageLoaded_InSeconds_Step3 != 0)
    						{
    							Thread.Sleep(currentPageCrawlingBehavior.WaitAfterpageLoaded_InSeconds_Step3 * 1000);
    						}
    					}
    					text = GetPageActualSourceCode();
    					if (!(text != string.Empty) || base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior == null || flag4)
    					{
    						goto IL_0843;
    					}
    					bool flag5 = false;
    					if (base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.PagesURLSubstringsToCheckLogin != null)
    					{
    						string[] pagesURLSubstringsToCheckLogin = base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.PagesURLSubstringsToCheckLogin;
    						foreach (string text2 in pagesURLSubstringsToCheckLogin)
    						{
    							if (NextUrlToCrawl.IndexOf(text2) != -1 || text2 == "*")
    							{
    								flag5 = true;
    								break;
    							}
    						}
    					}
    					if (base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.PagesURLSubstringsToCheckLogin == null)
    					{
    						flag5 = true;
    					}
    					bool flag6 = false;
    					if (base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.PagesURLSubstringsCheckRestrictions != null)
    					{
    						string[] pagesURLSubstringsCheckRestrictions = base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.PagesURLSubstringsCheckRestrictions;
    						foreach (string text3 in pagesURLSubstringsCheckRestrictions)
    						{
    							if (NextUrlToCrawl.IndexOf(text3) != -1 || text3 == "*")
    							{
    								flag6 = true;
    								break;
    							}
    						}
    					}
    					if (!flag5 || flag6)
    					{
    						goto IL_0843;
    					}
    					if (captchaSolverBase != null && captchaSolverBase.IsCaptchaShown(NextUrlToCrawl, text))
    					{
    						CaptchaResolveResults captchaResolveResults2 = captchaSolverBase.TryToSolveCaptcha(NextUrlToCrawl, text, webProxy);
    						if (captchaResolveResults2 == CaptchaResolveResults.CaptchaNotResolved)
    						{
    							base.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Captcha doesn't resolved; Trying to crawl page as-is.");
    						}
    					}
    					bool flag7 = text.IndexOf(base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.CheckUserLoggedInDocumentHTMLSubstring) != -1;
    					if (flag7)
    					{
    						goto IL_0843;
    					}
    					base.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "The user is not authorized. Attempting to log on...");
    					string websiteLoginPageURL = base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.WebsiteLoginPageURL;
    					if (websiteLoginPageURL != "*")
    					{
    						base.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Redirecting to login page...");
    						AsyncHelpers.RunSync(() => LoadPageWithGETAsync(base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.WebsiteLoginPageURL));
    						_ = base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.WaitInSecondsBeforeAndAfterLoginScript;
    						if (base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.WaitInSecondsBeforeAndAfterLoginScript > 0)
    						{
    							Thread.Sleep(base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.WaitInSecondsBeforeAndAfterLoginScript * 1000);
    						}
    						string pageActualSourceCode = GetPageActualSourceCode();
    						if (captchaSolverBase != null && captchaSolverBase.IsCaptchaShown(NextUrlToCrawl, pageActualSourceCode))
    						{
    							CaptchaResolveResults captchaResolveResults3 = captchaSolverBase.TryToSolveCaptcha(NextUrlToCrawl, pageActualSourceCode, webProxy);
    							if (captchaResolveResults3 == CaptchaResolveResults.CaptchaNotResolved)
    							{
    								base.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Captcha doesn't resolved; Trying to crawl page as-is.");
    							}
    						}
    						flag7 = pageActualSourceCode.IndexOf(base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.CheckUserLoggedInDocumentHTMLSubstring) != -1;
    						if (!flag7)
    						{
    							_ = base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.WaitInSecondsBeforeAndAfterLoginScript;
    							if (base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.WaitInSecondsBeforeAndAfterLoginScript > 0)
    							{
    								Thread.Sleep(base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.WaitInSecondsBeforeAndAfterLoginScript * 1000);
    							}
    						}
    					}
    					if (!flag7)
    					{
    						base.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Executing authorization script on the site...");
    						EvaluateScriptAndGetResult(base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.UserLoginJSScript);
    						_ = base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.WaitInSecondsBeforeAndAfterLoginScript;
    						if (base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.WaitInSecondsBeforeAndAfterLoginScript > 0)
    						{
    							Thread.Sleep(base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFWebsiteAuthenticationBehavior.WaitInSecondsBeforeAndAfterLoginScript * 1000);
    						}
    						base.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "The authentication script has been executed. We're trying to process the page again...");
    						num++;
    						num4++;
    					}
    					else
    					{
    						if (currentPageCrawlingBehavior == null)
    						{
    							goto IL_0843;
    						}
    						flag4 = true;
    						num++;
    						num4++;
    					}
    					goto end_IL_0174;
    					IL_0843:
    					httpStatusCode = ((!(text != string.Empty)) ? HttpStatusCode.NoContent : HttpStatusCode.OK);
    					if (TakeScreenshotAfterPageLoaded)
    					{
    						pageScreenshot = TakeScreenshot();
    					}
    					if (base.CrawlingServerEnvironmentLink.CrawlerProperties.ExpandPageFrames)
    					{
    						pageFrames = GetAllIFramesSourceCodes();
    					}
    					if (currentPageCrawlingBehavior != null && currentPageCrawlingBehavior.LeavePageRule != 0)
    					{
    						result.DEBehaviorLinkPoxtfix = string.Format("{0}={1}", "excavator-cef-jsitteration", num4);
    						result.DEBehaviorCycledIndexationProcess = true;
    						dateTime = DateTime.Now;
    						timeSpan = dateTime - now;
    						list = new List<PageLink>();
    						if (base.CrawlingServerEnvironmentLink.CrawlerProperties.CrawlWebsiteLinks)
    						{
    							list = ParsePageLinks(text);
    						}
    						if (list2 != null && list2.Count > 0)
    						{
    							List<PageLink> list3 = new List<PageLink>();
    							foreach (PageLink NextParsedLink in list)
    							{
    								if (list2.Where((PageLink link) => link.OriginalLink == NextParsedLink.OriginalLink).Count() == 0)
    								{
    									list3.Add(NextParsedLink);
    								}
    							}
    							list = list3;
    						}
    						FirePageCrawledEvent(new PageCrawlingResultMetaInformation(dateTime, timeSpan, httpStatusCode, num, crawledPageThrownException), result, text, list, pageScreenshot, pageFrames);
    						num2 += list.Count;
    						num3 = list.Count;
    						bool flag8 = false;
    						switch (currentPageCrawlingBehavior.LeavePageRule)
    						{
    						case CEFCrawlingPageLeaveEventType.LeavePageAfterSomeTimeSpentInSeconds:
    							if (timeSpan.TotalSeconds >= (double)Convert.ToInt32(currentPageCrawlingBehavior.LeavePageRuleValue))
    							{
    								flag8 = true;
    							}
    							break;
    						case CEFCrawlingPageLeaveEventType.LeavePageAfterJSEventReturnsSomeResult:
    						{
    							string text4 = EvaluateScriptAndGetResult(currentPageCrawlingBehavior.JSScriptToExecuteAfterPageHTMLCodeGrabbed);
    							if (text4 == currentPageCrawlingBehavior.LeavePageRuleValue)
    							{
    								flag8 = true;
    							}
    							break;
    						}
    						case CEFCrawlingPageLeaveEventType.LeavePageAfterNLinksParsed:
    							if (num2 >= Convert.ToInt32(currentPageCrawlingBehavior.LeavePageRuleValue))
    							{
    								flag8 = true;
    							}
    							break;
    						case CEFCrawlingPageLeaveEventType.LeavePageAfterNoNewLinksParsed:
    							if (list2 == null)
    							{
    								list2 = new List<PageLink>();
    							}
    							if (num3 > 0)
    							{
    								list2.AddRange(list);
    							}
    							else
    							{
    								flag8 = true;
    							}
    							break;
    						}
    						num4++;
    						flag2 = true;
    						if (flag8)
    						{
    							num++;
    							result.DEBehaviorCycledIndexationProcess = false;
    							flag = true;
    							break;
    						}
    					}
    					else if (httpStatusCode == HttpStatusCode.OK)
    					{
    						flag = true;
    					}
    					end_IL_0174:;
    				}
    				catch (Exception ex)
    				{
    					crawledPageThrownException = ex;
    				}
    				finally
    				{
    					num++;
    					num4++;
    				}
    			}
    			dateTime = DateTime.Now;
    			timeSpan = dateTime - now;
    			list = new List<PageLink>();
    			if (base.CrawlingServerEnvironmentLink.CrawlerProperties.CrawlWebsiteLinks)
    			{
    				list = ParsePageLinks(text);
    			}
    			if (!flag3)
    			{
    				FirePageCrawledEvent(new PageCrawlingResultMetaInformation(dateTime, timeSpan, httpStatusCode, num, crawledPageThrownException), result, text, list, pageScreenshot, pageFrames);
    			}
    			Thread.Sleep(base.CrawlingServerEnvironmentLink.CrawlerProperties.CrawlingThreadDelayMilliseconds);
    		}
    	}

    	/// <summary>
    	/// Set CEF proxy for existing browser
    	/// </summary> 
    	/// <param name="Address"></param>
    	private void CEFSetProxy(string Address, int Port, string Login, string Password)
    	{
    		AsyncHelpers.RunSync(() => Cef.UIThreadTaskFactory.StartNew(delegate
    		{
    			if (Login != string.Empty && Password != string.Empty)
    			{
    				CEFBrowser.RequestHandler = (IRequestHandler)(object)new CEFProxyAuthRequestHandler(Login, Password);
    			}
    			IRequestContext requestContext = CEFBrowser.GetBrowser().GetHost().RequestContext;
                bool flag = requestContext.SetPreference("proxy", (object)new Dictionary<string, object>(2)
                {
                    ["mode"] = "fixed_servers",
                    ["server"] = Address.Replace("http://", string.Empty).Replace("https://", string.Empty).Trim(' ', '/')
                }, out string text);
            }));
    	}

    	/// <summary>
    	/// Loads page by specified URL asyncronycally with $_GET method
    	/// </summary>
    	/// <param name="CEFBrowserLink"></param>
    	/// <param name="address"></param>
    	/// <returns></returns>
    	private Task LoadPageWithGETAsync(string address)
    	{
    		TaskCompletionSource<bool> TaskSource = new TaskCompletionSource<bool>(TaskCreationOptions.None);
    		EventHandler<LoadingStateChangedEventArgs> CEFPageLoadedHandler = null;
    		CEFPageLoadedHandler = delegate(object sender, LoadingStateChangedEventArgs args)
    		{
    			if (!args.IsLoading)
    			{
    				CEFBrowser.LoadingStateChanged -= CEFPageLoadedHandler;
    				TaskSource.TrySetResult(result: true);
    			}
    		};
    		CEFBrowser.LoadingStateChanged += CEFPageLoadedHandler;
    		CEFBrowser.Load(address);
    		return TaskSource.Task;
    	}

    	/// <summary>
    	/// Get frames source code
    	/// </summary>
    	public Dictionary<string, string> GetAllIFramesSourceCodes()
    	{
    		Dictionary<string, string> dictionary = new Dictionary<string, string>();
    		List<long> frameIdentifiers = CEFBrowser.GetBrowser().GetFrameIdentifiers();
    		foreach (long item in frameIdentifiers)
    		{
    			IFrame frame = CEFBrowser.GetBrowser().GetFrame(item);
    			if (frame.Url.Trim() != string.Empty)
    			{
    				string value = AsyncHelpers.RunSync(() => frame.GetSourceAsync());
    				if (!dictionary.ContainsKey(frame.Url))
    				{
    					dictionary.Add(frame.Url, value);
    				}
    			}
    		}
    		return dictionary;
    	}

    	/// <summary>
    	/// Loads page by specified URL syncronycally with $_POST method
    	/// </summary>
    	/// <param name="address"></param>
    	/// <param name="PostData"></param>
    	private void LoadPageWithPOSTSync(string address, Dictionary<string, string> PostData)
    	{
    		IFrame mainFrame = WebBrowserExtensions.GetMainFrame((IWebBrowser)(object)CEFBrowser);
    		IRequest val = mainFrame.CreateRequest(true);
    		val.Url = address;
    		val.Method = "POST";
    		val.InitializePostData();
    		if (PostData != null && PostData.Count > 0)
    		{
    			StringBuilder stringBuilder = new StringBuilder();
    			foreach (KeyValuePair<string, string> crawlingRequestAdditionalParams in base.CrawlingServerEnvironmentLink.CrawlerProperties.CrawlingRequestAdditionalParamsList)
    			{
    				string text = $"{crawlingRequestAdditionalParams.Key}={crawlingRequestAdditionalParams.Value}";
    				byte[] bytes = Encoding.ASCII.GetBytes(text.ToString());
    				IPostDataElement val2 = val.PostData.CreatePostDataElement();
    				val2.Bytes = bytes;
    				val.PostData.AddElement(val2);
    			}
    		}
    		NameValueCollection nameValueCollection = new NameValueCollection();
    		nameValueCollection.Add("Content-Type", "application/x-www-form-urlencoded");
    		val.Headers = nameValueCollection;
    		mainFrame.LoadRequest(val);
    	}

    	/// <summary>
    	/// Evaluates script and returns results
    	/// </summary>
    	/// <param name="JSScript">Script to evaluate</param>
    	/// <returns>Script evaluation results</returns>
    	internal string EvaluateScriptAndGetResult(string JSScript)
    	{
    		JavascriptResponse val = AsyncHelpers.RunSync(() => WebBrowserExtensions.EvaluateScriptAsync((IWebBrowser)(object)CEFBrowser, JSScript, (TimeSpan?)null));
    		if (val.Success)
    		{
    			if (val.Result != null)
    			{
    				return val.Result.ToString();
    			}
    			return string.Empty;
    		}
    		base.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"JS-script evaluation failed. Error = {val.Message}");
    		return string.Empty;
    	}

    	/// <summary>
    	/// Evaluates specified script without results
    	/// </summary>
    	/// <param name="JSScript">Script to evaluate</param> 
    	private void EvaluateScriptWithoutResults(string JSScript)
    	{
    		WebBrowserExtensions.ExecuteScriptAsync((IWebBrowser)(object)CEFBrowser, JSScript);
    	}

    	/// <summary>
    	/// Gets page actual source code
    	/// </summary>
    	/// <returns></returns>
    	internal string GetPageActualSourceCode()
    	{
    		return AsyncHelpers.RunSync(() => WebBrowserExtensions.GetSourceAsync((IWebBrowser)(object)CEFBrowser));
    	}

    	/// <summary>
    	/// Start crawling pages
    	/// </summary>
    	public override void StartCrawling()
    	{
    		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
    		//IL_0015: Expected O, but got Unknown
    		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
    		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
    		//IL_0027: Expected O, but got Unknown
    		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
    		//IL_0033: Expected O, but got Unknown
    		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
    		//IL_004c: Expected O, but got Unknown
    		if (CEFBrowser == null)
    		{
    			BrowserSettings val = new BrowserSettings();
    			RequestContextSettings val2 = new RequestContextSettings
    			{
    				CachePath = CEFSharpFactory.CEFTempDirectory
    			};
    			CEFRequestContext = new RequestContext(val2);
    			CEFBrowser = new ChromiumWebBrowser(string.Empty, val, (IRequestContext)(object)CEFRequestContext, true);
    			CEFBrowser.Size = new Size(1900, 1400);
    		}
    		base.StartCrawling();
    	}

    	/// <summary>
    	/// Stop crawling pages
    	/// </summary>
    	public override void StopCrawling()
    	{
    		base.StopCrawling();
    		try
    		{
    			CEFBrowser.Dispose();
    			CEFBrowser = null;
    			CEFRequestContext = null;
    		}
    		catch (Exception)
    		{
    		}
    	}

    	/// <summary>
    	/// Fetch actual CEF behavior from known CEF rules, it they specified
    	/// </summary>
    	/// <param name="PageUrl">Actual page url</param>
    	/// <returns>CEF behavior</returns>
    	private CEFCrawlingBehavior GetCurrentPageCrawlingBehavior(string PageUrl)
    	{
    		if (base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFCrawlingBehaviors == null || base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFCrawlingBehaviors.Count == 0)
    		{
    			return null;
    		}
    		for (int i = 0; i < base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFCrawlingBehaviors.Count; i++)
    		{
    			if (base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFCrawlingBehaviors[i].PageUrlSubstringPattern == null || base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFCrawlingBehaviors[i].PageUrlSubstringPattern == "" || base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFCrawlingBehaviors[i].PageUrlSubstringPattern == "*" || PageUrl.IndexOf(base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFCrawlingBehaviors[i].PageUrlSubstringPattern) != -1)
    			{
    				return base.CrawlingServerEnvironmentLink.CrawlerProperties.CEFCrawlingBehaviors[i];
    			}
    		}
    		return null;
    	}

    	/// <summary>
    	/// Fires page crawled event
    	/// </summary>
    	/// <param name="DownloadedPageUrl"></param>
    	/// <param name="PageHtml"></param>
    	/// <param name="PageLinks"></param>
    	private void FirePageCrawledEvent(PageCrawlingResultMetaInformation CrawledPageMeta, PageLink DownloadedPageUrl, string PageHtml, List<PageLink> PageLinks, Bitmap PageScreenshot, Dictionary<string, string> PageFrames)
    	{
    		PageCrawled?.Invoke(new PageCrawledCallback(CrawledPageMeta, DownloadedPageUrl, PageHtml, PageLinks, PageScreenshot, PageFrames));
    	}

    	/// <summary>
    	/// Takes browser actual screenshot 
    	/// </summary>
    	/// <returns></returns>
    	public Bitmap TakeScreenshot()
    	{
    		return CEFBrowser.ScreenshotOrNull((PopupBlending)0);
    	}
    }
}
