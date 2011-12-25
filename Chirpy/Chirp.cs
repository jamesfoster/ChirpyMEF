namespace Chirpy
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.Diagnostics;
	using System.Linq;
	using ChirpyInterface;
	using Imports;
	using Microsoft.VisualStudio.Shell;

	[Export]
	public class Chirp
	{
		protected CompositionContainer Container;

		[Import] public IEngineResolver EngineResolver { get; set; }
		[Import] public IExtensionResolver ExtensionResolver { get; set; }
		[Import] public ITaskList TaskList { get; set; }
		[Import] public IFileHandler FileHandler { get; set; }

		public Dictionary<string, List<string>> Dependancies { get; set; }

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
			Dependancies = new Dictionary<string, List<string>>();
		}

		public bool CheckDependancies(string filename)
		{
			var engine = EngineResolver.GetEngineByFilename(filename);

			if (engine == null)
				return false;

			var contents = FileHandler.GetContents(filename);

			SaveDependancies(filename, contents, engine);

			return true;
		}

		public IEnumerable<string> Run(string filename)
		{
			var engine = EngineResolver.GetEngineByFilename(filename);
			var result = new List<string>();

			if (engine == null)
				return null;

			try
			{
				var contents = FileHandler.GetContents(filename);

				var engineResults = engine.Process(contents, filename);

				if(engineResults == null)
					return null;

				foreach (var engineResult in engineResults)
				{
					if (engineResult.Exceptions != null && engineResult.Exceptions.Any())
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

					if (string.IsNullOrEmpty(outputFilename))
						outputFilename = GetOutputFileName(engineResult, filename, engine);

					outputFilename = FileHandler.GetAbsoluteFileName(outputFilename, filename);

					FileHandler.SaveFile(outputFilename, engineResult.Contents);

					result.Add(outputFilename);
				}

				return result;
			}
			catch (Exception e)
			{
				TaskList.Add(e.Message, filename, 0, 0, TaskErrorCategory.Error);

				Console.WriteLine("{0}", e.Message);
			}
			return null;
		}

		string GetOutputFileName(EngineResult result, string filename, IEngine engine)
		{
			var engineContainer = engine as EngineContainer;
			var inputExtension = ExtensionResolver.GetExtensionFromCategory(engineContainer.Category);

			Debug.Assert(filename.EndsWith(inputExtension));

			var outputCategory = result.Category ?? engineContainer.OutputCategory;
			var outpuExtension = ExtensionResolver.GetExtensionFromCategory(outputCategory);

			return filename.Substring(0, filename.Length - inputExtension.Length) + outpuExtension;
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
	}
}