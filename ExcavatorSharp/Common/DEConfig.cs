// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Common.DEConfig
namespace ExcavatorSharp.Common
{
    /// <summary>
    /// Common program config
    /// </summary>
    public class DEConfig
    {
    	/// <summary>
    	/// Default user-agent for all crawling threads (includes CEF and Native crawling)
    	/// </summary>
    	public const string DataExcavatorDefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";

    	/// <summary>
    	/// Application shutdown flag
    	/// </summary>
    	public static volatile bool ShutDownProgram = false;

    	/// <summary>
    	/// Manufacturer name
    	/// </summary>
    	public static string VendorDataFolderName = "PIC";

    	/// <summary>
    	/// Folder name for storing application common data and temp files
    	/// </summary>
    	public static string ApplicationDataFolderName = "DataExcavator";

    	/// <summary>
    	/// Folder name for storing CEF cache, is CEF used for crawling data
    	/// </summary>
    	public static string CEFCacheFolderName = "CEFCache";

    	/// <summary>
    	/// The name of the parameter with the iteration value used to change the source link to the page when it iteratively indexed through CrawlingThread
    	/// </summary>
    	public const string CEFCrawlingItterationHashPostfixParamName = "excavator-cef-jsitteration";

    	/// <summary>
    	/// CEF User agent for all crawling tasks
    	/// </summary>
    	public static string CEFUserAgentCommon = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";

    	/// <summary>
    	/// Link for proxy testing before crawling starts
    	/// </summary>
    	public static string ProxyServerTestingResourceLink = "https://www.wikipedia.org/";

    	/// <summary>
    	/// Substring for proxy testing, looked up after ProxyServerTestingResourceLink downloaded
    	/// </summary>
    	public static string ProxyServerTestingExpectedSubstringInSourceCode = "wikipedia";

    	/// <summary>
    	/// List of trusted hosts thru the all tasks
    	/// </summary>
    	public static string[] TrustedHostsGlobal = null;

    	/// <summary>
    	/// Overloads property ServicePointManager.DefaultConnectionLimit
    	/// </summary>
    	public static int HttpConnectionsMaxCount = 100;

    	/// <summary>
    	/// Overload property ServicePointManager.Expect100Continue
    	/// </summary>
    	public static bool HttpWebRequest_Expect100Continue = false;

    	/// <summary>
    	/// Overloads property ServicePointManager.CheckCertificateRevocationList
    	/// </summary>
    	public static bool HttpWebRequest_CheckCertificateRevocationList = false;

    	/// <summary>
    	/// Maximum value of crawl-delay directive. Applyed when robots.txt respected and crawl-delay value larger than actual variable value.
    	/// </summary>
    	public static int MaximumCrawlDelayValueInSeconds = 60;

    	/// <summary>
    	/// Variable for transferring task name at online export process
    	/// </summary>
    	public static string ExportDataOnFly_TaskNameParamName = "task-name";

    	/// <summary>
    	/// Variable for transferring encoded JSON data
    	/// </summary>
    	public static string ExportDataOnFly_EncodedContentParamName = "parsed-data";

    	/// <summary>
    	/// Varlable for transferring binary files data
    	/// </summary>
    	public static string ExportDataOnFly_BinaryContentParamName = "parsed-binary";

    	/// <summary>
    	/// Applies changed properties set
    	/// </summary>
    	public static void ApplyPropertiesSet()
    	{
    		ECHttpServingCommon.ConfigureServicePointManager();
    	}
    }
}
