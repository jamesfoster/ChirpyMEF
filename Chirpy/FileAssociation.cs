namespace Chirpy
{
	using EnvDTE;

	public class FileAssociation
	{
		public string FileName { get; set; }
		public ProjectItem Parent { get; set; }

		public FileAssociation(string fileName, ProjectItem parent)
		{
			FileName = fileName;
			Parent = parent;
		}
	}
}