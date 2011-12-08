namespace Chirpy
{
	using System;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.Linq;
	using ChirpyInterface;

	public class Chirp
	{
		[Import]
		protected IEngineResolver EngineResolver { get; set; }

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

		public string Run(string category, string subCategory, string contents, string filename)
		{
			var engine = EngineResolver.GetEngine(category, subCategory);

			if (engine == null)
				return null;

			return RunEngine(contents, filename, engine);
		}

		static string RunEngine(string contents, string filename, IChirpyEngine engine)
		{
			try
			{
				return engine.Process(contents, filename);
			}
			catch (ChirpyException e)
			{
				// output well formatted TaskList item
				Console.WriteLine("{0}:{1} - {2}\n{3}", e.Message, e.FileName, e.LineNumber, e.Line);
			}
			catch (Exception e)
			{
				// output generic error
				Console.WriteLine("{0}", e.Message);
			}

			return null;
		}
	}
}