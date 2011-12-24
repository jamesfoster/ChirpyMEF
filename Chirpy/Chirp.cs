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

			if (engine == null)
				return null;

			try
			{
				var contents = FileHandler.GetContents(filename);

				var result = engine.Process(contents, filename);

				if(result == null)
					return null;

				SetOutputFileNames(result.Where(r => string.IsNullOrEmpty(r.FileName)), filename, engine);

				foreach (var engineResult in result)
				{
					var outputFilename = engineResult.FileName;

					outputFilename = FileHandler.GetAbsoluteFileName(outputFilename, filename);

					FileHandler.SaveFile(outputFilename, engineResult.Contents);
				}

				return result.Select(r => r.FileName);
			}
			catch (ChirpyException e)
			{
				// TaskList.Add(e);

				Console.WriteLine("{0}:{1} - {2}\n{3}", e.Message, e.FileName, e.LineNumber, e.Line);
			}
			catch (Exception e)
			{
				// TaskList.Add(filename, e.Message);

				Console.WriteLine("{0}", e.Message);
			}
			return null;
		}

		void SetOutputFileNames(IEnumerable<EngineResult> result, string filename, IEngine engine)
		{
			var engineContainer = engine as EngineContainer;
			var inputExtension = ExtensionResolver.GetExtensionFromCategory(engineContainer.Category);

			Debug.Assert(filename.EndsWith(inputExtension));

			foreach (var engineResult in result)
			{
				var outputCategory = engineResult.Category ?? engineContainer.OutputCategory;
				var outpuExtension = ExtensionResolver.GetExtensionFromCategory(outputCategory);

				engineResult.FileName = filename.Substring(0, filename.Length - inputExtension.Length) + outpuExtension;
			}
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