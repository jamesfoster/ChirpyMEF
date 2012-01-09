namespace ChirpyInterface
{
	public interface IWebFileHandler
	{
		/// <summary>
		/// Gets the contents of the file.
		/// </summary>
		/// <param name="filename">The full path to the file name.</param>
		/// <returns>A string containing the contents of the file</returns>
		string GetContents(string filename);

		/// <summary>
		/// Gets the full file name of the specified path relative to the specified file.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="relativeTo">The full path to the file which <paramref name="path"/> is relative to.</param>
		/// <returns></returns>
		string GetAbsoluteFileName(string path, string relativeTo);
	}
}