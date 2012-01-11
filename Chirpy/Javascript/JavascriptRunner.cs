namespace Chirpy.Javascript
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using ChirpyInterface;

	[Export(typeof(IJavascriptRunner))]
	public class JavascriptRunner : IJavascriptRunner
	{
		[Import] public IWebFileHandler WebFileHandler { get; set; }

		public JavascriptResult Execute(string script, Dictionary<string, object> properties)
		{
			var scriptEngine = new Jurassic.ScriptEngine();

			var result = new JavascriptResult(properties);
			var external = new JavascriptExternal(scriptEngine, result, WebFileHandler);

			scriptEngine.EnableDebugging = true;
			scriptEngine.SetGlobalValue("external", external);

			scriptEngine.Execute(GetScript(script));

			return result;
		}

		string GetScript(string script)
		{
			return @"
var window = {};

var module = window;

var onerror = window.onerror = function(msg, file, line){
	external.LogError(msg, line, 0, file, null);
};

var console = window.console = {
	log: function(msg){
		external.LogMessage(msg + '');
	},

	error: function(msg){
		external.LogError(msg + '');
	},

	warning: function(msg){
		external.LogWarning(msg + '');
	}
};

var require = window.require = (function() {
	var required = {}, bases = [];
	return function (path, presource, postsource) {
		var key = external.GetFullUri(path, bases.length == 0 ? '' : bases[bases.length-1]);

		if(!required[key])
		{
			required[key] = {};
			var code = external.Download(key);
			var func = new Function('exports', (presource || '') + ';\r\n' + code+ ';\r\n' + (postsource || ''));

			bases.push(key);
			func(required[key]);
			bases.pop();
		}
		return required[key];
	};
})();

try
{
	" + script + @"
}
catch(x)
{
	var line = x.line || 0;
	var column = x.col || x.column || 0;
	external.LogError(x.message, line, column);
}
";
		}
	}
}