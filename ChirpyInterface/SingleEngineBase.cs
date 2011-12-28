namespace ChirpyInterface
{
	using System.Collections.Generic;

	public abstract class SingleEngineBase : IEngine
	{
		public abstract List<string> GetDependancies(string contents, string filename);
		public abstract string Process(string contents, string filename, out string outputExtension);

		List<EngineResult> IEngine.Process(string contents, string filename)
		{
			var engineResult = new EngineResult();
			var result = new List<EngineResult> {engineResult};

			try
			{
				string outputExtension;

				var output = Process(contents, filename, out outputExtension);

				engineResult.Contents = output;
				engineResult.Extension = outputExtension;
			}
			catch (ChirpyException cex)
			{
				engineResult.Exceptions.Add(cex);
			}

			return result;
		}
	}
}