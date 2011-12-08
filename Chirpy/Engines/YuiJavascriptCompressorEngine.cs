namespace Chirpy.Engines
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using ChirpyInterface;

	[Export(typeof (IChirpyEngine))]
	[ChirpyEngineMetadata("YUI Javascript Compressor", "js", "yui", true)]
	public class YuiJavascriptCompressorEngine : IChirpyEngine
	{
		public List<string> GetDependancies(string contents, string filename)
		{
			return null;
		}

		public string Process(string contents, string filename)
		{
			return Yahoo.Yui.Compressor.JavaScriptCompressor.Compress(contents);
		}
	}
}