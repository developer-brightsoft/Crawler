// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Crawler.WebsiteInnerLinksAnalyser
using System;
using System.Collections.Generic;
using System.Linq;
using ExcavatorSharp.Objects;

namespace ExcavatorSharp.Crawler
{
    /// <summary>
    /// Service for analysing and normalizing website pages links
    /// </summary>
    public class WebsiteInnerLinksAnalyser
    {
    	/// <summary>
    	/// Special links types
    	/// </summary>
    	public static string[] UnindexingLinksSubstrings = new string[13]
    	{
    		"javascript:", "mailto:", "skype:", "callto:", "tel:", "#", "android-app:", "telnet:", "imap:", "ipps:",
    		"message:", "mms:", "bitcoin:"
    	};

    	/// <summary>
    	/// Separates website real links from all href links, founded into page
    	/// </summary>
    	/// <param name="WebsiteUrl"></param>
    	/// <param name="WebsiteLinks"></param>
    	/// <returns></returns>
    	public List<PageLink> AnalyseWebsitePageLinks(Uri WebsiteUrl, PageLink SourcePage, List<PageLink> WebsiteLinks)
    	{
    		List<PageLink> list = new List<PageLink>();
    		for (int i = 0; i < WebsiteLinks.Count; i++)
    		{
    			AnalyseWebsitePageLink(WebsiteUrl, SourcePage, WebsiteLinks[i]);
    			list.Add(WebsiteLinks[i]);
    		}
    		return list;
    	}

    	/// <summary>
    	/// Normalizes next website page link to absolute URL. You only must define WebsiteRoot URL and UrlFromSourcePageHTML. SourcePageUrl can be null.
    	/// </summary>
    	/// <param name="WebsiteRootUrl">Website root URL</param>
    	/// <param name="SourcePageUrl">Source page url</param>
    	/// <param name="UrlFromSourcePageHTML">Link to normalize</param>
    	public void AnalyseWebsitePageLink(Uri WebsiteRootUrl, PageLink SourcePageUrl, PageLink UrlFromSourcePageHTML)
    	{
    		string text = $"{WebsiteRootUrl.Scheme.ToLower()}://{WebsiteRootUrl.Authority.ToLower()}";
    		string text2 = UrlFromSourcePageHTML.OriginalLink;
    		string text3 = UrlFromSourcePageHTML.OriginalLinkLowercase;
    		string value = ((text.IndexOf("www.") != -1) ? text : $"{WebsiteRootUrl.Scheme.ToLower()}:www.//{WebsiteRootUrl.Authority.ToLower()}");
    		string value2 = ((text.IndexOf("www.") == -1) ? text : text.Replace("www.", string.Empty));
    		if (SourcePageUrl == null)
    		{
    			SourcePageUrl = new PageLink(text);
    			SourcePageUrl.NormalizedOriginalLink = SourcePageUrl.OriginalLink;
    			SourcePageUrl.LinkOwnerType = PageLinkOwnerType.InnerLink;
    			SourcePageUrl.LinkResourceType = PageLinkResourceType.ContentPageLink;
    		}
    		string[] unindexingLinksSubstrings = UnindexingLinksSubstrings;
    		foreach (string value3 in unindexingLinksSubstrings)
    		{
    			if (text3.IndexOf(value3) == 0)
    			{
    				UrlFromSourcePageHTML.NormalizedOriginalLink = text2;
    				UrlFromSourcePageHTML.LinkOwnerType = PageLinkOwnerType.InnerLink;
    				UrlFromSourcePageHTML.LinkResourceType = PageLinkResourceType.SpecialTagLink;
    				return;
    			}
    		}
    		if (text3.IndexOf("//") == 0)
    		{
    			if (Uri.IsWellFormedUriString(SourcePageUrl.OriginalLink, UriKind.RelativeOrAbsolute))
    			{
    				Uri uri = new Uri(SourcePageUrl.NormalizedOriginalLink);
    				text3 = $"{uri.Scheme}:{text3}";
    				text2 = $"{uri.Scheme}:{text2}";
    			}
    			else
    			{
    				text3 = $"{WebsiteRootUrl.Scheme}:{text3}";
    				text2 = $"{WebsiteRootUrl.Scheme}:{text2}";
    			}
    		}
    		if (text3.IndexOf("http://") == 0 || text3.IndexOf("https://") == 0)
    		{
    			if (text.IndexOf("www.") != -1 && text3.IndexOf(value) != 0)
    			{
    				UrlFromSourcePageHTML.NormalizedOriginalLink = text2;
    				UrlFromSourcePageHTML.LinkOwnerType = PageLinkOwnerType.OuterLink;
    				UrlFromSourcePageHTML.LinkResourceType = PageLinkResourceType.NotIntrested;
    				if (UrlFromSourcePageHTML.NormalizedOriginalLink.IndexOf(".") != -1)
    				{
    					UrlFromSourcePageHTML.LinkResourceExtension = GetFileExtension(UrlFromSourcePageHTML.NormalizedOriginalLink);
    				}
    				UrlFromSourcePageHTML.IsLinkResolvedAndWellFormattedToAbsolutePathUrl = Uri.IsWellFormedUriString(text2, UriKind.RelativeOrAbsolute);
    				return;
    			}
    			if (text.IndexOf("www.") == -1 && text3.IndexOf(value2) != 0)
    			{
    				UrlFromSourcePageHTML.NormalizedOriginalLink = text2;
    				UrlFromSourcePageHTML.LinkOwnerType = PageLinkOwnerType.OuterLink;
    				UrlFromSourcePageHTML.LinkResourceType = PageLinkResourceType.NotIntrested;
    				if (UrlFromSourcePageHTML.NormalizedOriginalLink.IndexOf(".") != -1)
    				{
    					UrlFromSourcePageHTML.LinkResourceExtension = GetFileExtension(UrlFromSourcePageHTML.NormalizedOriginalLink);
    				}
    				UrlFromSourcePageHTML.IsLinkResolvedAndWellFormattedToAbsolutePathUrl = Uri.IsWellFormedUriString(text2, UriKind.RelativeOrAbsolute);
    				return;
    			}
    		}
    		else
    		{
    			if (text3.IndexOf("ftp://") == 0 || text3.IndexOf("ftps://") == 0)
    			{
    				UrlFromSourcePageHTML.NormalizedOriginalLink = text2;
    				UrlFromSourcePageHTML.LinkOwnerType = PageLinkOwnerType.OuterLink;
    				UrlFromSourcePageHTML.LinkResourceType = PageLinkResourceType.NotIntrested;
    				if (UrlFromSourcePageHTML.NormalizedOriginalLink.IndexOf(".") != -1)
    				{
    					UrlFromSourcePageHTML.LinkResourceExtension = GetFileExtension(UrlFromSourcePageHTML.NormalizedOriginalLink);
    				}
    				UrlFromSourcePageHTML.IsLinkResolvedAndWellFormattedToAbsolutePathUrl = Uri.IsWellFormedUriString(text2, UriKind.RelativeOrAbsolute);
    				return;
    			}
    			if (text3.IndexOf("../") == 0 && SourcePageUrl != null)
    			{
    				text3 = $"{SourcePageUrl.NormalizedOriginalLink}/{text3}";
    				text2 = $"{SourcePageUrl.NormalizedOriginalLink}/{text2}";
    			}
    			else if (text3.Length >= 2 && text3.Substring(0, 2) == "//")
    			{
    				text3 = $"{WebsiteRootUrl.Scheme.ToLower()}:{text3}";
    				text2 = $"{WebsiteRootUrl.Scheme.ToLower()}:{text2}";
    			}
    			else if (text3.IndexOf("/") == 0)
    			{
    				text3 = $"{text}{text3}";
    				text2 = $"{text}{text2}";
    			}
    			else
    			{
    				if (SourcePageUrl == null)
    				{
    					throw new NullReferenceException("There is no way to process this link. Please, use normalized link like http(s)://website.domain/path-to-page/");
    				}
    				text3 = $"{SourcePageUrl.NormalizedOriginalLink}/{text3}";
    				text2 = $"{SourcePageUrl.NormalizedOriginalLink}/{text2}";
    			}
    		}
    		UrlFromSourcePageHTML.LinkOwnerType = PageLinkOwnerType.InnerLink;
    		UrlFromSourcePageHTML.NormalizedOriginalLink = text2;
    		UrlFromSourcePageHTML.IsLinkResolvedAndWellFormattedToAbsolutePathUrl = Uri.IsWellFormedUriString(text2, UriKind.RelativeOrAbsolute);
    		string linkWithoutArguments = GetLinkWithoutArguments(text3);
    		string text4 = (UrlFromSourcePageHTML.LinkResourceExtension = GetFileExtension(linkWithoutArguments));
    		if (text4 == string.Empty || LinksExtensionsStroage.WebsiteContentPagesExtensions.Contains(text4) || linkWithoutArguments.IndexOf(value2) == 0 || linkWithoutArguments.IndexOf(value) == 0)
    		{
    			UrlFromSourcePageHTML.LinkResourceType = PageLinkResourceType.ContentPageLink;
    			return;
    		}
    		bool flag = false;
    		Dictionary<PageLinkResourceType, string[]> nativeFilesFormats = LinksExtensionsStroage.GetNativeFilesFormats();
    		foreach (KeyValuePair<PageLinkResourceType, string[]> item in nativeFilesFormats)
    		{
    			if (item.Value.Contains(text4))
    			{
    				UrlFromSourcePageHTML.LinkResourceType = item.Key;
    				flag = true;
    				break;
    			}
    		}
    		if (!flag)
    		{
    			UrlFromSourcePageHTML.LinkResourceType = PageLinkResourceType.UnknownResourceLink;
    		}
    	}

