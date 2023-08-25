// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.CEF.CEFCrawlingBehavior
namespace ExcavatorSharp.CEF
{
    /// <summary>
    /// The behavior of the CEF indexer, which determines how to index the page and when to leave the page
    /// </summary>
    public class CEFCrawlingBehavior
    {
    	/// <summary>
    	/// URL substring pattern for pages to which Behavior will be applied. If setted to "*", will applied for all pages
    	/// </summary>
    	public string PageUrlSubstringPattern { get; set; }

    	/// <summary>
    	/// Wait a certain amount of time in seconds after the page loads, Step1
    	/// </summary>
    	public int WaitAfterPageLoaded_InSeconds_Step1 { get; set; }

    	/// <summary>
    	/// Execute some JS script without Results evaluation, Step2
    	/// </summary>
    	public string JSScriptToExecute_Step2 { get; set; }

    	/// <summary>
    	/// Wait a certain amount of time in seconds after the page loads, Step3
    	/// </summary>
    	public int WaitAfterpageLoaded_InSeconds_Step3 { get; set; }

    	/// <summary>
    	/// After the occurrence of which of the events should leave the page
    	/// </summary>
    	public CEFCrawlingPageLeaveEventType LeavePageRule { get; set; }

    	/// <summary>
    	/// JS script to execute. If LeavePageRule setted to LeavePageAfterJSEventReturnsSomeResult, this property must contains some JS function with some value to return. Usually,
    	/// it must be function with return method for end of page indexing. Otherwise (if LeavePageRule not setted to LevaePageAfterJSEventReturnsSomeResult), this function will 
    	/// invoked after page load, without results control. Step4
    	/// </summary>
    	public string JSScriptToExecuteAfterPageHTMLCodeGrabbed { get; set; }

    	/// <summary>
    	/// Value for LeavePageRule:
    	///     If setted to LeavePageAfterSomeTimeSpentInSeconds - must contain time in seconds.
    	///     If setted to LevaePageAfterJSEventReturnsSomeResult, it must contain some value that we expect from the JSScriptToExecute function to complete the page scan.
    	///     If setted to LeavePageAfterNLinksParsed, it must contain links count.
    	///     If setted to LeavePageAfterNoNewLinksParsed, it must contain links count
    	/// </summary>
    	public string LeavePageRuleValue { get; set; }

    	/// <summary>
    	/// Creates a new instance of CEFCrawlingBehavior
    	/// </summary>
    	/// <param name="PageUrlSubstringPattern">URL substring pattern for pages to which Behavior will be applied. If setted to "*", will applied for all pages</param>
    	/// <param name="LeavePageRule">After the occurrence of which of the events should leave the page</param>
    	/// <param name="JSScriptToExecuteAfterPageHTMLCodeGrabbed">JS script to execute. If LeavePageRule setted to LeavePageAfterJSEventReturnsSomeResult, this property must contains some JS function with some value to return. Usually, it must be cycled function with return method for end of page indexing. Otherwise (if LeavePageRule not setted to LevaePageAfterJSEventReturnsSomeResult), this function will invoked after page load, without results control.</param>
    	/// <param name="LeavePageRuleValue">Value for LeavePageRule: If setted to LeavePageAfterSomeTimeSpentInSeconds - must contain time in seconds. If setted to LevaePageAfterJSEventReturnsSomeResult, it must contain some value that we expect from the JSScriptToExecute function to complete the page scan. If setted to LeavePageAfterNLinksParsed, it must contain links count. If setted to LeavePageAfterNoNewLinksParsed, it must contain links count</param>
    	/// <param name="JSScriptToExecute_Step2">JS Script to execute - Step2</param>
    	/// <param name="WaitAfterPageLoaded_InSeconds_Step1">Wait a certain amount of time in seconds after the page loads, before starting to index the content - Step1</param>
    	/// <param name="WaitAfterpageLoaded_InSeconds_Step3">Wait a certain amount of time in seconds after the page loads, before starting to index the content - Step3</param>
    	public CEFCrawlingBehavior(string PageUrlSubstringPattern = "*", int WaitAfterPageLoaded_InSeconds_Step1 = 0, string JSScriptToExecute_Step2 = "", int WaitAfterpageLoaded_InSeconds_Step3 = 0, CEFCrawlingPageLeaveEventType LeavePageRule = CEFCrawlingPageLeaveEventType.LeavePageAfterIndexing, string JSScriptToExecuteAfterPageHTMLCodeGrabbed = "", string LeavePageRuleValue = "")
    	{
    		this.WaitAfterPageLoaded_InSeconds_Step1 = WaitAfterPageLoaded_InSeconds_Step1;
    		this.JSScriptToExecute_Step2 = JSScriptToExecute_Step2;
    		this.WaitAfterpageLoaded_InSeconds_Step3 = WaitAfterpageLoaded_InSeconds_Step3;
    		this.PageUrlSubstringPattern = PageUrlSubstringPattern;
    		this.LeavePageRule = LeavePageRule;
    		this.JSScriptToExecuteAfterPageHTMLCodeGrabbed = JSScriptToExecuteAfterPageHTMLCodeGrabbed;
    		this.LeavePageRuleValue = LeavePageRuleValue;
    	}
    }
}
