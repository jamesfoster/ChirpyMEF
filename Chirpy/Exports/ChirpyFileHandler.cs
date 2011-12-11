namespace Chirpy.Exports
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.IO;
	using ChirpyInterface;

	[Export(typeof(IFileHandler))]
	public class ChirpyFileHandler : IFileHandler
	{
		public string GetContents(string filename)
		{
			if(!File.Exists(filename))
				return null;

			return File.ReadAllText(filename);
		}

		public string GetFileName(string path, string relativeTo)
		{
			var relativeDirectory = Path.GetDirectoryName(relativeTo) ?? "";

			return Path.Combine(relativeDirectory, path);
		}

		public string GetBaseFileName(string filename)
		{
			throw new System.NotImplementedException();
		}

		public void Refresh(string filename)
		{
			throw new System.NotImplementedException();
		}

		public void RefreshMany(List<string> filenames)
		{
			throw new System.NotImplementedException();
		}
	}
}