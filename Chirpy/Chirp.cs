namespace Chirpy
{
	using System;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.IO;
	using ChirpyInterface;

	[Export]
	public class Chirp
	{
		protected CompositionContainer Container;

		[Import] public IEngineResolver EngineResolver { get; set; }
		[Import] public ITaskList TaskList { get; set; }
		[Import] public IProjectItemManager ProjectItemManager { get; set; }
		[Import] public IFileHandler FileHandler { get; set; }

		public Chirp(IEngineResolver engineResolver, ITaskList taskList, IProjectItemManager projectItemManager, IFileHandler fileHandler)
		{
			EngineResolver = engineResolver;
			TaskList = taskList;
			ProjectItemManager = projectItemManager;
			FileHandler = fileHandler;
		}

		Chirp()
		{
		}

		public string Run(string filename)
		{
			var engine = EngineResolver.GetEngineByFilename(filename);

			if (engine == null)
				return null;

			try
			{
				var contents = FileHandler.GetContents(filename);

				var result = engine.Process(contents, filename);

				return result;
				// ProjectItemManager.AddFile(newFilename, filename);
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
	}
}