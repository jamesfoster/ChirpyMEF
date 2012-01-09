namespace Chirpy.Javascript
{
	using ChirpyInterface;
	using Jurassic;
	using Jurassic.Library;

	public class JavascriptExternal : ObjectInstance
	{
		public JavascriptResult JavascriptResult { get; set; }
		public IWebFileHandler WebFileHandler { get; set; }

		public JavascriptExternal(ScriptEngine engine, JavascriptResult javascriptResult, IWebFileHandler webFileHandler)
			: base(engine)
		{
			JavascriptResult = javascriptResult;
			WebFileHandler = webFileHandler;

			PopulateFunctions();
		}

		[JSFunction]
		public void Set(string name, object value)
		{
			JavascriptResult.Set(name, value);
		}

		[JSFunction]
		public object Get(string name)
		{
			return JavascriptResult.Get(name);
		}

		[JSFunction]
		public void LogMessage(string message, int lineNumber, int column, string filename = null, string line = null)
		{
			JavascriptResult.LogMessage(message, lineNumber, column, filename, line);
		}

		[JSFunction]
		public void LogWarning(string message, int lineNumber, int column, string filename = null, string line = null)
		{
			JavascriptResult.LogWarning(message, lineNumber, column, filename, line);
		}

		[JSFunction]
		public void LogError(string message, int lineNumber, int column, string filename = null, string line = null)
		{
			JavascriptResult.LogError(message, lineNumber, column, filename, line);
		}

		[JSFunction]
		public string GetFullUri(string path, string relativeTo)
		{
			return WebFileHandler.GetAbsoluteFileName(path, relativeTo);
		}

		[JSFunction]
		public string Download(string path)
		{
			return WebFileHandler.GetContents(path);
		}
	}
}