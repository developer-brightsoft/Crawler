// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Common.IOCommon
using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;

namespace ExcavatorSharp.Common
{
    /// <summary>
    /// Common IO operations
    /// </summary>
    public class IOCommon
    {
    	/// <summary>
    	/// Try to check app permissions and install application folder with files
    	/// </summary>
    	public static void CheckAppIOPermissions()
    	{
    		try
    		{
    			GetDataExcavatorCommonIOPath();
    		}
    		catch (Exception innerException)
    		{
    			throw new IOException("Cannot initialize ProgramData/PIC/DataExcavator folder. You do not have enough rights to perform this operation. Try to restart the program under an administrator account.", innerException);
    		}
    	}

    	/// <summary>
    	/// Gets specified directory size in bytes
    	/// </summary>
    	/// <param name="TargetDirectory"></param>
    	/// <param name="includeSubDir"></param>
    	/// <returns></returns>
    	public long DirectorySize(DirectoryInfo TargetDirectory, bool includeSubDir)
    	{
    		long num = TargetDirectory.EnumerateFiles().Sum((FileInfo file) => file.Length);
    		if (includeSubDir)
    		{
    			num += TargetDirectory.EnumerateDirectories().Sum((DirectoryInfo dir) => DirectorySize(dir, includeSubDir: true));
    		}
    		return num;
    	}

    	/// <summary>
    	/// Returns absolute path to DataExcavator common-files directory
    	/// </summary>
    	/// <returns></returns>
    	public static string GetDataExcavatorCommonIOPath()
    	{
    		string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
    		string text = $"{folderPath}/{DEConfig.VendorDataFolderName}";
    		string text2 = $"{text}/{DEConfig.ApplicationDataFolderName}";
    		if (!Directory.Exists(text))
    		{
    			DirectoryInfo folderFullAccessPermissions = Directory.CreateDirectory(text);
    			SetFolderFullAccessPermissions(folderFullAccessPermissions);
    		}
    		if (!Directory.Exists(text2))
    		{
    			DirectoryInfo folderFullAccessPermissions2 = Directory.CreateDirectory(text2);
    			SetFolderFullAccessPermissions(folderFullAccessPermissions2);
    		}
    		return text2;
    	}

    	/// <summary>
    	/// Set folder permissions to all users -&gt; Write, ReadAndExecute, Modify, ContainerInherit, ObjectInherit, InheritOnly, Allow
    	/// Access control is based on the next article: https://www.codeproject.com/Tips/61987/Allow-write-modify-access-to-CommonApplicationData
    	/// </summary>
    	public static void SetFolderFullAccessPermissions(DirectoryInfo directoryInfo)
    	{
    		SecurityIdentifier identity = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            DirectorySecurity accessControl = directoryInfo.GetAccessControl();
    		FileSystemAccessRule rule = new FileSystemAccessRule(identity, FileSystemRights.Modify, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly, AccessControlType.Allow);
    		accessControl.ModifyAccessRule(AccessControlModification.Add, rule, out var _);
    		directoryInfo.SetAccessControl(accessControl);
    	}
    }
}
