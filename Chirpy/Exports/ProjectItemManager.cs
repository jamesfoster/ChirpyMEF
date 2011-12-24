namespace Chirpy.Exports
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Linq;
	using EnvDTE;
	using EnvDTE80;
	using Extensions;
	using Imports;

	[Export(typeof(IProjectItemManager))]
	public class ProjectItemManager : IProjectItemManager
	{
		[Import] public DTE2 App { get; set; }
		[Import] public Chirp Chirp { get; set; }

		public List<ProjectItem> HandledFiles { get; set; }

		public void SolutionOpened()
		{
			HandledFiles = new List<ProjectItem>();

			foreach (var projectItem in AllProjectItems())
			{
				var filename = projectItem.FileName();

				if (Chirp.CheckDependancies(filename))
					HandledFiles.Add(projectItem);
			}
		}

		IEnumerable<ProjectItem> AllProjectItems()
		{
			var projects = App.Solution.Projects;

			return projects
				.Cast<Project>()
				.SelectMany(project => AllProjectItemsRecursive(project.ProjectItems));
		}

		static IEnumerable<ProjectItem> AllProjectItemsRecursive(ProjectItems projectItems)
		{
			if (projectItems == null)
				yield break;

			foreach (ProjectItem projectItem in projectItems)
			{
				if (projectItem.IsFolder() && projectItem.ProjectItems != null)
				{
					foreach (var folderProjectItem in AllProjectItemsRecursive(projectItem.ProjectItems))
					{
						yield return folderProjectItem;
					}
				}
				else if (projectItem.IsSolutionFolder())
				{
					foreach (var solutionProjectItem in AllProjectItemsRecursive(projectItem.SubProject.ProjectItems))
					{
						yield return solutionProjectItem;
					}
				}
				else
				{
					yield return projectItem;
				}
			}
		}

		public void ProjectAdded(Project project)
		{

		}

		public void ProjectRemoved(Project project)
		{

		}

		public void ItemAdded(ProjectItem projectItem)
		{
			ItemSaved(projectItem);
		}

		public void ItemSaved(ProjectItem projectItem)
		{
			var filename = projectItem.FileName();

			var files = Chirp.Run(filename);

			foreach (var file in files)
			{
				projectItem.ProjectItems.AddFromFile(file);
			}
		}

		public void ItemRenamed(ProjectItem projectItem)
		{

		}

		public void ItemRemoved(ProjectItem projectItem)
		{

		}
	}
}