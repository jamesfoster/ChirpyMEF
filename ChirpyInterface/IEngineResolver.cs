namespace ChirpyInterface
{
	using System;
	using System.Collections.Generic;

	public interface IEngineResolver
	{
		IEnumerable<Lazy<IChirpyEngine, IChirpyEngineMetadata>> GetEngines(string category, string filename);
		IChirpyEngine GetEngine(string name);
	}
}
