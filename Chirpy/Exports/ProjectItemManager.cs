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

		public void SolutionOpened()
		{
			foreach (var projectItem in AllProjectItems())
			{
				Chirp.CheckDependencies(projectItem);
			}
		}

		IEnumerable<ProjectItem> AllProjectItems(Project project)
		{
			return AllProjectItemsRecursive(project.ProjectItems);
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
			foreach (var projectItem in AllProjectItems(project))
			{
				Chirp.CheckDependencies(projectItem);
			}
		}

		public void ProjectRemoved(Project project)
		{
			foreach (var projectItem in AllProjectItems(project))
			{
				Chirp.RemoveDependencies(projectItem);
			}
		}

		public void ItemAdded(ProjectItem projectItem)
		{
			Chirp.CheckDependencies(projectItem);

			ItemSaved(projectItem);
		}

		public void ItemSaved(ProjectItem projectItem)
		{
			var fileAssociations = Chirp.Run(projectItem);

			ProcessFileAssociations(fileAssociations);
		}

		public void ItemRenamed(ProjectItem projectItem, string oldname)
		{
			Chirp.RunDependencies(oldname);

			Chirp.RemoveDependencies(projectItem);
			Chirp.CheckDependencies(projectItem);
		}

		public void ItemRemoved(ProjectItem projectItem)
		{
			Chirp.Run(projectItem);

			Chirp.RemoveDependencies(projectItem);
		}

		public void ItemClosed(ProjectItem projectItem)
		{
      Chirp.TaskList.Remove(projectItem.FileName());
		}

		static void ProcessFileAssociations(IEnumerable<FileAssociation> fileAssociations)
		{
			if (fileAssociations == null)
				return;

			foreach (var fileAssociation in fileAssociations)
			{
				fileAssociation.Parent.ProjectItems.AddFromFile(fileAssociation.FileName);
			}
		}
	}
}