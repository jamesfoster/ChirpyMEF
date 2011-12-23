namespace Chirpy.Imports
{
	public interface ITaskList
	{
		/// <summary>
		/// Adds a TaskList error for the specified file.
		/// </summary>
		/// <param name="filename">The file name.</param>
		void Add(string filename/* ... */);

		/// <summary>
		/// Removes any errors for the specified file.
		/// </summary>
		/// <param name="filename">The filename.</param>
		void Remove(string filename);

		/// <summary>
		/// Removes all errors.
		/// </summary>
		void RemoveAll();

		/// <summary>
		/// Determines whether the specified file has an error.
		/// </summary>
		/// <param name="filename">The file name.</param>
		/// <returns>
		///   <c>true</c> if the specified file has an error; otherwise, <c>false</c>.
		/// </returns>
		bool HasError(string filename);
	}
}