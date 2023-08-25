// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DERobotsTXTInnerRow
using System.Text.RegularExpressions;
using ExcavatorSharp.Crawler;
using Newtonsoft.Json;

/// <summary>
/// Entry of robots.txt file
/// </summary>
namespace ExcavatorSharp.Objects
{
	public class DERobotsTXTInnerRow
	{
		/// <summary>
		/// User-agent name
		/// </summary>
		public string UserAgent { get; set; }

		/// <summary>
		/// Row parameter name
		/// </summary>
		public DERobotsEntryType ParamName { get; set; }

		/// <summary>
		/// Row parameter value
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Value regexp if Value contains wildcard elements
		/// </summary>
		[JsonIgnore]
		public string ValueRegex { get; set; }

		/// <summary>
		/// Row index
		/// </summary>
		public int RowOriginalPosition { get; set; }

		/// <summary>
		/// Row type = { KeyValueDirective, CleanParamDirective }
		/// </summary>
		public string RowType { get; set; }

		/// <summary>
		/// Creates a new instance of DERobotsTxtEntry
		/// </summary>
		/// <param name="Param">Param name</param>
		/// <param name="Value">Param value</param>
		/// <param name="RowOriginalPosition">Row initial position into file</param>
		/// <param name="UserAgent">Parameter user-agent</param>
		public DERobotsTXTInnerRow(string UserAgent, DERobotsEntryType Param, string Value, int RowOriginalPosition, string RowType)
		{
			this.UserAgent = UserAgent;
			ParamName = Param;
			this.Value = Value;
			this.RowOriginalPosition = RowOriginalPosition;
			this.RowType = RowType;
			if ((ParamName == DERobotsEntryType.Allow || ParamName == DERobotsEntryType.Disallow) && (this.Value.IndexOf('*') != -1 || this.Value.IndexOf('$') != -1))
			{
				ValueRegex = Regex.Escape(this.Value).Replace("\\*", ".*");
			}
			else
			{
				ValueRegex = string.Empty;
			}
		}
	}
}
