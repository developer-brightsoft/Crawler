// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Crawler.ProxyAccessor
using System;
using System.Net;
using System.Threading;
using ExcavatorSharp.Objects;

/// <summary>
/// Provider for safe accessing CrawlingServer proxies and retrieving proxy for usage
/// </summary>
namespace ExcavatorSharp.Crawler
{
	public class ProxyAccessor
	{
		/// <summary>
		/// Link to crawling server
		/// </summary>
		private CrawlingServerProperties CrawlingServerPropertiesLink { get; set; }

		/// <summary>
		/// Link to sequencive rotation mutex, located into CrawlingServer class
		/// </summary>
		private Mutex ProxySequenciveRotationMutexLink { get; set; }

		/// <summary>
		/// Get randomization intial value
		/// TODO: rewrite to global randomizator (?)
		/// </summary>
		private int RandomizationInitialValue { get; set; }

		/// <summary>
		/// Creates new instance of ProxyAccessor
		/// </summary>
		/// <param name="CrawlingServerPropertiesLink">Link to crawling server properties set</param>
		public ProxyAccessor(CrawlingServerProperties CrawlingServerPropertiesLink, Mutex ProxySequenciveRotationMutexLink, int RandomizationInitialValue = 0)
		{
			this.CrawlingServerPropertiesLink = CrawlingServerPropertiesLink;
			this.ProxySequenciveRotationMutexLink = ProxySequenciveRotationMutexLink;
			if (RandomizationInitialValue == 0)
			{
				RandomizationInitialValue = Thread.VolatileRead(ref this.CrawlingServerPropertiesLink.ProxySequenciveRotationPointer) * 1024;
			}
			this.RandomizationInitialValue = RandomizationInitialValue;
		}

		/// <summary>
		/// Peeks proxy from CrawlingServer properties
		/// </summary>
		/// <returns></returns>
		public WebProxy PeekProxy()
		{
			if (CrawlingServerPropertiesLink.HTTPWebRequestProxiesList == null || CrawlingServerPropertiesLink.HTTPWebRequestProxiesList.Count == 0)
			{
				return null;
			}
			if (CrawlingServerPropertiesLink.HTTPWebRequestProxiesList.Count == 1)
			{
				return CrawlingServerPropertiesLink.HTTPWebRequestProxiesList[0].ProxyServerLink;
			}
			if (CrawlingServerPropertiesLink.ProxiesRotation == ProxiesRotationType.NoRotation)
			{
				return CrawlingServerPropertiesLink.HTTPWebRequestProxiesList[0].ProxyServerLink;
			}
			if (CrawlingServerPropertiesLink.ProxiesRotation == ProxiesRotationType.SequenciveRotation)
			{
				int num = CrawlingServerPropertiesLink.HTTPWebRequestProxiesList.Count - 1;
				int num2 = -1;
				bool flag = false;
				try
				{
					flag = ProxySequenciveRotationMutexLink.WaitOne();
					num2 = CrawlingServerPropertiesLink.ProxySequenciveRotationPointer;
					CrawlingServerPropertiesLink.ProxySequenciveRotationPointer++;
					if (CrawlingServerPropertiesLink.ProxySequenciveRotationPointer > num)
					{
						CrawlingServerPropertiesLink.ProxySequenciveRotationPointer = 0;
					}
				}
				finally
				{
					if (flag)
					{
						ProxySequenciveRotationMutexLink.ReleaseMutex();
					}
				}
				if (num2 == -1)
				{
					num2 = 0;
				}
				return CrawlingServerPropertiesLink.HTTPWebRequestProxiesList[num2].ProxyServerLink;
			}
			Random random = new Random(RandomizationInitialValue);
			int index = random.Next(0, CrawlingServerPropertiesLink.HTTPWebRequestProxiesList.Count - 1);
			return CrawlingServerPropertiesLink.HTTPWebRequestProxiesList[index].ProxyServerLink;
		}
	}
}