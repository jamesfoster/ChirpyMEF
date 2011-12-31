namespace Chirpy
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.Diagnostics;
	using System.Linq;
	using ChirpyInterface;
	using EnvDTE;
	using Extensions;
	using Imports;

	[Export]
	public class Chirp
	{
		protected CompositionContainer Container;

		[Import] public IEngineResolver EngineResolver { get; set; }
		[Import] public IExtensionResolver ExtensionResolver { get; set; }
		[Import] public ITaskList TaskList { get; set; }
		[Import] public IFileHandler FileHandler { get; set; }

		public Dictionary<string, List<ProjectItem>> Dependancies { get; set; }

		public Chirp(IEngineResolver engineResolver, ITaskList taskList, IFileHandler fileHandler, IExtensionResolver extensionResolver)
			: this()
		{
			EngineResolver = engineResolver;
			TaskList = taskList;
			FileHandler = fileHandler;
			ExtensionResolver = extensionResolver;
		}

		Chirp()
		{
			Dependancies = new Dictionary<string, List<ProjectItem>>();
		}

		public bool CheckDependancies(ProjectItem projectItem)
		{
			var filename = projectItem.FileName();

			var engine = EngineResolver.GetEngineByFilename(filename);

			if (engine == null)
				return false;

			var contents = FileHandler.GetContents(filename);

			SaveDependancies(projectItem, filename, contents, engine);

			return true;
		}

		public void RemoveDependancies(ProjectItem projectItem)
		{
			var filename = projectItem.FileName();

			if (Dependancies.ContainsKey(filename))
				Dependancies.Remove(filename);

			RemoveDependanciesForFile(projectItem);
		}

		public IEnumerable<FileAssociation> Run(ProjectItem projectItem)
		{
			var filename = projectItem.FileName();
			var engine = EngineResolver.GetEngineByFilename(filename);
			var result = new List<FileAssociation>();

			if (engine == null)
				return null;

			ProcessEngine(projectItem, filename, engine, result);

			RunDependancies(filename);

			return result;
		}

		public IEnumerable<FileAssociation> RunDependancies(string filename)
		{
			if(!Dependancies.ContainsKey(filename))
				return null;

			var dependancies = Dependancies[filename];

			var result = new List<FileAssociation>();

			foreach (var dependancy in dependancies)
			{
				var fileAssociations = Run(dependancy);

				if (fileAssociations != null)
					result.AddRange(fileAssociations);
			}

			return result;
		}

		void ProcessEngine(ProjectItem projectItem, string filename, IEngine engine, ICollection<FileAssociation> result)
		{
			try
			{
				var contents = FileHandler.GetContents(filename);

				var engineResults = engine.Process(contents, filename);

				if (engineResults == null)
					return;

				foreach (var engineResult in engineResults)
				{
					if (engineResult.Exceptions.Any())
					{
						foreach (var exception in engineResult.Exceptions)
						{
							TaskList.Add(exception);
						}
						continue;
					}

					if (engineResult.Contents == null)
						continue;

					var outputFilename = engineResult.FileName;

					if (!string.IsNullOrEmpty(engineResult.Extension))
						outputFilename = GetOutputFileName(engineResult, filename, engine);

					outputFilename = FileHandler.GetAbsoluteFileName(outputFilename, filename);

					FileHandler.SaveFile(outputFilename, engineResult.Contents);

					result.Add(new FileAssociation(outputFilename, projectItem));
				}
			}
			catch (Exception e)
			{
				TaskList.Add(e.Message, filename, ErrorCategory.Error);

				Console.WriteLine("{0}", e.Message);
			}
		}

		string GetOutputFileName(EngineResult result, string filename, IEngine engine)
		{
			string baseFileName;
			if(!string.IsNullOrEmpty(result.FileName))
			{
				baseFileName = FileHandler.GetBaseFileName(result.FileName);
			}
			else
			{
				var engineContainer = engine as EngineContainer;
				var inputExtension = ExtensionResolver.GetExtensionFromCategory(engineContainer.Category);

				Debug.Assert(filename.EndsWith(inputExtension));

				baseFileName = filename.Substring(0, filename.Length - inputExtension.Length);
			}

			return string.Format("{0}.{1}", baseFileName, result.Extension);
		}

		void SaveDependancies(ProjectItem projectItem, string filename, string contents, IEngine engine)
		{
			RemoveDependanciesForFile(projectItem);

			var dependancies = engine.GetDependancies(contents, filename);

			if(dependancies == null)
				return;

			AddDependanciesForFile(projectItem, filename, dependancies);
		}

		void AddDependanciesForFile(ProjectItem projectItem, string filename, IEnumerable<string> dependancies)
		{
			foreach (var s in dependancies)
			{
				var dependancy = FileHandler.GetAbsoluteFileName(s, relativeTo: filename);

				if (Dependancies.ContainsKey(dependancy))
					Dependancies[dependancy].Add(projectItem);
				else
					Dependancies[dependancy] = new List<ProjectItem> {projectItem};
			}
		}

		void RemoveDependanciesForFile(ProjectItem projectItem)
		{
			foreach (var key in Dependancies.Keys.ToArray())
			{
				var files = Dependancies[key];
				if (files.Remove(projectItem) && files.Count == 0)
					Dependancies.Remove(key);
			}
		}
	}
}