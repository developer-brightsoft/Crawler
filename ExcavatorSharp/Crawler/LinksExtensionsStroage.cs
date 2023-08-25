// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Crawler.LinksExtensionsStroage
using System.Collections.Generic;

namespace ExcavatorSharp.Crawler
{
    /// <summary>
    /// Directory-class for storing most popular files extensions
    /// </summary>
    public sealed class LinksExtensionsStroage
    {
    	/// <summary>
    	/// Images formats
    	/// </summary>
    	public static string[] ImagesFileFormats = new string[9] { "jpg", "jpeg", "png", "svg", "gif", "tiff", "bmp", "raw", "ico" };

    	/// <summary>
    	/// Stylesheets formats
    	/// </summary>
    	public static string[] StylesheetFileFormats = new string[6] { "css", "sass", "less", "ccss", "pcss", "hss" };

    	/// <summary>
    	/// Scripts files formats
    	/// </summary>
    	public static string[] ScriptsFilesExtensions = new string[7] { "jsp", "jspx", "wss", "do", "action", "js", "pl" };

    	/// <summary>
    	/// Executable files formats
    	/// </summary>
    	public static string[] ExecutableFilesExtensions = new string[7] { "cgi", "dll", "exe", "swf", "bat", "apk", "msi" };

    	/// <summary>
    	/// Native documents formats
    	/// </summary>
    	public static string[] NativeDocumentsFilesExtensions = new string[10] { "doc", "docx", "xls", "xlsx", "csv", "ppt", "pptx", "odt", "odf", "txt" };

    	/// <summary>
    	/// Structured files formats
    	/// </summary>
    	public static string[] StructuredFilesExtensions = new string[4] { "xml", "yml", "yaml", "rss" };

    	/// <summary>
    	/// Other native files formats
    	/// </summary>
    	public static string[] OtherFilesExtensions = new string[3] { "yaws", "cfm", "cab" };

    	/// <summary>
    	/// Archives formats
    	/// </summary>
    	public static string[] ArchiveFilesExtensions = new string[11]
    	{
    		"zip", "zipx", "rar", "7z", "iso", "tar", "tgz", "bz2", "gz", "s7z",
    		"xar"
    	};

    	/// <summary>
    	/// Website content pages formats
    	/// </summary>
    	public static string[] WebsiteContentPagesExtensions = new string[16]
    	{
    		"asp", "aspx", "axd", "asx", "asmx", "ashx", "html", "htm", "xhtml", "jhtml",
    		"rhtml", "shtml", "php", "php4", "php3", "phtml"
    	};

    	/// <summary>
    	/// A library of a most popular native resources, probably linked from the indexed pages
    	/// </summary>
    	public static Dictionary<PageLinkResourceType, string[]> ResourceFormats = null;

    	/// <summary>
    	/// Get native files fromats, associated with files types
    	/// </summary>
    	/// <returns></returns>
    	public static Dictionary<PageLinkResourceType, string[]> GetNativeFilesFormats()
    	{
    		if (ResourceFormats != null)
    		{
    			return ResourceFormats;
    		}
    		ResourceFormats = new Dictionary<PageLinkResourceType, string[]>
    		{
    			{
    				PageLinkResourceType.ExecutableFileLink,
    				ExecutableFilesExtensions
    			},
    			{
    				PageLinkResourceType.ImageLink,
    				ImagesFileFormats
    			},
    			{
    				PageLinkResourceType.NativeDocumentLink,
    				NativeDocumentsFilesExtensions
    			},
    			{
    				PageLinkResourceType.OtherFileLink,
    				OtherFilesExtensions
    			},
    			{
    				PageLinkResourceType.ScriptLink,
    				ScriptsFilesExtensions
    			},
    			{
    				PageLinkResourceType.StructuredFileLink,
    				StructuredFilesExtensions
    			},
    			{
    				PageLinkResourceType.StylesheetLink,
    				StylesheetFileFormats
    			},
    			{
    				PageLinkResourceType.ArchiveFileLink,
    				ArchiveFilesExtensions
    			}
    		};
    		return ResourceFormats;
    	}
    }
}
