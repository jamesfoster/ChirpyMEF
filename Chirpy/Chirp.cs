namespace Chirpy
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.Linq;
	using ChirpyInterface;

	public class Chirp
	{
		[Import]
		protected IEngineResolver EngineResolver { get; set; }

		protected IDictionary<string, string> ExtensionMap { get; set; }

		public Chirp()
		{
			ExtensionMap = new Dictionary<string, string>();

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

		public string Run(string category, string contents, string filename)
		{
			var engineGroups = EngineResolver.GetEngines(category, filename)
				.GroupBy(e => e.Metadata.Name);

			var result = contents;

			foreach (var engineGroup in engineGroups)
			{
				// try external engine first
				var engine = engineGroup.FirstOrDefault(e => !e.Metadata.Internal);

				if (engine != null)
				{
					result = RunEngine(result, filename, engine.Value);

					// an error occurred
					if (result == null)
						return null;

					continue;
				}

				// try internal engine if no external engine or error occurred
				engine = engineGroup.FirstOrDefault(e => e.Metadata.Internal);
				if (engine != null)
				{
					result = RunEngine(result, filename, engine.Value);

					// an error occurred
					if (result == null)
						return null;
				}
			}

			return result;
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
			}
			catch (Exception e)
			{
				// output generic error
			}

			return null;
		}
	}
}