namespace Chirpy.Examples
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using ChirpyInterface;

	[Export(typeof(IChirpyEngine))]
	[ChirpyEngineMetadata("Dotless", "less", ".chirp.less", true)]
	public class DotlessChirpyEngine : IChirpyEngine
	{
		public List<string> GetDependancies(string contents, string filename)
		{
			throw new System.NotImplementedException();
		}

		public string Process(string contents, string filename)
		{
			return string.Format("internal {0}", contents);
		}
	}
}
