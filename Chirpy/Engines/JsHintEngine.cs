namespace Chirpy.Engines
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using ChirpyInterface;

	[Export(typeof (IEngine))]
	[EngineMetadata("JS Hint", "1.0", ".js", true)]
	public class JsHintEngine : IEngine
	{
		[Import] public IJavascriptRunner JavascriptRunner { get; set; }

		public List<string> GetDependancies(string contents, string filename)
		{
			return null;
		}

		public List<EngineResult> Process(string contents, string filename)
		{
			var result = new EngineResult {FileName = filename};
			var properties = new Dictionary<string, object>();

			properties["js"] = contents;

			const string script = @"
var exports = require('https://raw.github.com/jshint/jshint/master/jshint.js', null, '');
var JSHINT = exports.JSHINT;

var js = external.Get('js');
var result = JSHINT(js);
var errors = JSHINT.errors;
 
external.Set('result', result);
external.Set('errors', errors.length);
if (!result) 
{
    for (var i = 0; i < errors.length; i++) 
    {
        external.LogWarning((errors[i].reason || '') + ': ' + (errors[i].evidence || ''), errors[i].line || 0, errors[i].character || 0);
    }
}
";

			var javascriptResult = JavascriptRunner.Execute(script, properties);

			result.Exceptions.AddRange(javascriptResult.Messages);

			return new List<EngineResult> {result};
		}
	}
}