namespace Chirpy
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ChirpyInterface;

	public class EngineContainer : IEngine
	{
		public IEnumerable<Lazy<IEngine, IEngineMetadata>> Engines { get; set; }

		public string Name { get; private set; }
		public string Category { get; private set; }
		public string OutputCategory { get; private set; }
		public bool[] Internal { get; private set; }
		public bool Minifier { get; private set; }

		public EngineContainer(IEnumerable<Lazy<IEngine, IEngineMetadata>> engines)
		{
			Engines = engines;

			if(!engines.Any())
				return;

			var metadata = engines.First().Metadata;

			Name = metadata.Name;
			Category = metadata.Category;
			OutputCategory = metadata.OutputCategory;
			Minifier = metadata.Minifier;

			Internal = engines.Select(e => e.Metadata.Internal).ToArray();
		}

		public List<string> GetDependancies(string contents, string filename)
		{
			var engine = GetEngine();

			if (engine != null)
				return engine.GetDependancies(contents, filename);

			return null;
		}

		public string Process(string contents, string filename)
		{
			var engine = GetEngine();

			if (engine != null)
				return engine.Process(contents, filename);

			return null;
		}

		IEngine GetEngine()
		{
			// try external first
			var engine = Engines.FirstOrDefault(e => !e.Metadata.Internal);

			if (engine == null)
				engine = Engines.FirstOrDefault(e => e.Metadata.Internal);

			if (engine != null)
				return engine.Value;

			return null;
		}
	}
}