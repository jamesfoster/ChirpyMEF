namespace ChirpyInterface
{
	using System;

	public class ChirpyException : Exception
	{
		public string FileName { get; set; }
		public int? LineNumber { get; set; }
		public int? Position { get; set; }
		public string Line { get; set; }
		public ErrorCategory Category { get; set; }

		public ChirpyException(string message, string fileName, int? lineNumber, int? position, string line)
			: this(message, fileName, lineNumber, position, line, ErrorCategory.Error)
		{
		}

		public ChirpyException(string message, string fileName, int? lineNumber, int? position, string line, ErrorCategory category)
			: base(message)
		{
			FileName = fileName;
			LineNumber = lineNumber;
			Position = position;
			Line = line;
			Category = category;
		}

		public ChirpyException(string message, string fileName)
			: this(message, fileName, ErrorCategory.Error)
		{
		}

		public ChirpyException(string message, string fileName, ErrorCategory category)
			: this(message, fileName, null, null, null, category)
		{
		}
	}
}