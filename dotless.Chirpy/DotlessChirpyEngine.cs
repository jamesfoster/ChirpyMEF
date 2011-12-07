namespace dotless.Chirpy
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using ChirpyInterface;
	using Core.Parser;

	[Export(typeof(IChirpyEngine))]
	[ChirpyEngineMetadata("Dotless", "less", ".chirp.less")]
	public class DotlessChirpyEngine : IChirpyEngine
	{
		public List<string> GetDependancies(string contents, string filename)
		{
			throw new System.NotImplementedException();
		}

		public string Process(string contents, string filename)
		{
			var parser = new Parser();

			var ruleset = parser.Parse(contents, filename);

			return ruleset.AppendCSS();
		}
	}
}
