namespace Chirpy.Exports
{
	using System.ComponentModel.Composition;
	using EnvDTE80;
	using Imports;

	[Export(typeof(ITaskList))]
	public class TaskList : ITaskList
	{
		[Import] public DTE2 App { get; set; }

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