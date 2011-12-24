namespace ChirpyInterface
{
	using System.Collections.Generic;

	public abstract class SingleEngineBase : IEngine
	{
		public abstract List<string> GetDependancies(string contents, string filename);
		public abstract string Process(string contents, string filename);

		List<EngineResult> IEngine.Process(string contents, string filename)
		{
			var output = Process(contents, filename);

			return new List<EngineResult>
			       	{
			       		new EngineResult {Contents = output}
			       	};
		}
	}
}