namespace Chirpy.Engines
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using ChirpyInterface;

	[Export(typeof (IEngine))]
	[EngineMetadata("YUI CSS Compressor", "1.6.0.2", "yui.css", true, Minifier = true)]
	public class YuiCssCompressorEngine : SingleEngineBase
	{
		public override List<string> GetDependancies(string contents, string filename)
		{
			return null;
		}

		public override string Process(string contents, string filename, out string outputExtension)
		{
			outputExtension = "min.css";

			return Yahoo.Yui.Compressor.CssCompressor.Compress(contents);
		}
	}
}