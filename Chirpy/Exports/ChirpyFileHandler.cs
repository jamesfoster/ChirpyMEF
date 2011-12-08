namespace Chirpy.Exports
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.IO;
	using ChirpyInterface;

	[Export(typeof(IFileHandler))]
	class ChirpyFileHandler : IFileHandler
	{
		public string GetContents(string filename)
		{
			return string.Format("contents of '{0}'", filename);
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