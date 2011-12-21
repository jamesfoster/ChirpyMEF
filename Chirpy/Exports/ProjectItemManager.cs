namespace Chirpy.Exports
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Diagnostics;
	using System.Linq;
	using ChirpyInterface;
	using EnvDTE;
	using EnvDTE80;
	using Extensions;
	using Imports;

	[Export(typeof(IProjectItemManager))]
	[Export(typeof(IInternalProjectItemManager))]
	public class ProjectItemManager : IInternalProjectItemManager, IProjectItemManager
	{
		public Dictionary<string, List<string>> Dependancies { get; set; }

		[Import] public DTE2 App { get; set; }
		[Import] public IEngineResolver EngineResolver { get; set; }
		[Import] public IExtensionResolver ExtensionResolver { get; set; }
		[Import] public IFileHandler FileHandler { get; set; }

		public List<ProjectItem> HandledFiles { get; set; }

		public ProjectItemManager()
		{
			Dependancies = new Dictionary<string, List<string>>();
		}

		public void AddFile(string filename, string dependsUpon, string contents)
		{
			throw new System.NotImplementedException();
		}

		public void SolutionOpened()
		{
			HandledFiles = new List<ProjectItem>();

			foreach (var projectItem in AllProjectItems())
			{
				var filename = projectItem.FileName();
				var engine = EngineResolver.GetEngineByFilename(filename);

				if (engine != null)
				{
					HandledFiles.Add(projectItem);

					var contents = FileHandler.GetContents(filename);

					SaveDependancies(filename, contents, engine);
				}
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
			var engine = GetEngine(filename);

			if (engine == null)
				return;

			var contents = FileHandler.GetContents(filename);

			var output = engine.Process(contents, filename);

			var outputFileName = GetOutputFileName(filename, engine);

			FileHandler.SaveFile(outputFileName, output);

			projectItem.ProjectItems.AddFromFile(outputFileName);
		}

		public void ItemRenamed(ProjectItem projectItem)
		{

		}

		public void ItemRemoved(ProjectItem projectItem)
		{

		}

		string GetOutputFileName(string filename, IEngine engine)
		{
			var engineContainer = engine as EngineContainer;
			var inputExtension = ExtensionResolver.GetExtensionFromCategory(engineContainer.Category);
			var outpuExtension = ExtensionResolver.GetExtensionFromCategory(engineContainer.OutputCategory);

			Debug.Assert(filename.EndsWith(inputExtension));

			var outputFileName = filename.Substring(0, filename.Length - inputExtension.Length) + outpuExtension;
			return outputFileName;
		}

		void SaveDependancies(string filename, string contents, IEngine engine)
		{
			// remove dependancies for file
			foreach (var key in Dependancies.Keys.ToArray())
			{
				var files = Dependancies[key];
				if(files.Remove(filename) && files.Count == 0)
					Dependancies.Remove(key);
			}

			// add dependancies
			var dependancies = engine.GetDependancies(contents, filename);

			if(dependancies == null)
				return;

			foreach (var s in dependancies)
			{
				var dependancy = FileHandler.GetAbsoluteFileName(s, relativeTo: filename);

				if(Dependancies.ContainsKey(dependancy))
					Dependancies[dependancy].Add(filename);
				else
					Dependancies[dependancy] = new List<string> {filename};
			}
		}

		IEngine GetEngine(string filename)
		{
			return EngineResolver.GetEngineByFilename(filename);
		}
	}
}