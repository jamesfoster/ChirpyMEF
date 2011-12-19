namespace Chirpy.Imports
{
	using EnvDTE;

	public interface IInternalProjectItemManager
	{
		void ItemSaved(ProjectItem projectItem);
	}
}