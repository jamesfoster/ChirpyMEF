namespace ChirpyInterface
{
	using System;

	public class ChirpyException : Exception
	{
		public string Message { get; set; }
		public string FileName { get; set; }
		public int LineNumber { get; set; }
		public int Position { get; set; }
		public string Line { get; set; }

		public ChirpyException(string message, string fileName, int lineNumber, int position, string line)
		{
			Message = message;
			FileName = fileName;
			LineNumber = lineNumber;
			Position = position;
			Line = line;
		}
	}
}