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
	using Logging;

	[Export]
	public class Chirp
	{
		[Import] public IInternalEngineResolver EngineResolver { get; set; }
		[Import] public IExtensionResolver ExtensionResolver { get; set; }
		[Import] public ITaskList TaskList { get; set; }
		[Import] public IFileHandler FileHandler { get; set; }
		[Import] public ILogger Logger { get; set; }

		public Dictionary<string, List<ProjectItem>> Dependancies { get; set; }

		public Chirp()
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

			if (engine != null)
				result.AddRange(ProcessEngine(projectItem, filename, engine));

			var associations = RunDependancies(filename);

			if (associations != null)
				result.AddRange(associations);

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

		IEnumerable<FileAssociation> ProcessEngine(ProjectItem projectItem, string filename, EngineContainer engine)
		{
			List<EngineResult> engineResults = null;
			try
			{
				var contents = FileHandler.GetContents(filename);

				engineResults = engine.Process(contents, filename);
			}
			catch (Exception e)
			{
				TaskList.Add(e.Message, filename, ErrorCategory.Error);

				Console.WriteLine("{0}", e.Message);
			}

			if (engineResults == null)
				yield break;

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

				yield return new FileAssociation(outputFilename, projectItem);

				Logger.Log(string.Format("{0} > {1}", engine.Name, outputFilename));
			}
		}

		string GetOutputFileName(EngineResult result, string filename, EngineContainer engine)
		{
			var baseFileName = filename;

			if (!string.IsNullOrEmpty(result.FileName))
				baseFileName = result.FileName;

			var inputExtension = ExtensionResolver.GetExtensionFromCategory(engine.Category);

			if (baseFileName.EndsWith(inputExtension))
				baseFileName = filename.Substring(0, filename.Length - inputExtension.Length);
			else
				baseFileName = FileHandler.GetBaseFileName(baseFileName);

			return string.Format("{0}.{1}", baseFileName, result.Extension);
		}

		void SaveDependancies(ProjectItem projectItem, string filename, string contents, EngineContainer engine)
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