// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Common.RegistryOperations
using System;
using Microsoft.Win32;

/// <summary>
/// Class for common registry operations
/// </summary>
namespace ExcavatorSharp.Common
{
	internal class RegistryOperations
	{
		/// <summary>
		/// Path to application registry key
		/// </summary>
		public const string RegistryKeyPath = "SOFTWARE\\deapp";

		/// <summary>
		/// Returns application registry key or null
		/// </summary>
		/// <returns></returns>
		internal static RegistryKey GetApplicationRegistryKeyNode()
		{
			try
			{
				return Registry.CurrentUser.OpenSubKey("SOFTWARE\\deapp", writable: true);
			}
			catch (Exception)
			{
			}
			return null;
		}

		/// <summary>
		/// Creates application registry key
		/// </summary>
		/// <returns></returns>
		internal static RegistryKey CreateApplicationRegistryKey()
		{
			try
			{
				return Registry.CurrentUser.CreateSubKey("SOFTWARE\\deapp");
			}
			catch (Exception)
			{
			}
			return null;
		}

		/// <summary>
		/// Closes some registry key
		/// </summary>
		internal static void CloseRegistryKey(RegistryKey keylink)
		{
			try
			{
				keylink.Close();
			}
			catch (Exception)
			{
			}
		}
	}
}