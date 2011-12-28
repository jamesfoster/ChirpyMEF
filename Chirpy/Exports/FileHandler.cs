namespace Chirpy.Exports
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.IO;
	using ChirpyInterface;

	[Export(typeof(IFileHandler))]
	public class FileHandler : IFileHandler
	{
		public string GetContents(string filename)
		{
			if(!File.Exists(filename))
				return null;

			return File.ReadAllText(filename);
		}

		public void SaveFile(string filename, string contents)
		{
			File.WriteAllText(filename, contents);
		}

		public string GetAbsoluteFileName(string path, string relativeTo)
		{
			var relativeDirectory = Path.GetDirectoryName(relativeTo) ?? "";

			return Path.Combine(relativeDirectory, path);
		}

		public string GetBaseFileName(string filename)
		{
			var baseFileName = Path.GetFileNameWithoutExtension(filename);
			var directory = Path.GetDirectoryName(filename);
			return Path.Combine(directory, baseFileName);
		}
	}
}