namespace Chirpy.Examples
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using ChirpyInterface;

	[Export(typeof(IChirpyEngine))]
	[ChirpyEngineMetadata("NewDotless", "myless", ".my.less", true)]
	public class CompoundChirpyEngine : IChirpyEngine
	{
		[Import]
		public IEngineResolver EngineResolver { get; set; }

		public List<string> GetDependancies(string contents, string filename)
		{
			var dotlessEngine = EngineResolver.GetEngine("Dotless");

			return dotlessEngine.GetDependancies(contents, filename);
		}

		public string Process(string contents, string filename)
		{
			var dotlessEngine = EngineResolver.GetEngine("Dotless");

			var css = dotlessEngine.Process(contents, filename);

			var mybit = string.Format("==mybit== \n\n{0}", css);

			return mybit;
		}
	}
}