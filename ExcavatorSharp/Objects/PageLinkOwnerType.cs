// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Objects.PageLinkOwnerType
/// <summary>
/// Probabled type of link owne - is it link to outer website or it is link to inner resource?
/// </summary>
namespace ExcavatorSharp.Objects
{
	public enum PageLinkOwnerType
	{
		/// <summary>
		/// Link not yet alanyzed
		/// </summary>
		NotAnalysedYet,
		/// <summary>
		/// Link to outer resource (other domain)
		/// </summary>
		OuterLink,
		/// <summary>
		/// Link to inner resource (current domain)
		/// </summary>
		InnerLink
	}
}