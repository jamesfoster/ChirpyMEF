namespace Chirpy
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ChirpyInterface;

	public class LazyMefEngine : IChirpyEngine
	{
		public IEnumerable<Lazy<IChirpyEngine, IChirpyEngineMetadata>> Engines { get; set; }

		public LazyMefEngine(IEnumerable<Lazy<IChirpyEngine, IChirpyEngineMetadata>> engines)
		{
			Engines = engines;
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

		IChirpyEngine GetEngine()
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