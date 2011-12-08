namespace ChirpyInterface
{
	using System;
	using System.Collections.Generic;

	public interface IEngineResolver
	{
		IChirpyEngine GetEngine(string category, string subCategory);
		IChirpyEngine GetEngineByName(string name);
		IChirpyEngine GetEngineForFile(string filename);
	}
}
