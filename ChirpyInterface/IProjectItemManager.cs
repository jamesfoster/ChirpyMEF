namespace ChirpyInterface
{
	public interface IProjectItemManager
	{
		/// <summary>
		/// Adds the specified file as a dependant of <paramref name="dependsUpon"/> to the project.
		/// </summary>
		/// <param name="filename">The file being added to the project.</param>
		/// <param name="dependsUpon">The file name of the parent file in the project.</param>
		void AddFile(string filename, string dependsUpon);
	}
}