namespace Chirpy
{
	using System;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using ChirpyInterface;

	[Export]
	public class Chirp
	{
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

		internal static Chirp Create()
		{
			var chirp = new Chirp();

			chirp.Compose();

			return chirp;
		}

		void Compose()
		{
			var assemblyCatalog = new AssemblyCatalog(typeof (Chirp).Assembly);
			var directoryCatalog = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory);
			var catalog = new AggregateCatalog(assemblyCatalog, directoryCatalog);

			var container = new CompositionContainer(catalog);

			container.ComposeParts(this);
		}

		public void Run(string category, string subCategory, string filename)
		{
			var engine = EngineResolver.GetEngine(category, subCategory);

			if (engine == null)
				return;

			try
			{
				var contents = FileHandler.GetContents(filename);

				var result = engine.Process(contents, filename);

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
		}
	}
}