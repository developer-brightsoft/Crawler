// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Common.HTMLFramesHelper
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

/// <summary>
/// Helper for HTML entities execution
/// </summary>
/// 
namespace ExcavatorSharp.Common
{
	public static class HTMLFramesHelper
	{
		/// <summary>
		/// Return index of IFrame url
		/// </summary>
		/// <param name="HTMLDocumentObject"></param>
		/// <param name="Substring"></param>
		/// <param name="StartIndex"></param>
		/// <returns></returns>
		public static int IndexOfIFrameURL(this string HTMLDocumentObject, string Substring, int StartIndex)
		{
			if (HTMLDocumentObject.IndexOf(Substring, StartIndex, StringComparison.InvariantCultureIgnoreCase) != -1)
			{
				return HTMLDocumentObject.IndexOf(Substring, StartIndex, StringComparison.InvariantCultureIgnoreCase);
			}
			string value = HttpUtility.HtmlEncode(Substring);
			if (HTMLDocumentObject.IndexOf(value, StartIndex, StringComparison.InvariantCultureIgnoreCase) != -1)
			{
				return HTMLDocumentObject.IndexOf(value, StartIndex, StringComparison.InvariantCultureIgnoreCase);
			}
			value = HttpUtility.HtmlDecode(Substring);
			if (HTMLDocumentObject.IndexOf(value, StartIndex, StringComparison.InvariantCultureIgnoreCase) != -1)
			{
				return HTMLDocumentObject.IndexOf(value, StartIndex, StringComparison.InvariantCultureIgnoreCase);
			}
			return -1;
		}

		/// <summary>
		/// Return index of IFrame url
		/// </summary>
		/// <param name="HTMLDocumentObject"></param>
		/// <param name="Substring"></param>
		/// <param name="StartIndex"></param>
		/// <returns></returns>
		public static int IndexOfIFrameURL(this string HTMLDocumentObject, string Substring, int StartIndex, int Count)
		{
			if (HTMLDocumentObject.IndexOf(Substring, StartIndex, Count, StringComparison.InvariantCultureIgnoreCase) != -1)
			{
				return HTMLDocumentObject.IndexOf(Substring, StartIndex, Count, StringComparison.InvariantCultureIgnoreCase);
			}
			string value = HttpUtility.HtmlEncode(Substring);
			if (HTMLDocumentObject.IndexOf(value, StartIndex, Count, StringComparison.InvariantCultureIgnoreCase) != -1)
			{
				return HTMLDocumentObject.IndexOf(value, StartIndex, Count, StringComparison.InvariantCultureIgnoreCase);
			}
			value = HttpUtility.HtmlDecode(Substring);
			if (HTMLDocumentObject.IndexOf(value, StartIndex, Count, StringComparison.InvariantCultureIgnoreCase) != -1)
			{
				return HTMLDocumentObject.IndexOf(value, StartIndex, Count, StringComparison.InvariantCultureIgnoreCase);
			}
			return -1;
		}

		/// <summary>
		/// Expands page iframes
		/// </summary>
		/// <param name="PageHTMLSource">Page source HTML</param>
		/// <param name="PageFrames">Page frames list</param>
		/// <returns>Page with replaced iframes</returns>
		public static string ExpandIFrames(string PageHTMLSource, Dictionary<string, string> PageFrames)
		{
			string text = PageHTMLSource;
			foreach (KeyValuePair<string, string> PageFrame in PageFrames)
			{
				string key = PageFrame.Key;
				int num = IndexOfIFrameURL(text, key, 0);
				while (num != -1)
				{
					int num2 = num;
					while (num2 > -1 && text[num2] != '<')
					{
						num2--;
					}
					int num3 = text.IndexOf("iframe", num2, num - num2, StringComparison.InvariantCultureIgnoreCase);
					if (num3 == -1)
					{
						num = -1;
						continue;
					}
					int num4 = text.IndexOf('>', num);
					string text2 = text.Substring(0, num4 + 1);
					string text3 = text.Substring(num4 + 1);
					StringBuilder stringBuilder = new StringBuilder(text2.Length + text3.Length + PageFrame.Value.Length);
					stringBuilder.Append(text2).Append(PageFrame.Value).Append(text3);
					text = stringBuilder.ToString();
					stringBuilder.Clear();
					num = IndexOfIFrameURL(text, key, num + PageFrame.Value.Length + 1);
				}
			}
			return text;
		}
	}
}