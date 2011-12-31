namespace ChirpyInterface
{
	using System;
	using System.Collections.Generic;

	public interface IFileHandler
	{
		/// <summary>
		/// Gets the contents of the file.
		/// </summary>
		/// <param name="filename">The full path to the file name.</param>
		/// <returns>A string containing the contents of the file</returns>
		string GetContents(string filename);

		/// <summary>
		/// Saves <paramref name="contents"/> to the specified file.
		/// </summary>
		/// <param name="filename">The full path to the file being saved.</param>
		/// <param name="contents">The contents of the file.</param>
		void SaveFile(string filename, string contents);

		/// <summary>
		/// Gets the full file name of the specified path relative to the specified file.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="relativeTo">The full path to the file which <paramref name="path"/> is relative to.</param>
		/// <returns></returns>
		string GetAbsoluteFileName(string path, string relativeTo);

		/// <summary>
		/// Gets the name of the file without the extension.
		/// </summary>
		/// <param name="filename">The file name.</param>
		/// <returns></returns>
		string GetBaseFileName(string filename);
		
		/// <summary>
		/// Determines whether the file exists or not
		/// </summary>
		/// <param name="filename">The file name.</param>
		/// <returns>true if the file exists, otherwise false</returns>
		bool FileExists(string filename);
	}
}