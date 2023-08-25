// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.ExtensionMethods.DEExtensions
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ExcavatorSharp.ExtensionMethods
{
	/// <summary>
	/// Extension methods
	/// </summary>
	public static class DEExtensions
	{
		/// <summary>
		/// Latin alphabet
		/// </summary>
		internal static readonly string LatinAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

		/// <summary>
		/// Returns the information that the thread needs to be re-created before running.
		/// </summary>
		/// <param name="ThreadLink">Current thread link</param>
		/// <returns>Information about thread current state - it is necessary to reconstruct thread before running.</returns>
		public static bool IsThreadMustBeReloadedBeforeStart(this Thread ThreadLink)
		{
			return ThreadLink == null || ThreadLink.ThreadState == ThreadState.Aborted || ThreadLink.ThreadState == ThreadState.AbortRequested || ThreadLink.ThreadState == ThreadState.Stopped || ThreadLink.ThreadState == ThreadState.StopRequested || ThreadLink.ThreadState == ThreadState.Suspended || ThreadLink.ThreadState == ThreadState.SuspendRequested;
		}

		/// <summary>
		/// Gets only characters from string, skip numbers and punctuation
		/// </summary>
		/// <param name="StringLink">Analysing string</param>
		/// <returns>Characters from string</returns>
		public static string SkipNumbersAndPunctuation(this string StringLink)
		{
			StringBuilder stringBuilder = new StringBuilder(StringLink.Length);
			for (int i = 0; i < StringLink.Length; i++)
			{
				if (!char.IsNumber(StringLink[i]) && !char.IsPunctuation(StringLink[i]))
				{
					stringBuilder.Append(StringLink[i]);
				}
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Replaces many founding elements to some value
		/// </summary>
		/// <param name="StringLink">Link to some string</param>
		/// <param name="Search">Searching elements</param>
		/// <param name="Replace">Replacing values</param>
		/// <returns>Replaced string</returns>
		public static string ReplaceMany(this string StringLink, string[] Search, string Replace)
		{
			for (int i = 0; i < Search.Length; i++)
			{
				if (StringLink.IndexOf(Search[i]) != -1)
				{
					StringLink = StringLink.Replace(Search[i], Replace);
				}
			}
			return StringLink;
		}

		/// <summary>
		/// Removes all punctuation from a string except numbers and Latin characters.
		/// </summary>
		/// <param name="StringLink">Operating string</param>
		/// <returns>Normalized string</returns>
		public static string NormalizeStringToCharactersAndNumbers(this string StringLink)
		{
			StringBuilder stringBuilder = new StringBuilder(StringLink.Length);
			for (int i = 0; i < StringLink.Length; i++)
			{
				if (char.IsNumber(StringLink[i]) || LatinAlphabet.IndexOf(StringLink[i]) != -1)
				{
					stringBuilder.Append(StringLink[i]);
				}
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// checks if url is correct
		/// </summary>
		/// <param name="Url">URl address</param>
		/// <returns>Url validation result</returns>
		public static bool CheckValidUrl(string Url)
		{
			Uri result;
			return Uri.TryCreate(Url, UriKind.Absolute, out result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
		}

		/// <summary>
		/// Get argument value by specified name
		/// </summary>
		/// <param name="StringLink"></param>
		/// <param name="ArgName"></param>
		/// <returns></returns>
		public static string GetArgumentByTagName(this string StringLink, string ArgName)
		{
			if (StringLink.IndexOf(ArgName) == -1)
			{
				return string.Empty;
			}
			int startIndex = StringLink.IndexOf(ArgName);
			int num = StringLink.IndexOf("\"", startIndex);
			int num2 = StringLink.IndexOf("\"", num + 1);
			string text = StringLink.Substring(num, num2 - num);
			text = text.Replace("\"", "");
			return text.Trim();
		}

		/// <summary>
		/// Adds $_GET-arguments to link
		/// </summary>
		/// <param name="PageAddress">Page URL</param>
		/// <param name="ValuesList">Values list</param>
		/// <returns>List of values</returns>
		public static string AddGETArgsToLink(string PageAddress, Dictionary<string, string> ValuesList)
		{
			bool flag = false;
			int num = -1;
			bool flag2 = false;
			int num2 = -1;
			for (int num3 = PageAddress.Length - 1; num3 > -1; num3--)
			{
				if (PageAddress[num3] == '?')
				{
					flag = true;
					num = num3;
					break;
				}
				if (PageAddress[num3] == '#' && !flag2)
				{
					num2 = num3;
					flag2 = true;
				}
			}
			if (!flag && !flag2)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(PageAddress).Append('?');
				foreach (KeyValuePair<string, string> Values in ValuesList)
				{
					stringBuilder.Append('&').Append(Values.Key).Append('=')
						.Append(Values.Value);
				}
				return stringBuilder.ToString();
			}
			if (!flag && flag2)
			{
				string text = $"{PageAddress.Substring(0, num2)}?";
				string arg = PageAddress.Substring(num2 + 1);
				foreach (KeyValuePair<string, string> Values2 in ValuesList)
				{
					text += $"&{Values2.Key}={Values2.Value}";
				}
				return text + $"#{arg}";
			}
			if (flag && !flag2)
			{
				foreach (KeyValuePair<string, string> Values3 in ValuesList)
				{
					PageAddress += $"&{Values3.Key}={Values3.Value}";
				}
				return PageAddress;
			}
			if (flag && flag2)
			{
				string text2 = PageAddress.Substring(0, num2);
				string arg2 = PageAddress.Substring(num + 1);
				foreach (KeyValuePair<string, string> Values4 in ValuesList)
				{
					text2 += $"&{Values4.Key}={Values4.Value}";
				}
				return text2 + $"#{arg2}";
			}
			return PageAddress;
		}

		/// <summary>
		/// Gets string size in bytes
		/// </summary>
		/// <param name="StringLink"></param>
		/// <returns></returns>
		public static double GetStringApproximateSizeInBytes(this string StringLink)
		{
			if (StringLink == null || StringLink.Length == 0)
			{
				return 0.0;
			}
			return StringLink.Length * 2;
		}
	}
}

