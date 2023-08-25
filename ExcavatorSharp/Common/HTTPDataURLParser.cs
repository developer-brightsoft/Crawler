// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Common.HTTPDataURLParser
using System;
using System.Text.RegularExpressions;

namespace ExcavatorSharp.Common
{
    /// <summary>
    /// Class for parsing data from data: URL scheme (RFC 2397)
    /// </summary>
    public static class HTTPDataURLParser
    {
    	/// <summary>
    	/// Parses data from data: URL attribute data
    	/// </summary>
    	/// <param name="AttributeData"></param>
    	/// <returns></returns>
    	public static HTTPDataURLContent ParseData(string AttributeData)
    	{
    		try
    		{
    			Match match = Regex.Match(AttributeData, "data:(?<type>.+?);base64,(?<data>.+)");
    			string value = match.Groups["data"].Value;
    			string text = match.Groups["type"].Value.Trim();
    			string contentDetails = string.Empty;
    			if (text.IndexOf('/') != -1)
    			{
    				string[] array = text.Split('/');
    				if (array.Length == 2)
    				{
    					contentDetails = array[1];
    				}
    			}
    			return new HTTPDataURLContent
    			{
    				base64Data = value,
    				contentType = text,
    				contentDetails = contentDetails
    			};
    		}
    		catch (Exception)
    		{
    			return null;
    		}
    	}
    }
}
