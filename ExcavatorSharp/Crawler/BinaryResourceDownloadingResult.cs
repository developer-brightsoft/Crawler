// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Crawler.BinaryResourceDownloadingResult
using System;
using System.IO;
using System.Net;

namespace ExcavatorSharp.Crawler
{
	/// <summary>
	/// Binary resource downloading result
	/// </summary>
	internal class BinaryResourceDownloadingResult
	{
		/// <summary>
		/// Resulted binary data
		/// </summary>
		public byte[] ResourceData { get; set; }

		/// <summary>
		/// Resulted HTTP code
		/// </summary>
		public HttpStatusCode ResourceHttpStatusCode { get; set; }

		/// <summary>
		/// Number of download attempt
		/// </summary>
		public int ResourceDownloadAttempts { get; set; }

		/// <summary>
		/// Time spented for resource downloading
		/// </summary>
		public TimeSpan ResourceDownloadTime { get; set; }

		/// <summary>
		/// Resource download occured exception
		/// </summary>
		public Exception ResourceDownloadException { get; set; }

		/// <summary>
		/// Creates new instance of BinaryResourceDownloadingResult
		/// </summary>
		/// <param name="ResourceData">Downloaded resource data</param>
		/// <param name="ResourceHttpStatusCode">Resource response status code</param>
		/// <param name="ResourceDownloadAttempts">Number of download attempt</param>
		public BinaryResourceDownloadingResult(byte[] ResourceData, HttpStatusCode ResourceHttpStatusCode, int ResourceDownloadAttempts, TimeSpan ResourceDownloadTime, Exception ResourceDownloadException)
		{
			this.ResourceData = ResourceData;
			this.ResourceHttpStatusCode = ResourceHttpStatusCode;
			this.ResourceDownloadAttempts = ResourceDownloadAttempts;
			this.ResourceDownloadTime = ResourceDownloadTime;
			this.ResourceDownloadException = ResourceDownloadException;
		}

		/// <summary>
		/// Converts resource to string. If error occured, returns string.Empty
		/// </summary>
		/// <returns>Resource string</returns>
		public string ConvertResourceToString()
		{
			string result = string.Empty;
			try
			{
				using MemoryStream stream = new MemoryStream(ResourceData);
				using TextReader textReader = new StreamReader(stream);
				result = textReader.ReadToEnd();
			}
			catch (Exception)
			{
			}
			return result;
		}
	}
}
