// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Common.DataExcavatorInitializer
namespace ExcavatorSharp.Common
{
    /// <summary>
    /// Data Excavator initialization class
    /// </summary>
    internal class DataExcavatorInitializer
    {
    	/// <summary>
    	/// Initializes DataExcavator core
    	/// </summary>
    	internal static void Initialize()
    	{
    		ECHttpServingCommon.ConfigureServicePointManager();
    	}
    }
}
