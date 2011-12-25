namespace ChirpyInterface
{
	using System.Collections.Generic;

	public abstract class SingleEngineBase : IEngine
	{
		public abstract List<string> GetDependancies(string contents, string filename);
		public abstract string Process(string contents, string filename);

		List<EngineResult> IEngine.Process(string contents, string filename)
		{
			var result = new List<EngineResult>();

			try
			{
				var output = Process(contents, filename);

				result.Add(new EngineResult {Contents = output});
			}
			catch (ChirpyException cex)
			{
				result.Add(new EngineResult {Exceptions = new List<ChirpyException> {cex}});
			}

			return result;
		}
	}
}