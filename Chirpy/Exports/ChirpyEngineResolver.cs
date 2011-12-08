namespace Chirpy.Exports
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
		public IEnumerable<Lazy<IChirpyEngine, IChirpyEngineMetadata>> Engines { get; set; }

		protected Dictionary<string, LazyMefEngine> EngineCache { get; set; } 

		public IChirpyEngine GetEngine(string category, string subCategory)
		{
			var engine = LoadFromCache(category, subCategory);

			if(engine != null)
				return engine;

			var engines = Engines
				.Where(e => e.Metadata.Category.Equals(category, StringComparison.InvariantCultureIgnoreCase))
				.Where(e => e.Metadata.SubCategory.Equals(subCategory, StringComparison.InvariantCultureIgnoreCase))
				.ToList();

			if (!engines.Any())
				return null;

			if (!CheckEngines(engines))
			{
				var engineNames = engines.Select(e => string.Format("{0} ({1})",
				                                                    e.Metadata.Name,
				                                                    e.Metadata.Internal ? "Internal" : "External"));
				var message = string.Format("There are multiple engines defined for the category {0}:{1}\n{2}",
				                            category,
				                            subCategory,
				                            string.Join(", ", engineNames));
				throw new InvalidOperationException(message);
			}

			return SaveCache(category,subCategory,engines);
		}

		public IChirpyEngine GetEngineByName(string name)
		{
			var engine = LoadFromCache(name);

			if(engine != null)
				return engine;

			var engines = Engines
				.Where(e => e.Metadata.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				.ToList();

			if (!engines.Any())
				return null;

			if (!CheckEngines(engines))
			{
				var engineNames = engines.Select(e => string.Format("{0} ({1})",
				                                                    e.Metadata.Name,
				                                                    e.Metadata.Internal ? "Internal" : "External"));
				var message = string.Format("There are multiple engines defined for the name '{0}'\n{1}",
				                            name,
				                            string.Join(", ", engineNames));
				throw new InvalidOperationException(message);
			}

			return SaveCache(name, engines);
		}

		public IChirpyEngine GetEngineForFile(string filename)
		{
			throw new NotImplementedException();
		}

		LazyMefEngine LoadFromCache(string category, string subCategory)
		{
			var key = GetCategoryKey(category, subCategory);

			return LoadFromCache(key);
		}

		LazyMefEngine LoadFromCache(string key)
		{
			if (EngineCache == null || !EngineCache.ContainsKey(key))
				return null;

			return EngineCache[key];
		}

		LazyMefEngine SaveCache(string category, string subCategory,IEnumerable<Lazy<IChirpyEngine, IChirpyEngineMetadata>> engines)
		{
			var key = GetCategoryKey(category, subCategory);

			return SaveCache(key, engines);
		}

		LazyMefEngine SaveCache(string key, IEnumerable<Lazy<IChirpyEngine, IChirpyEngineMetadata>> engines)
		{
			if (EngineCache == null)
				EngineCache = new Dictionary<string, LazyMefEngine>();

			return EngineCache[key] = new LazyMefEngine(engines);
		}

		static string GetCategoryKey(string category, string subCategory)
		{
			var key = string.Format(">>{0}:{1}<<", category, subCategory);
			return key;
		}

		bool CheckEngines(IEnumerable<Lazy<IChirpyEngine, IChirpyEngineMetadata>> engines)
		{
			var names = engines.Select(e => e.Metadata.Name.ToLowerInvariant()).Distinct().ToList();

			if (names.Count > 1)
				return false;

			var internalEngines = engines.Where(e => e.Metadata.Internal).ToList();

			if (internalEngines.Count > 1)
				return false;

			var externalEngines = engines.Where(e => !e.Metadata.Internal).ToList();

			if (externalEngines.Count > 1)
				return false;

			return true;
		}
	}
}