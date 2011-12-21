namespace Chirpy.Imports
{
	using EnvDTE;

	public interface IInternalProjectItemManager
	{
		void SolutionOpened();
		void ProjectAdded(Project project);
		void ProjectRemoved(Project project);
		void ItemAdded(ProjectItem projectItem);
		void ItemSaved(ProjectItem projectItem);
		void ItemRenamed(ProjectItem projectItem);
		void ItemRemoved(ProjectItem projectItem);
	}
}