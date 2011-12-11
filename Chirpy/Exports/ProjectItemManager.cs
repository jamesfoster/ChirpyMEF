namespace Chirpy.Exports
{
	using System.ComponentModel.Composition;
	using ChirpyInterface;

	[Export(typeof(IProjectItemManager))]
	public class ProjectItemManager : IProjectItemManager
	{
		public void AddFile(string filename, string dependsUpon, string contents)
		{
			throw new System.NotImplementedException();
		}
	}
}