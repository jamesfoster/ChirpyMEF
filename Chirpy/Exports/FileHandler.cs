namespace Chirpy.Exports
{
	using System;
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
			if (Path.IsPathRooted(path))
				return path;

			if (!Path.IsPathRooted(relativeTo))
				throw new InvalidOperationException(string.Format("Unable to get absolute path of '{0}' relative to '{1}'", path, relativeTo));

			var relativeDirectory = Path.GetDirectoryName(relativeTo);

			var relativePath = Path.Combine(relativeDirectory, path);

			return Path.GetFullPath(relativePath);
		}

		public string GetBaseFileName(string filename)
		{
			var baseFileName = Path.GetFileNameWithoutExtension(filename);
			var directory = Path.GetDirectoryName(filename);
			return Path.Combine(directory, baseFileName);
		}

		public bool FileExists(string filename)
		{
			return File.Exists(filename);
		}
	}
}