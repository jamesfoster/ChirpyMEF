namespace Chirpy.Engines
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using ChirpyInterface;

	[Export(typeof(IChirpyEngine))]
	[ChirpyEngineMetadata("NewDotless", "less", "my", true)]
	public class CompoundChirpyEngine : IChirpyEngine
	{
		[Import]
		public IEngineResolver EngineResolver { get; set; }

		public List<string> GetDependancies(string contents, string filename)
		{
			var dotlessEngine = EngineResolver.GetEngineByName("Dotless");

			return dotlessEngine.GetDependancies(contents, filename);
		}

		public string Process(string contents, string filename)
		{
			var dotlessEngine = EngineResolver.GetEngineByName("Dotless");

			var css = dotlessEngine.Process(contents, filename);

			var mybit = string.Format("==mybit== \n\n{0}", css);

			return mybit;
		}
	}
}