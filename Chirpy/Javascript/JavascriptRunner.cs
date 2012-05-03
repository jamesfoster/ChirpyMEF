namespace Chirpy.Javascript
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Threading;
	using System.Windows.Forms;
	using ChirpyInterface;

	[Export(typeof(IJavascriptRunner))]
	public class JavascriptRunner : IJavascriptRunner
	{
		[Import] public IWebFileHandler WebFileHandler { get; set; }

		public JavascriptResult Execute(string script, Dictionary<string, object> properties)
		{
			var result = new JavascriptResult(properties);
			var external = new JavascriptExternal(result, WebFileHandler);
			var autoResetEvent = new AutoResetEvent(false);

			using (var ie = new WebBrowser())
			{
				ie.ObjectForScripting = external;
				ie.DocumentCompleted += DocumentCompleted(autoResetEvent);

				var html = GetHtml(script);
				ie.DocumentText = html;

				while (!autoResetEvent.WaitOne(100))
				{
					Application.DoEvents();
				}
			}

			return result;
		}

		WebBrowserDocumentCompletedEventHandler DocumentCompleted(AutoResetEvent autoResetEvent)
		{
			return (sender, eventArgs) => autoResetEvent.Set();
		}

		string GetHtml(string script)
		{
			return @"<!DOCTYPE html>
<html>
	<head>
		<meta http-equiv=""X-UA-Compatible"" content=""IE=edge,chrome=1"" />
	</head>
	<body>
		<script>
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
				var required = {};
				return function _require(path, presource, postsource, root) {
          if(!path) return undefined;
          if(path.substr(path.lastIndexOf('.')+1).toLowerCase() !== 'js')
            path += '.js';

					var url = external.GetFullUri(path, root || '');
          var key = '~/' + url;
          var module = required[key];

					if(!module)
					{
						module = required[key] = {exports:{}};
						var code = external.Download(url);
            code = (presource || '') + ';\r\n' + code+ ';\r\n' + (postsource || '');
						var func = new Function('window', 'module', 'exports', 'require', 'external', code);

            func(module, module, module.exports, function (path) {
              return _require(path, null, null, url);
            });
					}
					return module.exports;
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
		</script>
	</body>
</html>";
		}
	}
}