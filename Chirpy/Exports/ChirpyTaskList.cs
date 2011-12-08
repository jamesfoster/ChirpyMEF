namespace Chirpy.Exports
{
	using System.ComponentModel.Composition;
	using ChirpyInterface;

	[Export(typeof(ITaskList))]
	public class ChirpyTaskList : ITaskList
	{
		public void Add(string filename)
		{
			throw new System.NotImplementedException();
		}

		public void Remove(string filename)
		{
			throw new System.NotImplementedException();
		}

		public void RemoveAll()
		{
			throw new System.NotImplementedException();
		}

		public bool HasError(string filename)
		{
			throw new System.NotImplementedException();
		}
	}
}