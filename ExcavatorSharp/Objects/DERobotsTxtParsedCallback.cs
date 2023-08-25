// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DERobotsTxtParsedCallback
using ExcavatorSharp.Crawler;

/// <summary>
/// Robots.txt parsed callback data
/// </summary>
namespace ExcavatorSharp.Objects
{
	public class DERobotsTxtParsedCallback
	{
		/// <summary>
		/// Link to parsed robots.txt file
		/// </summary>
		public DERobotsTxtFile RobotsTxtFileLink { get; set; }

		/// <summary>
		/// Is file parsed ok
		/// </summary>
		public bool IsParsedSuccessfully { get; set; }

		/// <summary>
		/// Results of Robots.txt parsing or information comment
		/// </summary>
		public string ParsingResultsInformation { get; set; }

		/// <summary>
		/// Creates new instance of RobotsTxtParsedHandler
		/// </summary>
		/// <param name="RobotsTxtFileLink">Link to robots.txt file</param>
		/// <param name="IsParsedSuccessfully">Is file parsed ok</param>
		/// <param name="ParsingResults">Results of Robots.txt parsing or information comment</param>
		public DERobotsTxtParsedCallback(DERobotsTxtFile RobotsTxtFileLink, bool IsParsedSuccessfully, string ParsingResults)
		{
			this.RobotsTxtFileLink = RobotsTxtFileLink;
			this.IsParsedSuccessfully = IsParsedSuccessfully;
			ParsingResultsInformation = ParsingResults;
		}
	}
}