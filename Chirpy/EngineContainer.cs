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
			return Execute(e => e.GetDependancies(contents, filename));
		}

		public string Process(string contents, string filename)
		{
			return Execute(e => e.Process(contents, filename));
		}

		T Execute<T>(Func<IEngine, T> action)
		{
			// try external first
			var engine = Engines.FirstOrDefault(e => !e.Metadata.Internal);

			T result;

			var success = Try(action, engine, out result);

			// try internal
			if (!success)
				engine = Engines.FirstOrDefault(e => e.Metadata.Internal);

			Try(action, engine, out result);

			return result;
		}

		bool Try<T>(Func<IEngine, T> action, Lazy<IEngine, IEngineMetadata> engine, out T result)
		{
			if(engine == null)
			{
				result = default(T);
				return false;
			}

			try
			{
				result = action(engine.Value);
				return true;
			}
			catch (ChirpyException e)
			{
//				if(Internal.Length == 1 || engine.Metadata.Internal)
//				{
//					// Log exception
//				}
			}
			catch (Exception e)
			{
//				if(Internal.Length == 1 || engine.Metadata.Internal)
//				{
//					// Log exception
//				}
			}

			result = default(T);
			return false;
		}
	}
}