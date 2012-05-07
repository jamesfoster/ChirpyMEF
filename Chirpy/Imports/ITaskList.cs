namespace Chirpy.Imports
{
	using ChirpyInterface;
	using EnvDTE;

	public interface ITaskList
	{
		void Add(ChirpyException exception);

		void Add(string message, string filename, ErrorCategory category);

		/// <summary>
		/// Adds a TaskList error for the specified file.
		/// </summary>
		/// <param name="message">The error message</param>
		/// <param name="filename">The file containing the error.</param>
		/// <param name="line">The line the error appears on</param>
		/// <param name="lineNumber">The line number</param>
		/// <param name="column">The position in the line where the error appears</param>
		/// <param name="category">The category of the error</param>
		void Add(string message, string filename, string line, int? lineNumber, int? column, ErrorCategory category);

		/// <summary>
		/// Adds a TaskList error for the specified file in the specified project.
		/// </summary>
		/// <param name="project">The project containing the file</param>
		/// <param name="message">The error message</param>
		/// <param name="filename">The file containing the error.</param>
		/// <param name="line">The line the error appears on</param>
		/// <param name="lineNumber">The line number</param>
		/// <param name="column">The position in the line where the error appears</param>
		/// <param name="category">The category of the error</param>
		void Add(Project project, string message, string filename, string line, int? lineNumber, int? column, ErrorCategory category);

		/// <summary>
		/// Removes any errors for the specified file.
		/// </summary>
		/// <param name="filename">The filename.</param>
		void Remove(string filename);

		/// <summary>
		/// Removes any errors for the specified project.
		/// </summary>
		/// <param name="project">The project</param>
		void Remove(Project project);

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