// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Licensing.CheckLicenseInBackgroundArgs
using System.Threading;
using ExcavatorSharp.Excavator;

/// <summary>
/// Class for storing arguments for License background validation process
/// </summary>
namespace ExcavatorSharp.Licensing
{
	internal class CheckLicenseInBackgroundArgs
	{
		/// <summary>
		/// Link to checking thread
		/// </summary>
		public Thread ThreadLink { get; set; }

		/// <summary>
		/// Link to task factory
		/// </summary>
		public DataExcavatorTasksFactory TaskFactoryLink { get; set; }
	}
}
