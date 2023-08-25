// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DataGrabbingTask
namespace ExcavatorSharp.Objects
{
    /// <summary>
    /// The grabbing task with information about page callback and data pattern
    /// </summary>
    public class DataGrabbingTask
    {
    	/// <summary>
    	/// Callback from CrawlingServer
    	/// </summary>
    	public PageCrawledCallback PageCrawledCallbackData { get; set; }

    	/// <summary>
    	/// Creates new instance of DataGrabbingTask
    	/// </summary>
    	/// <param name="PageCrawledCallbackData"></param>
    	public DataGrabbingTask(PageCrawledCallback PageCrawledCallbackData)
    	{
    		this.PageCrawledCallbackData = PageCrawledCallbackData;
    	}
    }
}
