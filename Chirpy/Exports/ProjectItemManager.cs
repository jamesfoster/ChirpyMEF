namespace Chirpy.Exports
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Diagnostics;
	using System.Windows.Forms;
	using ChirpyInterface;
	using EnvDTE;
	using Extensions;
	using Imports;

	[Export(typeof(IProjectItemManager))]
	[Export(typeof(IInternalProjectItemManager))]
	public class ProjectItemManager : IInternalProjectItemManager, IProjectItemManager
	{
		public Dictionary<string, List<string>> Dependancies { get; set; }

		[Import] public IEngineResolver EngineResolver { get; set; }
		[Import] public IExtensionResolver ExtensionResolver { get; set; }
		[Import] public IFileHandler FileHandler { get; set; }

		public ProjectItemManager()
		{
			Dependancies = new Dictionary<string, List<string>>();
		}

		public void AddFile(string filename, string dependsUpon, string contents)
		{
			throw new System.NotImplementedException();
		}

		public void ItemSaved(ProjectItem projectItem)
		{
			var filename = projectItem.FileName();
			var engine = GetEngine(filename);

			if (engine == null)
				return;

			var contents = FileHandler.GetContents(filename);

			SaveDependancies(filename, contents, engine);

			var output = engine.Process(contents, filename);

			var outputFileName = GetOutputFileName(filename, engine);

			FileHandler.SaveFile(outputFileName, output);

			projectItem.ProjectItems.AddFromFile(outputFileName);
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
			var dependancies = engine.GetDependancies(contents, filename);

			Dependancies[filename] = dependancies;
		}

		IEngine GetEngine(string filename)
		{
			return EngineResolver.GetEngineByFilename(filename);
		}
	}
}