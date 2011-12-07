namespace Chirpy
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Linq;
	using ChirpyInterface;

	[Export(typeof (IEngineResolver))]
	public class ChirpyEngineResolver : IEngineResolver
	{
		[ImportMany]
		protected IEnumerable<Lazy<IChirpyEngine, IChirpyEngineMetadata>> Engines { get; set; }

		public IEnumerable<Lazy<IChirpyEngine, IChirpyEngineMetadata>> GetEngines(string category, string filename)
		{
			return Engines
				.Where(e => e.Metadata.Category.Equals(category, StringComparison.InvariantCultureIgnoreCase))
				.Where(e => filename.EndsWith(e.Metadata.DefaultExtension, StringComparison.InvariantCultureIgnoreCase));
		}

		public IChirpyEngine GetEngine(string name)
		{
			var engines = Engines.Where(e => e.Metadata.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

			return new LazyCompoundEngine(engines);
		}
	}
}