// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.ProxiesRotationType
using System.ComponentModel;

/// <summary>
/// Type of proxy rotation if many proxies used.
/// </summary>
namespace ExcavatorSharp.Objects
{
	public enum ProxiesRotationType
	{
		/// <summary>
		/// No proxies rotation - will used only first proxy server from list [ 0 -&gt; 0 -&gt; 0 -&gt; ...]
		/// </summary>
		[Description("No proxies rotation - will be used only first proxy server from the list")]
		NoRotation,
		/// <summary>
		/// Rotate proxies sequencive - from first proxy to last proxy, and loop back again. [0 -&gt; 1 -&gt; 2 -&gt; 0 -&gt; 1 -&gt; 2 -&gt; ...]
		/// </summary>
		[Description("Rotate proxies sequencive - from the first proxy to the last proxy, and loop back again")]
		SequenciveRotation,
		/// <summary>
		/// Rotate proxies with random. [ 0 -&gt; 1 -&gt; 0 -&gt; 2 -&gt; 1 -&gt; 1 -&gt; 2 -&gt; 0 -&gt; ...]
		/// </summary>
		[Description("Rotate proxies with random")]
		RandomRotation
	}
}