namespace ChirpyInterface
{
	using System.Collections.Generic;

	public interface IEngine
	{
		List<string> GetDependencies(string contents, string filename);
		List<EngineResult> Process(string contents, string filename);
	}
}
