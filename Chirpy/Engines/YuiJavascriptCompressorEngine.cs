namespace Chirpy.Engines
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using ChirpyInterface;

	[Export(typeof (IEngine))]
	[EngineMetadata("YUI Javascript Compressor", "1.6.0.2", "yui.js", "js", true, Minifier = true)]
	public class YuiJavascriptCompressorEngine : IEngine
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