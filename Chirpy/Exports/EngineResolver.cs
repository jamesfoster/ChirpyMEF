namespace Chirpy.Exports
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Linq;
	using ChirpyInterface;
	using Imports;

	[Export(typeof (IEngineResolver))]
	public class EngineResolver : IEngineResolver
	{
		[ImportMany]
		public IEnumerable<Lazy<IEngine, IEngineMetadata>> Engines { get; set; }

		[Import]
		public IExtensionResolver ExtensionResolver { get; set; }

		protected Dictionary<string, EngineContainer> EngineCache { get; set; } 

		public IEngine GetEngine(string category)
		{
			var engine = LoadFromCacheByCategory(category);

			if(engine != null)
				return engine;

			var engines = Engines
				.Where(e => e.Metadata.Category.Equals(category, StringComparison.InvariantCultureIgnoreCase))
				.ToList();

			if (!engines.Any())
				return null;

			if (!CheckEngines(engines))
			{
				var engineNames = engines.Select(e => string.Format("{0} ({1})",
				                                                    e.Metadata.Name,
				                                                    e.Metadata.Internal ? "Internal" : "External"));
				var message = string.Format("There are multiple engines defined for the category {0}\n{1}",
				                            category,
				                            string.Join(", ", engineNames));
				throw new InvalidOperationException(message);
			}

			return SaveCacheByCategory(category, engines);
		}

		public IEngine GetEngineByName(string name)
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

		public IEngine GetEngineForFile(string filename)
		{
			Func<Lazy<IEngine, IEngineMetadata>, bool> categoryMatchesFile = 
				e => filename.EndsWith(ExtensionResolver.GetExtensionFromCategory(e.Metadata.Category));

			var engines = Engines
				.Where(e => e.Metadata.Category.Contains("."))
				.Where(categoryMatchesFile)
				.ToList();

			if (!engines.Any())
				engines = Engines
					.Where(e => !e.Metadata.Category.Contains("."))
					.Where(categoryMatchesFile)
					.ToList();

			if (!engines.Any())
				return null;

			if (!CheckEngines(engines))
			{
				var engineNames = engines.Select(e => string.Format("{0} ({1})",
				                                                    e.Metadata.Name,
				                                                    e.Metadata.Internal ? "Internal" : "External"));
				var message = string.Format("There are multiple engines which can handle '{0}'\n{1}",
				                            filename,
				                            string.Join(", ", engineNames));
				throw new InvalidOperationException(message);
			}

			return GetEngineByName(engines.First().Metadata.Name);
		}

		EngineContainer LoadFromCacheByCategory(string category)
		{
			var key = GetCategoryKey(category);

			return LoadFromCache(key);
		}

		EngineContainer LoadFromCache(string key)
		{
			if (EngineCache == null || !EngineCache.ContainsKey(key))
				return null;

			return EngineCache[key];
		}

		EngineContainer SaveCacheByCategory(string category, IEnumerable<Lazy<IEngine, IEngineMetadata>> engines)
		{
			var key = GetCategoryKey(category);

			return SaveCache(key, engines);
		}

		EngineContainer SaveCache(string key, IEnumerable<Lazy<IEngine, IEngineMetadata>> engines)
		{
			if (EngineCache == null)
				EngineCache = new Dictionary<string, EngineContainer>();

			return EngineCache[key] = new EngineContainer(engines);
		}

		static string GetCategoryKey(string category)
		{
			var key = string.Format("Category::{0}", category);
			return key;
		}

		bool CheckEngines(IEnumerable<Lazy<IEngine, IEngineMetadata>> engines)
		{
			var engineList = engines.ToList();

			var names = engineList.Select(e => e.Metadata.Name.ToLowerInvariant()).Distinct();

			if (names.Count() > 1)
				return false;

			var internalEngines = engineList.Where(e => e.Metadata.Internal);

			if (internalEngines.Count() > 1)
				return false;

			var externalEngines = engineList.Where(e => !e.Metadata.Internal);

			if (externalEngines.Count() > 1)
				return false;

			return true;
		}
	}
}