// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Exporter.MySQLColumnPointer
namespace ExcavatorSharp.Exporter
{
    /// <summary>
    /// Class for storing pointer to some MySQLColumn
    /// </summary>
    internal class MySQLColumnPointer
    {
    	/// <summary>
    	/// Hash of some pattern who's parent of PatternItemElement
    	/// </summary>
    	public int PatternHash { get; set; }

    	/// <summary>
    	/// Name of some pattern item element
    	/// </summary>
    	public string PatternItemElementName { get; set; }

    	/// <summary>
    	/// Name of some column
    	/// </summary>
    	public string ColumnName { get; set; }

    	/// <summary>
    	/// Type of MySQLColumnPointer
    	/// </summary>
    	public MySQLColumnPointerType PointerType { get; set; }

    	/// <summary>
    	/// Creates new instance of MySQLColumnPointer
    	/// </summary>
    	/// <param name="PatternHash">Hash of parent pattern</param>
    	/// <param name="PatternItemElementName">Some child item name</param>
    	/// <param name="ColumnName">Column assembled name</param>
    	/// <param name="PointerType">Type of pointer data</param>
    	public MySQLColumnPointer(int PatternHash, string PatternItemElementName, string ColumnName, MySQLColumnPointerType PointerType)
    	{
    		this.PatternHash = PatternHash;
    		this.PatternItemElementName = PatternItemElementName;
    		this.ColumnName = ColumnName;
    		this.PointerType = PointerType;
    	}
    }
}
