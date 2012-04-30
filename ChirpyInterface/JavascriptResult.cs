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
			lock (@lock)
			{
				if (!Properties.ContainsKey(name))
					return null;

				return Properties[name];
			}
		}

		public void LogMessage(string message, int? lineNumber = null, int? column = null, string filename = null, string line = null)
		{
			Messages.Add(new ChirpyException(message, filename, lineNumber, column, line, ErrorCategory.Message));
		}

		public void LogWarning(string message, int? lineNumber = null, int? column = null, string filename = null, string line = null)
		{
			Messages.Add(new ChirpyException(message, filename, lineNumber, column, line, ErrorCategory.Warning));
		}

		public void LogError(string message, int? lineNumber = null, int? column = null, string filename = null, string line = null)
		{
			Messages.Add(new ChirpyException(message, filename, lineNumber, column, line, ErrorCategory.Error));
		}
	}
}