    	/// <summary>
    	/// Returns a file extension or empty string, if link goes to a content page.
    	/// </summary>
    	/// <param name="Link"></param>
    	/// <returns></returns>
    	public string GetFileExtension(string Link)
    	{
    		if (Link.IndexOf('#') != -1 || Link.IndexOf('?') != -1)
    		{
    			Link = GetLinkWithoutArguments(Link);
    		}
    		string text = string.Empty;
    		int num = -1;
    		int num2 = Link.Length - 1;
    		while (num2 >= 0 && Link[num2] != '/')
    		{
    			if (Link[num2] == '.')
    			{
    				num = num2;
    				break;
    			}
    			num2--;
    		}
    		if (num != -1)
    		{
    			text = Link.Substring(num + 1);
    		}
    		if (text.IndexOf(';') != -1)
    		{
    			text = text.Substring(0, text.IndexOf(';'));
    		}
    		return text;
    	}

    	/// <summary>
    	/// Returns a link to a file or page with no arguments.
    	/// </summary>
    	/// <param name="Link">File or page link with arguments</param>
    	/// <returns>Cleared link without arguments</returns>
    	public string GetLinkWithoutArguments(string Link)
    	{
    		if (!Link.Contains('#') && !Link.Contains('?'))
    		{
    			return Link;
    		}
    		int num = -1;
    		for (int i = 0; i < Link.Length; i++)
    		{
    			if (Link[i] == '#' || Link[i] == '?')
    			{
    				num = i;
    				break;
    			}
    		}
    		if (num != -1)
    		{
    			return Link.Substring(0, num);
    		}
    		return string.Empty;
    	}
    }
}
