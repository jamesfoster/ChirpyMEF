namespace Chirpy.Engines
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using ChirpyInterface;

	[Export(typeof (IEngine))]
	[EngineMetadata("CSS Lint", "1.0", "css", true)]
	public class CssLintEngine : IEngine
	{
		[Import] public IJavascriptRunner JavascriptRunner { get; set; }

		public List<string> GetDependencies(string contents, string filename)
		{
			return null;
		}

		public List<EngineResult> Process(string contents, string filename)
		{
			var result = new EngineResult {FileName = filename};
			var properties = new Dictionary<string, object>();

			properties["css"] = contents;

			const string script = @"
var exports = require('http://csslint.net/js/csslint.js', null, 'exports.CSSLint = CSSLint;');
var CSSLint = exports.CSSLint;

var css = external.Get('css');

var result = CSSLint.verify(css);

for(var i = 0; i < result.messages.length; i++)
{
	var message = result.messages[i];
	var messageText = message.message + ': ' + message.evidence;

	external.LogMessage(messageText, message.line, message.col);
}
";

			var javascriptResult = JavascriptRunner.Execute(script, properties);

			result.Exceptions.AddRange(javascriptResult.Messages);

			return new List<EngineResult> {result};
		}
	}
}