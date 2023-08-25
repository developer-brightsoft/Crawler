// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.DataExcavatorStartStopTaskArg
using System;
using System.Threading;

/// <summary>
/// Argument for starting DataExcavatorTask
/// </summary>
namespace ExcavatorSharp.Objects
{
	internal class DataExcavatorStartStopTaskArg
	{
		/// <summary>
		/// Link to a parent starter thread
		/// </summary>
		public Thread StarterLink { get; set; }

		/// <summary>
		/// Callback invoked after DataExcavatorTask started
		/// </summary>
		public Action DataExcavatorStartedCallback { get; set; }
	}
}