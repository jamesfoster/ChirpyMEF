using System;
using System.Collections.Generic;
namespace ChirpyInterface
{
	public interface IEngineResolver
	{
		IEngine GetEngine(string category);
		IEngine GetEngineByName(string name);
		IEngine GetEngineByFilename(string filename);
    IEnumerable<Lazy<IEngine, IEngineMetadata>> Engines { get; set; }
	}
}
