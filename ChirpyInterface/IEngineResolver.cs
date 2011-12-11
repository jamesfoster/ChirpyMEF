namespace ChirpyInterface
{
	using System;
	using System.Collections.Generic;

	public interface IEngineResolver
	{
		IChirpyEngine GetEngine(string category);
		IChirpyEngine GetEngineByName(string name);
		IChirpyEngine GetEngineForFile(string filename);
	}
}
