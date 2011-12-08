namespace Chirpy
{
	using System;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using ChirpyInterface;

	public class Chirp
	{
		[Import] protected IEngineResolver EngineResolver { get; set; }
		[Import] protected ITaskList TaskList { get; set; }
		[Import] protected IProjectItemManager ProjectItemManager { get; set; }
		[Import] protected IFileHandler FileHandler { get; set; }

		public Chirp(IEngineResolver engineResolver)
		{
			EngineResolver = engineResolver;
		}

		internal Chirp()
		{
			ComposeEngines();
		}

		void ComposeEngines()
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