// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Captcha.CR2CaptchaConnector
using System;
using System.Net;
using System.Threading;
using ExcavatorSharp.Captcha;
using ExcavatorSharp.Crawler;
using ExcavatorSharp.ExtensionMethods;
using ExcavatorSharp.Objects;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;

/// <summary>
/// Connector to 2Captcha https://2captcha.com/
/// Docs: https://2captcha.com/2captcha-api
/// </summary>
namespace ExcavatorSharp.Captcha
{
	internal class CR2CaptchaConnector : CaptchaSolverBase
	{
		/// <summary>
		/// RestClient connector
		/// </summary>
		private RestClient RestClientConnector { get; set; }

		/// <summary>
		/// 2Captcha API key
		/// </summary>
		private string APIKey => base.CaptchaResolverSettings.CaptchaSolverAdditionalParams["APIKey"];

		/// <summary>
		/// Create new instance of 2Captcha resolver
		/// </summary> 
		public CR2CaptchaConnector(CrawlingThreadCEF CrawlingThreadLink)
			: base(CrawlingThreadLink)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			RestClientConnector = new RestClient("https://2captcha.com/");
		}

		/// <summary>
		/// Sends 2Captcha request and reads response
		/// </summary>
		/// <param name="Request"></param>
		/// <returns></returns>
		private string Send2CaptchaRequest(string Request)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected O, but got Unknown
			Request = $"{Request}&key={APIKey}&soft_id={9415850}";
			RestRequest val = new RestRequest(Request, (Method)0);
			IRestResponse val2 = RestClientConnector.Execute((IRestRequest)(object)val);
			if (val2.Content.IndexOf("ERROR") != -1)
			{
				base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Error during captcha resolving; Error code = {val2.Content}");
			}
			base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "CAPTCHA processing request is sent to 2captcha.com service.");
			return val2.Content;
		}

		/// <summary>
		/// Prepares proxy string for http(s) web request
		/// </summary>
		/// <param name="usedProxy"></param>
		/// <returns></returns>
		private string PrepareProxyString(WebProxy UsedProxy)
		{
			if (UsedProxy == null)
			{
				return string.Empty;
			}
			string text = ((UsedProxy.Credentials != null) ? (UsedProxy.Credentials as NetworkCredential).UserName : string.Empty);
			string text2 = ((UsedProxy.Credentials != null) ? (UsedProxy.Credentials as NetworkCredential).Password.ToString() : string.Empty);
			string host = UsedProxy.Address.Host;
			int port = UsedProxy.Address.Port;
			if (text != string.Empty && text2 != string.Empty)
			{
				return $"{text}:{text2}@{host}:{port}";
			}
			return $"{host}:{port}";
		}

		/// <summary>
		/// Get param value by whole page HTML
		/// </summary>
		/// <param name="ParamName"></param>
		/// <returns></returns>
		private string GetAttrParamByWholePage(string ParamName, string PageHTMLSource)
		{
			int num = PageHTMLSource.IndexOf(ParamName);
			int num2 = PageHTMLSource.IndexOf("\"", num);
			int num3 = PageHTMLSource.IndexOf("\"", num2 + 1);
			if (num == -1 || num2 == -1 || num3 == -1)
			{
				return string.Empty;
			}
			string text = PageHTMLSource.Substring(num2 + 1, num3 - num2 - 1);
			return text.Trim();
		}

		/// <summary>
		/// Get captcha response (step 2)
		/// </summary>
		/// <param name="RequestID"></param>
		/// <returns></returns>
		private CR2CaptchaProcessingResponseData GetCAPTCHAResolvedCodeStep2(string RequestID, DateTime ResolvingStartDateTime)
		{
			bool flag = false;
			string empty = string.Empty;
			while (!flag)
			{
				if ((DateTime.Now - ResolvingStartDateTime).TotalSeconds > (double)base.CaptchaResolverSettings.CaptchaSolvingTimeoutSeconds)
				{
					base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "CAPTCHA resolving stopped by timeout. Captcha not resolved.");
					return new CR2CaptchaProcessingResponseData(IsResolvedOK: false, "TIMEOUT_ERROR");
				}
				Thread.Sleep(5000);
				empty = Send2CaptchaRequest($"res.php?action=get&id={RequestID}");
				if (empty == "CAPCHA_NOT_READY")
				{
					base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "CAPTCHA results are not ready yet, waiting for 2captcha.com response...");
					continue;
				}
				return new CR2CaptchaProcessingResponseData(IsResolvedOK: true, empty);
			}
			return new CR2CaptchaProcessingResponseData(IsResolvedOK: false, "UNKNOWN_ERROR");
		}

		/// <summary>
		/// Try to solve some captcha
		/// </summary>
		/// <param name="chromiumWebBrowserLink"></param>
		/// <param name="PageHTMLSource"></param>
		internal override CaptchaResolveResults TryToSolveCaptcha(string PageURL, string PageHTMLSource, WebProxy UsingProxy)
		{
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Expected O, but got Unknown
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Expected O, but got Unknown
			DateTime now = DateTime.Now;
			CaptchaResolveResults result = CaptchaResolveResults.CaptchaNotResolved;
			for (int i = 0; i < base.CaptchaResolverSettings.CaptchaSolvingAttempts; i++)
			{
				if ((DateTime.Now - now).TotalSeconds > (double)base.CaptchaResolverSettings.CaptchaSolvingTimeoutSeconds)
				{
					base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Captcha resolving stopped by timeout. Captcha not resolved.");
					return result;
				}
				CaptchaType captchaType = TryDetectCaptcha(PageHTMLSource);
				string arg = string.Empty;
				HtmlDocument val = null;
				switch (captchaType)
				{
				case CaptchaType.NoCaptcha:
					return CaptchaResolveResults.CaptchaResolved;
				case CaptchaType.ImageCaptcha:
				{
					string text5 = base.CaptchaResolverSettings.CaptchaSolverAdditionalParams["imagecaptcha_imageselector"];
					string arg4 = base.CaptchaResolverSettings.CaptchaSolverAdditionalParams["imagecaptcha_resultsselector"];
					val = new HtmlDocument();
					val.LoadHtml(PageHTMLSource);
					HtmlNode val3 = null;
					val3 = HapCssExtensionMethods.QuerySelector(val, text5);
					if (val3 == null)
					{
						base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Cannot fetch image CAPTCHA");
						return result;
					}
					string jSScript2 = "(function() { \r\n    var item = document.querySelector('" + text5 + "');\r\n    if (item) {\r\n        var clientRectangle = item.getBoundingClientRect();\r\n        if (clientRectangle) {\r\n            window.scrollTo({ top: clientRectangle.top });\r\n            return {\r\n            isOK: 1,\r\n\t\t\t\ttop: clientRectangle.top,\r\n\t\t\t\tleft: clientRectangle.left,\r\n\t\t\t\twindowInnerHeight: window.innerHeight,\r\n\t\t\t\twindowInnerWidth: window.innerWidth\r\n            };\r\n        }\r\n    } \r\n\treturn { isOK: 0 };\r\n}());";
					string text6 = base.CrawlingThreadLink.EvaluateScriptAndGetResult(jSScript2);
					string attributeValue = val3.GetAttributeValue("src", "");
					if (attributeValue == string.Empty)
					{
						base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Cannot fetch image CAPTCHA src attribute");
						return result;
					}
					BinaryResourceDownloader binaryResourceDownloader = new BinaryResourceDownloader(base.CrawlingThreadLink.CrawlingServerEnvironmentLink, UsingProxy);
					PageLink pageLink = new PageLink(attributeValue);
					WebsiteInnerLinksAnalyser websiteInnerLinksAnalyser = new WebsiteInnerLinksAnalyser();
					websiteInnerLinksAnalyser.AnalyseWebsitePageLink(base.CrawlingThreadLink.CrawlingServerEnvironmentLink.WebsiteBaseUrl, pageLink, new PageLink(PageURL)
					{
						NormalizedOriginalLink = PageURL
					});
					BinaryResourceDownloadingResult binaryResourceDownloadingResult = binaryResourceDownloader.DownloadResource(pageLink);
					if (binaryResourceDownloadingResult.ResourceDownloadException != null || binaryResourceDownloadingResult.ResourceHttpStatusCode != HttpStatusCode.OK || binaryResourceDownloadingResult.ResourceData == null)
					{
						base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Cannot fetch image CAPTCHA src attribute");
						return result;
					}
					string empty = string.Empty;
					try
					{
						empty = Convert.ToBase64String(binaryResourceDownloadingResult.ResourceData);
					}
					catch (Exception)
					{
						base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Cannot convert captcha image to base64 (trying to solve image captcha)");
						return result;
					}
					string text7 = Send2CaptchaRequest($"in.php?method=base64&body={empty}");
					CR2CaptchaFirstResponseData cR2CaptchaFirstResponseData3 = JsonConvert.DeserializeObject<CR2CaptchaFirstResponseData>(text7);
					if (cR2CaptchaFirstResponseData3.Status != 1)
					{
						base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Cannot resolve CAPTCHA; 2captcha.com returns error status; Page URL = {PageURL}");
						return result;
					}
					CR2CaptchaProcessingResponseData cAPTCHAResolvedCodeStep3 = GetCAPTCHAResolvedCodeStep2(cR2CaptchaFirstResponseData3.RequestCode, now);
					if (cAPTCHAResolvedCodeStep3.ResponseData.IndexOf("OK") != 0 || !cAPTCHAResolvedCodeStep3.IsResolvedOK)
					{
						base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Cannot solve CAPTCHA; 2captcha.com returns error; Error data = {cAPTCHAResolvedCodeStep3.ResponseData}, Page URL = {PageURL}");
						i = base.CaptchaResolverSettings.CaptchaSolvingAttempts + 1;
					}
					else
					{
						arg = cAPTCHAResolvedCodeStep3.CaptchaResolvedDataValue;
						string jSScript3 = string.Format("(function() {{ document.querySelector('{0}').innerHtml = '{1}'; document.querySelector('{0}').value = '{1}'; }}());", arg4, cAPTCHAResolvedCodeStep3.CaptchaResolvedDataValue);
						base.CrawlingThreadLink.EvaluateScriptAndGetResult(jSScript3);
					}
					break;
				}
				case CaptchaType.TextCaptcha:
				{
					string text3 = base.CaptchaResolverSettings.CaptchaSolverAdditionalParams["textcaptcha_textselector"];
					string arg2 = base.CaptchaResolverSettings.CaptchaSolverAdditionalParams["textcaptcha_resultsselector"];
					string arg3 = base.CaptchaResolverSettings.CaptchaSolverAdditionalParams["textcaptcha_languagecode"];
					val = new HtmlDocument();
					val.LoadHtml(PageHTMLSource);
					HtmlNode val2 = null;
					val2 = HapCssExtensionMethods.QuerySelector(val, text3);
					if (val2 == null)
					{
						base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Cannot fetch text CAPTCHA");
						return result;
					}
					string innerText = val2.InnerText;
					if (innerText == string.Empty)
					{
						base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Cannot fetch text CAPTCHA inner data");
						return result;
					}
					string text4 = Send2CaptchaRequest($"in.php?textcaptcha={innerText}&lang={arg3}");
					CR2CaptchaFirstResponseData cR2CaptchaFirstResponseData2 = JsonConvert.DeserializeObject<CR2CaptchaFirstResponseData>(text4);
					if (cR2CaptchaFirstResponseData2.Status != 1)
					{
						base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Cannot solve CAPTCHA; 2captcha.com returns error; Page URL = {PageURL}");
						return result;
					}
					CR2CaptchaProcessingResponseData cAPTCHAResolvedCodeStep2 = GetCAPTCHAResolvedCodeStep2(cR2CaptchaFirstResponseData2.RequestCode, now);
					if (cAPTCHAResolvedCodeStep2.ResponseData.IndexOf("OK") != 0 || !cAPTCHAResolvedCodeStep2.IsResolvedOK)
					{
						base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Cannot solve CAPTCHA; 2captcha.com returns error; Error data = {cAPTCHAResolvedCodeStep2.ResponseData}, Page URL = {PageURL}");
						i = base.CaptchaResolverSettings.CaptchaSolvingAttempts + 1;
					}
					else
					{
						arg = cAPTCHAResolvedCodeStep2.CaptchaResolvedDataValue;
						string jSScript = string.Format("(function() {{ document.querySelector('{0}').innerHtml = '{1}'; document.querySelector('{0}').value = '{1}'; }}());", arg2, cAPTCHAResolvedCodeStep2.CaptchaResolvedDataValue);
						base.CrawlingThreadLink.EvaluateScriptAndGetResult(jSScript);
					}
					break;
				}
				case CaptchaType.HCaptcha:
				{
					if (UsingProxy == null)
					{
						base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "Cannot solve hCAPTCHA without proxy. Please, add proxy to project");
						return result;
					}
					string attrParamByWholePage2 = GetAttrParamByWholePage("data-sitekey", PageHTMLSource);
					string text11 = Send2CaptchaRequest($"in.php?method=hcaptcha&sitekey={attrParamByWholePage2}&pageurl={PageURL}&json=1&proxy={PrepareProxyString(UsingProxy)}");
					CR2CaptchaFirstResponseData cR2CaptchaFirstResponseData5 = JsonConvert.DeserializeObject<CR2CaptchaFirstResponseData>(text11);
					if (cR2CaptchaFirstResponseData5.Status != 1)
					{
						base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Cannot solve CAPTCHA; 2captcha.com returns error; Page URL = {PageURL}");
						return result;
					}
					CR2CaptchaProcessingResponseData cAPTCHAResolvedCodeStep5 = GetCAPTCHAResolvedCodeStep2(cR2CaptchaFirstResponseData5.RequestCode, now);
					if (cAPTCHAResolvedCodeStep5.ResponseData.IndexOf("OK") != 0 || !cAPTCHAResolvedCodeStep5.IsResolvedOK)
					{
						base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Cannot solve CAPTCHA; 2captcha.com returns error; Error data = {cAPTCHAResolvedCodeStep5.ResponseData}, Page URL = {PageURL}");
						i = base.CaptchaResolverSettings.CaptchaSolvingAttempts + 1;
					}
					else
					{
						arg = cAPTCHAResolvedCodeStep5.CaptchaResolvedDataValue;
						string jSScript7 = string.Format("(function() {{ document.querySelector(\"[id*='g-recaptcha-response']\").value = \"{0}\"; document.querySelector(\"[id*='g-recaptcha-response']\").innerHTML = \"{0}\"; document.querySelector(\"[id*='h-recaptcha-response']\").value = \"{0}\"; document.querySelector(\"[id*='h-recaptcha-response']\").innerHTML = \"{0}\"; if (document.querySelector(\"#hcaptcha_submit\")) {{ document.querySelector(\"#hcaptcha_submit\").click(); }} }}());", cAPTCHAResolvedCodeStep5.CaptchaResolvedDataValue);
						base.CrawlingThreadLink.EvaluateScriptAndGetResult(jSScript7);
					}
					break;
				}
				case CaptchaType.RecaptchaV2:
				{
					string attrParamByWholePage = GetAttrParamByWholePage("data-sitekey", PageHTMLSource);
					string text8 = $"in.php?method=userrecaptcha&googlekey={attrParamByWholePage}&pageurl={PageURL}&json=1&proxy={PrepareProxyString(UsingProxy)}";
					if (PageHTMLSource.IndexOf("size=invisible") != -1 || PageHTMLSource.IndexOf("invisible=1") != -1)
					{
						text8 += "&invisible=1";
					}
					string text9 = Send2CaptchaRequest(text8);
					CR2CaptchaFirstResponseData cR2CaptchaFirstResponseData4 = JsonConvert.DeserializeObject<CR2CaptchaFirstResponseData>(text9);
					if (cR2CaptchaFirstResponseData4.Status != 1)
					{
						base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Cannot solve CAPTCHA; 2captcha.com returns error; Page URL = {PageURL}");
						return result;
					}
					CR2CaptchaProcessingResponseData cAPTCHAResolvedCodeStep4 = GetCAPTCHAResolvedCodeStep2(cR2CaptchaFirstResponseData4.RequestCode, now);
					if (cAPTCHAResolvedCodeStep4.ResponseData.IndexOf("OK") != 0 || !cAPTCHAResolvedCodeStep4.IsResolvedOK)
					{
						base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Cannot solve CAPTCHA; 2captcha.com returns error; Error data = {cAPTCHAResolvedCodeStep4.ResponseData}, Page URL = {PageURL}");
						i = base.CaptchaResolverSettings.CaptchaSolvingAttempts + 1;
						break;
					}
					arg = cAPTCHAResolvedCodeStep4.CaptchaResolvedDataValue;
					string jSScript4 = base.CrawlingThreadLink.EvaluateScriptAndGetResult($"(function() {{ document.getElementById('g-recaptcha-response').innerHTML = '{cAPTCHAResolvedCodeStep4.CaptchaResolvedDataValue}'; if (document.querySelector('.g-recaptcha').closest('form').querySelector('input[type=submit]')) document.querySelector('.g-recaptcha').closest('form').querySelector('input[type=submit]').click(); }}());");
					base.CrawlingThreadLink.EvaluateScriptAndGetResult(jSScript4);
					string argumentByTagName = PageHTMLSource.GetArgumentByTagName("data-callback");
					if (argumentByTagName.Length > 0)
					{
						string jSScript5 = $"(function() {{ {argumentByTagName}('{cAPTCHAResolvedCodeStep4.CaptchaResolvedDataValue}'); }}());";
						base.CrawlingThreadLink.EvaluateScriptAndGetResult(jSScript5);
						base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "reCaptchaV2 solved with «data-callback substring» method");
					}
					int num = PageHTMLSource.IndexOf("grecaptcha.render");
					if (num != -1)
					{
						try
						{
							int num2 = PageHTMLSource.IndexOf("}", num);
							if (num2 != -1)
							{
								int num3 = PageHTMLSource.IndexOf("callback", num, num2 - num);
								if (num3 != -1)
								{
									num3 += "callback".Length;
									int num4 = PageHTMLSource.IndexOf(":", num3);
									int num5 = PageHTMLSource.IndexOf(",", num4);
									string text10 = PageHTMLSource.Substring(num4, num5 - num4);
									text10 = text10.Trim();
									base.CrawlingThreadLink.EvaluateScriptAndGetResult($"(function() {{ {text10}('{cAPTCHAResolvedCodeStep4.CaptchaResolvedDataValue}'); }}());");
									base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "reCaptchaV2 solved with «grecaptcha.render» method");
								}
							}
						}
						catch (Exception)
						{
						}
					}
					string jSScript6 = string.Format("(function() {{ if (___grecaptcha_cfg && ___grecaptcha_cfg.clients && ___grecaptcha_cfg.clients[0].aa && ___grecaptcha_cfg.clients[0].aa.l && ___grecaptcha_cfg.clients[0].aa.callback) ___grecaptcha_cfg.clients[0].aa.l.callback('{0}'); /* ||| */ if (___grecaptcha_cfg && ___grecaptcha_cfg.clients && ___grecaptcha_cfg.clients[1].aa && ___grecaptcha_cfg.clients[1].aa.l && ___grecaptcha_cfg.clients[1].aa.callback) ___grecaptcha_cfg.clients[1].aa.l.callback('{0}'); /* ||| */ if (___grecaptcha_cfg && ___grecaptcha_cfg.clients && ___grecaptcha_cfg.clients[2].aa && ___grecaptcha_cfg.clients[2].aa.l && ___grecaptcha_cfg.clients[2].aa.callback) ___grecaptcha_cfg.clients[2].aa.l.callback('{0}'); }}());", cAPTCHAResolvedCodeStep4.CaptchaResolvedDataValue);
					base.CrawlingThreadLink.EvaluateScriptAndGetResult(jSScript6);
					break;
				}
				case CaptchaType.RecaptchaV3:
				{
					int j = PageHTMLSource.IndexOf("google.com/recaptcha/api.js?render=");
					string text = string.Empty;
					for (; j < PageHTMLSource.Length && !char.IsPunctuation(PageHTMLSource[j]) && PageHTMLSource[j] != '"' && PageHTMLSource[j] != '\'' && !char.IsWhiteSpace(PageHTMLSource[j]); j++)
					{
						text += PageHTMLSource[j];
					}
					string request = $"in.php?method=userrecaptcha&version=v3&action=verify&min_score=0.3&googlekey={text}&pageurl={PageURL}&json=1&proxy={PrepareProxyString(UsingProxy)}";
					string text2 = Send2CaptchaRequest(request);
					CR2CaptchaFirstResponseData cR2CaptchaFirstResponseData = JsonConvert.DeserializeObject<CR2CaptchaFirstResponseData>(text2);
					if (cR2CaptchaFirstResponseData.Status != 1)
					{
						base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Cannot solve CAPTCHA; 2captcha.com returns error; Page URL = {PageURL}");
						return result;
					}
					bool flag = false;
					CR2CaptchaProcessingResponseData cAPTCHAResolvedCodeStep = GetCAPTCHAResolvedCodeStep2(cR2CaptchaFirstResponseData.RequestCode, now);
					if (cAPTCHAResolvedCodeStep.ResponseData.IndexOf("OK") != 0 || !cAPTCHAResolvedCodeStep.IsResolvedOK)
					{
						base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"Cannot solve CAPTCHA; 2captcha.com returns error; Error data = {cAPTCHAResolvedCodeStep.ResponseData}, Page URL = {PageURL}");
						i = base.CaptchaResolverSettings.CaptchaSolvingAttempts + 1;
					}
					else
					{
						arg = cAPTCHAResolvedCodeStep.CaptchaResolvedDataValue;
					}
					break;
				}
				}
				string jSScript8 = string.Format("(function() {{ var captchaResolvedResponse = document.createElement('div'); captchaResolvedResponse.setAttribute('id', 'de-capresponse'); captchaResolvedResponse.setAttribute('data-response', '{1}'); document.body.appendChild(captchaResolvedResponse); }}());", arg);
				base.CrawlingThreadLink.EvaluateScriptAndGetResult(jSScript8);
				if (base.CaptchaResolverSettings.AdditionalJavaScriptAfterCaptchaPassed != string.Empty)
				{
					base.CrawlingThreadLink.EvaluateScriptAndGetResult(base.CaptchaResolverSettings.AdditionalJavaScriptAfterCaptchaPassed);
				}
				_ = base.CaptchaResolverSettings.WaitAfterCaptchaWasSolvedSeconds;
				if (true)
				{
					Thread.Sleep(base.CaptchaResolverSettings.WaitAfterCaptchaWasSolvedSeconds * 1000);
				}
				string pageActualSourceCode = base.CrawlingThreadLink.GetPageActualSourceCode();
				if (IsCaptchaShown(PageURL, pageActualSourceCode))
				{
					base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, $"CAPTCA hasn't solved. Attempt #{i}/{base.CaptchaResolverSettings.CaptchaSolvingAttempts} failed.");
				}
			}
			return result;
		}

		/// <summary>
		/// Returns whether the captcha is shown on the page.
		/// </summary>
		/// <param name="PageHTMLSource">Soure page code</param>
		/// <returns></returns>
		internal override bool IsCaptchaShown(string PageURL, string PageHTMLSource)
		{
			bool result = false;
			PageHTMLSource = PageHTMLSource.ToLower();
			CaptchaType captchaType = TryDetectCaptcha(PageHTMLSource);
			switch (captchaType)
			{
			case CaptchaType.NoCaptcha:
				return false;
			case CaptchaType.ImageCaptcha:
				result = true;
				base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "The page contains image captcha.");
				break;
			}
			if (captchaType == CaptchaType.TextCaptcha)
			{
				result = true;
				base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "The page contains text captcha.");
			}
			if (captchaType == CaptchaType.HCaptcha)
			{
				result = true;
				base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "The page contains hCAPTCHA.");
			}
			if (captchaType == CaptchaType.RecaptchaV2)
			{
				result = true;
				base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "The page contains ReCaptcha V2");
			}
			if (captchaType == CaptchaType.RecaptchaV3)
			{
				result = true;
				base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "The page contains ReCaptcha V3");
			}
			if (base.CaptchaResolverSettings.CaptchaSolverAdditionalParams.ContainsKey("captcha-userdefinedcaptchasubstring") && captchaType == CaptchaType.UserDefinedCaptcha)
			{
				result = true;
				base.CrawlingThreadLink.CrawlingServerEnvironmentLink.TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.CrawlingServer, "The page contains user-specified CAPTCHA");
			}
			return result;
		}

		/// <summary>
		/// Try to detect captcha on page
		/// </summary>
		/// <param name="PageHTMLSource"></param>
		/// <returns></returns>
		public CaptchaType TryDetectCaptcha(string PageHTMLSource)
		{
			if (base.CaptchaResolverSettings.ForceSpecifiedCaptchaDetectionSubstring != string.Empty && PageHTMLSource.IndexOf(base.CaptchaResolverSettings.ForceSpecifiedCaptchaDetectionSubstring) != -1)
			{
				return base.CaptchaResolverSettings.ForceSpecifiedCaptchaType;
			}
			if (PageHTMLSource.IndexOf("iframe") != -1 && PageHTMLSource.IndexOf("hcaptcha") != -1 && PageHTMLSource.IndexOf("data-sitekey") != -1)
			{
				return CaptchaType.HCaptcha;
			}
			if (PageHTMLSource.IndexOf("iframe") != -1 && PageHTMLSource.IndexOf("recaptcha") != -1 && PageHTMLSource.IndexOf("data-sitekey") != -1)
			{
				return CaptchaType.RecaptchaV2;
			}
			if (PageHTMLSource.IndexOf("https://www.google.com/recaptcha/api.js?render") != -1)
			{
				return CaptchaType.RecaptchaV3;
			}
			if (base.CaptchaResolverSettings.CaptchaSolverAdditionalParams.ContainsKey("captcha-detectionsubstring") && PageHTMLSource.IndexOf(base.CaptchaResolverSettings.CaptchaSolverAdditionalParams["captcha-userdefinedcaptchasubstring"]) != -1)
			{
				return CaptchaType.UserDefinedCaptcha;
			}
			return CaptchaType.NoCaptcha;
		}
	}
}