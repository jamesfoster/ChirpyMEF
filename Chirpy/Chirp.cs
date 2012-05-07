namespace Chirpy
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
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

		public Dictionary<string, List<ProjectItem>> Dependencies { get; set; }

		public Chirp()
		{
			Dependencies = new Dictionary<string, List<ProjectItem>>();
		}

		public bool CheckDependencies(ProjectItem projectItem)
		{
			var filename = projectItem.FileName();

			var engine = EngineResolver.GetEngineByFilename(filename);

			if (engine == null)
				return false;

			var contents = FileHandler.GetContents(filename);

			SaveDependencies(projectItem, filename, contents, engine);

			return true;
		}

		public void RemoveDependencies(ProjectItem projectItem)
		{
			var filename = projectItem.FileName();

			if (Dependencies.ContainsKey(filename))
				Dependencies.Remove(filename);

			RemoveDependenciesForFile(projectItem);
		}

		public IEnumerable<FileAssociation> Run(ProjectItem projectItem)
		{
			var filename = projectItem.FileName();
			var engine = EngineResolver.GetEngineByFilename(filename);
			var result = new List<FileAssociation>();

			TaskList.Remove(filename);

			if (engine != null)
				result.AddRange(ProcessEngine(projectItem, filename, engine));

			var associations = RunDependencies(filename);

			if (associations != null)
				result.AddRange(associations);

			return result;
		}

		public IEnumerable<FileAssociation> RunDependencies(string filename)
		{
			if (!Dependencies.ContainsKey(filename))
				return null;

			var dependencies = Dependencies[filename];

			var result = new List<FileAssociation>();

			foreach (var dependency in dependencies)
			{
				var fileAssociations = Run(dependency);

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
					var error = false;
					foreach (var exception in engineResult.Exceptions)
					{
						if (string.IsNullOrEmpty(exception.FileName))
						{
							exception.FileName = filename;
						}
						error = error || exception.Category == ErrorCategory.Error;
						TaskList.Add(exception);
					}
					if (error)
					{
						continue;
					}
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

		void SaveDependencies(ProjectItem projectItem, string filename, string contents, EngineContainer engine)
		{
			RemoveDependenciesForFile(projectItem);

			var dependencies = engine.GetDependencies(contents, filename);

			if (dependencies == null)
				return;

			AddDependenciesForFile(projectItem, filename, dependencies);
		}

		void AddDependenciesForFile(ProjectItem projectItem, string filename, IEnumerable<string> dependencies)
		{
			foreach (var s in dependencies)
			{
				var dependency = FileHandler.GetAbsoluteFileName(s, relativeTo: filename);

				if (Dependencies.ContainsKey(dependency))
					Dependencies[dependency].Add(projectItem);
				else
					Dependencies[dependency] = new List<ProjectItem> {projectItem};
			}
		}

		void RemoveDependenciesForFile(ProjectItem projectItem)
		{
			foreach (var key in Dependencies.Keys.ToArray())
			{
				var files = Dependencies[key];
				if (files.Remove(projectItem) && files.Count == 0)
					Dependencies.Remove(key);
			}
		}
	}
}