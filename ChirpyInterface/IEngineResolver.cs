namespace ChirpyInterface
{
	using System;
	using System.Collections.Generic;

	public interface IEngineResolver
	{
		IEnumerable<Lazy<IChirpyEngine, IChirpyEngineMetadata>> GetEngines(string category, string subCategory);
		IChirpyEngine GetEngine(string name);
		IChirpyEngine GetEngineForFile(string filename);
	}
}
