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
		[Import] public IFileHandler FileHandler { get; set; }

		public JavascriptResult Execute(string script, Dictionary<string, object> properties)
		{
			var external = new JavascriptResult(FileHandler, properties);
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

			return external;
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
			var module = window;
			window.onerror = function(msg, file, line){
				external.LogError(msg, line, 0, null, file);
			};
  
			window.console = {
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

			window.require = (function() {
				var required = {}, bases = [];
				return function (path, presource, postsource) {
					var key = external.GetFullUri(path, bases.length == 0 ? null : bases[bases.length-1]);

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
				external.LogError(line, column, x.message);
			}
		</script>
	</body>
</html>";
		}
	}
}