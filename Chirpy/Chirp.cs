namespace Chirpy
{
	using System;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.IO;
	using System.Reflection;
	using ChirpyInterface;

	[Export]
	public class Chirp : IDisposable
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

		internal static Chirp CreateWithPlugins()
		{
			var chirp = new Chirp();

			ComposeWithPlugins(chirp);

			return chirp;
		}

		internal static Chirp CreateWithoutPlugins()
		{
			var chirp = new Chirp();

			ComposeWithoutPlugins(chirp);

			return chirp;
		}

		static void ComposeWithPlugins(Chirp chirp)
		{
			var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var pluginDirectory = Path.Combine(assemblyDirectory, "Plugins");

			if(!Directory.Exists(pluginDirectory))
				Directory.CreateDirectory(pluginDirectory);

			var assemblyCatalog = new AssemblyCatalog(typeof (Chirp).Assembly);
			var directoryCatalog = new DirectoryCatalog(pluginDirectory);
			var catalog = new AggregateCatalog(assemblyCatalog, directoryCatalog);

			chirp.Container = new CompositionContainer(catalog);

			chirp.Container.ComposeParts(chirp);
		}

		static void ComposeWithoutPlugins(Chirp chirp)
		{
			var catalog = new AssemblyCatalog(typeof (Chirp).Assembly);

			chirp.Container = new CompositionContainer(catalog);

			chirp.Container.ComposeParts(chirp);
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

		protected bool IsDisposed { get; set; }

		public void Dispose()
		{
			lock(this)
			{
				if (!IsDisposed)
				{
					IsDisposed = true;
					if(Container != null)
						Container.Dispose();
				}
			}
		}
	}
}