// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.CEF.CEFWebsiteAuthBehavior
/// <summary>
/// Behavior for website authentication via login and password
/// </summary>
namespace ExcavatorSharp.CEF
{
	public class CEFWebsiteAuthBehavior
	{
		/// <summary>
		/// Authentication behavior example
		/// </summary>
		public const string WebsiteAuthBehaviorExample = "\r\n/* Example script: */\r\n(function(d) {\r\n\r\nd.querySelector('#login').value = 'YOURLOGIN';\r\nd.querySelector('#password').value = 'YOURPASSWORD';\r\nd.querySelector('#rememberMe').setAttribute('checked', 'checked');\r\n\r\nd.querySelector('#submit').click();\r\n\r\n}(document));\r\n";

		/// <summary>
		/// A substring list to define those pages where we check the fact of the user's login in the system.
		/// </summary>
		public string[] PagesURLSubstringsToCheckLogin { get; set; }

		/// <summary>
		/// The list of substrings by which we limit the pages on which the user's login to the system will be checked.
		/// </summary>
		public string[] PagesURLSubstringsCheckRestrictions { get; set; }

		/// <summary>
		/// URL to website login page
		/// </summary>
		public string WebsiteLoginPageURL { get; set; }

		/// <summary>
		/// Timeout to wait after redirecting to login page, in seconds
		/// </summary>
		public int WaitInSecondsBeforeAndAfterLoginScript { get; set; }

		/// <summary>
		/// JS script to execute before submit button will clicked
		/// </summary>
		public string UserLoginJSScript { get; set; }

		/// <summary>
		/// This line is used to check if a user is logged into the system. If we find this line in the HTML code of the document, it means that the user is logged in and we do nothing. If we do NOT find this line in the document, the user is NOT logged in, and we log on again.
		/// </summary>
		public string CheckUserLoggedInDocumentHTMLSubstring { get; set; }

		/// <summary>
		/// Creates new empty instance of CEFWebsiteAuthBehavior
		/// </summary>
		public CEFWebsiteAuthBehavior()
		{
		}

		/// <summary>
		/// Creates new instance of CEFWebsiteAuthBehavior
		/// </summary>
		/// <param name="PagesURLSubstringsToCheckLogin">A substring list to define those pages where we check the fact of the user's login in the system.</param>
		/// <param name="PagesURLSubstringsCheckRestrictions">The list of substrings by which we limit the pages on which the user's login to the system will be checked.</param>
		/// <param name="WebsiteLoginPageURL">URL to website login page</param>
		/// <param name="WaitInSecondsBeforeAndAfterLoginScript">Timeout to wait after redirecting to login page, in seconds</param>
		/// <param name="UserLoginJSScript">JS script to execute before submit button will clicked</param>
		/// <param name="CheckUserLoggedInDocumentHTMLSubstring">This line is used to check if a user is logged into the system. If we find this line in the HTML code of the document, it means that the user is logged in and we do nothing. If we do NOT find this line in the document, the user is NOT logged in, and we log on again.</param>
		public CEFWebsiteAuthBehavior(string[] PagesURLSubstringsToCheckLogin, string[] PagesURLSubstringsCheckRestrictions, string WebsiteLoginPageURL, int WaitInSecondsBeforeAndAfterLoginScript, string UserLoginJSScript, string CheckUserLoggedInDocumentHTMLSubstring)
		{
			this.PagesURLSubstringsToCheckLogin = PagesURLSubstringsToCheckLogin;
			this.PagesURLSubstringsCheckRestrictions = PagesURLSubstringsCheckRestrictions;
			this.WebsiteLoginPageURL = WebsiteLoginPageURL;
			this.WaitInSecondsBeforeAndAfterLoginScript = WaitInSecondsBeforeAndAfterLoginScript;
			this.UserLoginJSScript = UserLoginJSScript;
			this.CheckUserLoggedInDocumentHTMLSubstring = CheckUserLoggedInDocumentHTMLSubstring;
		}
	}
}