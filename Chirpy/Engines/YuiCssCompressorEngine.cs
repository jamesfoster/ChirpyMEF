namespace Chirpy.Engines
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using ChirpyInterface;

	[Export(typeof (IEngine))]
	[EngineMetadata("YUI CSS Compressor", "1.6.0.2", "yui.css", "css", true, Minifier = true)]
	public class YuiCssCompressorEngine : IEngine
	{
		public List<string> GetDependancies(string contents, string filename)
		{
			return null;
		}

		public string Process(string contents, string filename)
		{
			return Yahoo.Yui.Compressor.CssCompressor.Compress(contents);
		}
	}
}