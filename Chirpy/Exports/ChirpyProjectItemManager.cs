namespace Chirpy.Exports
{
	using System.ComponentModel.Composition;
	using ChirpyInterface;

	[Export(typeof(IProjectItemManager))]
	public class ChirpyProjectItemManager : IProjectItemManager
	{
		public void AddFile(string filename, string dependsUpon)
		{
			throw new System.NotImplementedException();
		}
	}
}