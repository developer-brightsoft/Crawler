// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Crawler.PageLinkResourceType
/// <summary>
/// Probabled type of linked page - binary file (.js, .css, .jpg) or page content link?
/// </summary>
namespace ExcavatorSharp.Crawler
{
	public enum PageLinkResourceType
	{
		/// <summary>
		/// Resource not yet analyzed 
		/// </summary>
		NotAnalysedYet,
		/// <summary>
		/// We not intrested about this resourse
		/// </summary>
		NotIntrested,
		/// <summary>
		/// This is a link to content page
		/// </summary>
		ContentPageLink,
		/// <summary>
		/// This is unknown-type link
		/// </summary>
		UnknownResourceLink,
		/// <summary>
		/// This is link with special tag, like 'javascript:' or 'mail:'
		/// </summary>
		SpecialTagLink,
		/// <summary>
		/// This is a link to image - like '/image.png' or '/someimage.jpg'
		/// </summary>
		ImageLink,
		/// <summary>
		/// This is a link to stylesheet file - like '/style.css'
		/// </summary>
		StylesheetLink,
		/// <summary>
		/// This is a link to script file - like '/scripts.js' or '/actions.pl'
		/// </summary>
		ScriptLink,
		/// <summary>
		/// This is a link to executable file - like '/somefile.exe' or /somefile.bat'
		/// </summary>
		ExecutableFileLink,
		/// <summary>
		/// This is a link to native document file - like '/file1.doc' or '/table.xls'
		/// </summary>
		NativeDocumentLink,
		/// <summary>
		/// This is a link to structured file - like '/sitemap.xml' or '/news.rss'
		/// </summary>
		StructuredFileLink,
		/// <summary>
		/// This is a link to archive file - like '/packed-data.7z' or '/somedata.zip'
		/// </summary>
		ArchiveFileLink,
		/// <summary>
		/// This is a link to other-typed file - like '/somefile.cab' or something likeless
		/// </summary>
		OtherFileLink
	}
}