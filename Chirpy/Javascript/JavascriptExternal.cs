namespace Chirpy.Javascript
{
	using ChirpyInterface;

	public class JavascriptExternal
	{
		JavascriptResult JavascriptResult { get; set; }
		IWebFileHandler WebFileHandler { get; set; }

		public JavascriptExternal(JavascriptResult result, IWebFileHandler webFileHandler)
		{
			JavascriptResult = result;
			WebFileHandler = webFileHandler;
		}

		public void Set(string name, object value)
		{
			JavascriptResult.Set(name, value);
		}

		public object Get(string name)
		{
			return JavascriptResult.Get(name);
		}

		public void LogMessage(string message, int lineNumber = 0, int column = 0, string filename = null, string line = null)
		{
			JavascriptResult.LogMessage(message, lineNumber, column, filename, line);
		}

		public void LogWarning(string message, int lineNumber = 0, int column = 0, string filename = null, string line = null)
		{
      JavascriptResult.LogWarning(message, lineNumber, column, filename, line);
		}

		public void LogError(string message, int lineNumber = 0, int column = 0, string filename = null, string line = null)
		{
      JavascriptResult.LogError(message, lineNumber, column, filename, line);
		}

		public string GetFullUri(string path, string relativeTo)
		{
			return WebFileHandler.GetAbsoluteFileName(path, relativeTo);
		}

		public string Download(string path)
		{
			return WebFileHandler.GetContents(path);
		}
	}
}