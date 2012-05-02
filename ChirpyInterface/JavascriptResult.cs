namespace ChirpyInterface
{
	using System.Collections.Generic;

	public class JavascriptResult
	{
		readonly object @lock = new object();

		public Dictionary<string, object> Properties { get; set; }
		public List<ChirpyException> Messages { get; set; }

		public JavascriptResult(IDictionary<string, object> properties)
		{
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
      object value;
			if (Properties.TryGetValue(name, out value))
				return value;

      return null;
		}

		public void LogMessage(string message, int? lineNumber = null, int? column = null, string filename = null, string line = null)
		{
      Log(ErrorCategory.Message, message, lineNumber, column, filename, line);
    }

		public void LogWarning(string message, int? lineNumber = null, int? column = null, string filename = null, string line = null)
		{
      Log(ErrorCategory.Warning, message, lineNumber, column, filename, line);
    }

    public void LogError(string message, int? lineNumber = null, int? column = null, string filename = null, string line = null) 
    {
      Log(ErrorCategory.Error, message, lineNumber, column, filename, line);
    }

		private void Log(ErrorCategory category, string message, int? lineNumber = null, int? column = null, string filename = null, string line = null)
		{
      Messages.Add(new ChirpyException(message, filename, lineNumber, column, line, category));
		}
	}
}