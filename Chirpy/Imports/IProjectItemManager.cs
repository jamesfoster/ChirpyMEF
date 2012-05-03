namespace Chirpy.Imports
{
	using EnvDTE;

	public interface IProjectItemManager
	{
		void SolutionOpened();
		void ProjectAdded(Project project);
		void ProjectRemoved(Project project);
    void ItemAdded(ProjectItem projectItem);
    void ItemSaved(ProjectItem projectItem);
    void ItemClosed(ProjectItem projectItem);
		void ItemRenamed(ProjectItem projectItem, string oldname);
		void ItemRemoved(ProjectItem projectItem);
	}
}