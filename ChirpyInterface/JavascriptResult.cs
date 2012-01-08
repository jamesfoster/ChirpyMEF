namespace ChirpyInterface
{
	using System.Collections.Generic;
	using System.Runtime.InteropServices;

	[ComVisible(true)]
	public class JavascriptResult
	{
		readonly object @lock = new object();

		public IFileHandler FileHandler { get; set; }
		public Dictionary<string, object> Properties { get; set; }
		public List<ChirpyException> Messages { get; set; }

		public JavascriptResult(IFileHandler fileHandler, IDictionary<string, object> properties)
		{
			FileHandler = fileHandler;

			if(properties == null)
				Properties = new Dictionary<string, object>();
			else
				Properties = new Dictionary<string, object>(properties);

			Messages = new List<ChirpyException>();
		}

		public void Set(string name, object value)
		{
			lock (@lock)
			{
				Properties[name] = value;
			}
		}

		public object Get(string name)
		{
			lock (@lock)
			{
				if (!Properties.ContainsKey(name))
					return null;

				return Properties[name];
			}
		}

		public void LogMessage(string message, int? lineNumber = null, int? column = null, string line = null, string filename = null)
		{
			Messages.Add(new ChirpyException(message, line, lineNumber, column, filename, ErrorCategory.Message));
		}

		public void LogWarning(string message, int? lineNumber = null, int? column = null, string line = null, string filename = null)
		{
			Messages.Add(new ChirpyException(message, line, lineNumber, column, filename, ErrorCategory.Warning));
		}

		public void LogError(string message, int? lineNumber = null, int? column = null, string line = null, string filename = null)
		{
			Messages.Add(new ChirpyException(message, line, lineNumber, column, filename, ErrorCategory.Error));
		}

		public string GetFullUri(string path, string relativeTo)
		{
			return FileHandler.GetAbsoluteFileName(path, relativeTo);
		}

		public string Download(string path)
		{
			return FileHandler.GetContents(path);
		}
	}
}