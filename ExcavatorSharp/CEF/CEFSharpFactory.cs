// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.CEF.CEFSharpFactory
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using ExcavatorSharp.Common;

/// <summary>
/// Chrome Entiti Framework - CEFSharp initialization factory and common methods
/// Source repo: https://github.com/cefsharp/cefsharp
/// </summary>
namespace ExcavatorSharp.CEF
{
	public class CEFSharpFactory
	{
		/// <summary>
		/// Determines that CEF was initialized at current application session
		/// </summary>
		internal static volatile bool CEFInitialized = false;

		/// <summary>
		/// Determines CEF cache directory (Lazy variable)
		/// </summary>
		internal static string CEFTempDirectory = "";

		/// <summary>
		/// Mutex for CEF initialization process
		/// </summary>
		internal static Mutex CEFInitializationMutex = new Mutex();

		/// <summary>
		/// CEFSharp request context settings
		/// </summary>
		internal static RequestContextSettings CEFRequestContextSettings = null;

		/// <summary>
		/// Makes CEF initialization
		/// </summary>
		public static void InitializeCEFBrowser()
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Expected O, but got Unknown
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Expected O, but got Unknown
			CEFInitializationMutex.WaitOne();
			if (CEFInitialized)
			{
				CEFInitializationMutex.ReleaseMutex();
				return;
			}
			string text = $"{IOCommon.GetDataExcavatorCommonIOPath()}/{DEConfig.CEFCacheFolderName}";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			CEFTempDirectory = text;
			CefSettings val = new CefSettings
			{
				CachePath = text
			};
			((AbstractCefSettings)val).IgnoreCertificateErrors = true;
			((AbstractCefSettings)val).UserAgent = DEConfig.CEFUserAgentCommon;
			Cef.Initialize((AbstractCefSettings)(object)val);
			CEFInitialized = true;
			CEFRequestContextSettings = new RequestContextSettings
			{
				CachePath = CEFTempDirectory
			};
			CEFInitializationMutex.ReleaseMutex();
		}

		/// <summary>
		/// Shutting down CEF browser and its components
		/// </summary>
		public static void ShutdownCEF()
		{
			if (CEFInitialized)
			{
				try
				{
					Cef.Shutdown();
				}
				catch (Exception)
				{
				}
				CEFInitialized = false;
			}
		}

		/// <summary>
		/// Make JS-script testing in CEF environment
		/// </summary>
		/// <param name="JSScript">Script to execute in CEF environment</param>
		/// <returns>Script testing results</returns>
		public static string TestJSScript(string JSScript, string PageURL)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected O, but got Unknown
			try
			{
				InitializeCEFBrowser();
			}
			catch (Exception)
			{
			}
			BrowserSettings val = new BrowserSettings();
			RequestContextSettings val2 = new RequestContextSettings
			{
				CachePath = CEFTempDirectory
			};
			RequestContext val3 = new RequestContext(val2);
			ChromiumWebBrowser chromiumWB = new ChromiumWebBrowser(string.Empty, val, (IRequestContext)(object)val3, true);
			chromiumWB.Size = new Size(1900, 1400);
			DateTime now = DateTime.Now;
			while (!chromiumWB.IsBrowserInitialized && !DEConfig.ShutDownProgram)
			{
				Thread.Sleep(500);
				DateTime now2 = DateTime.Now;
				if ((now2 - now).TotalSeconds > 30.0)
				{
					break;
				}
			}
			if (!chromiumWB.IsBrowserInitialized)
			{
				return "ERROR_CANNOT_INITIALIZE_CHROMIUM";
			}
			AsyncHelpers.RunSync(() => LoadPageWithGETAsync(PageURL, chromiumWB));
			Thread.Sleep(3000);
			JavascriptResponse val4 = AsyncHelpers.RunSync(() => WebBrowserExtensions.EvaluateScriptAsync((IWebBrowser)(object)chromiumWB, JSScript, (TimeSpan?)null));
			string empty = string.Empty;
			empty = ((!val4.Success) ? val4.Message : "OK");
			chromiumWB.Dispose();
			chromiumWB = null;
			val3 = null;
			return empty;
		}

		/// <summary>
		/// Loads page by specified URL asyncronycally with $_GET method
		/// </summary>
		/// <param name="CEFBrowserLink"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		private static Task LoadPageWithGETAsync(string address, ChromiumWebBrowser cefBrowserLink)
		{
			TaskCompletionSource<bool> TaskSource = new TaskCompletionSource<bool>(TaskCreationOptions.None);
			EventHandler<LoadingStateChangedEventArgs> CEFPageLoadedHandler = null;
			CEFPageLoadedHandler = delegate(object sender, LoadingStateChangedEventArgs args)
			{
				if (!args.IsLoading)
				{
					cefBrowserLink.LoadingStateChanged -= CEFPageLoadedHandler;
					TaskSource.TrySetResult(result: true);
				}
			};
			cefBrowserLink.LoadingStateChanged += CEFPageLoadedHandler;
			cefBrowserLink.Load(address);
			return TaskSource.Task;
		}
	}
}
