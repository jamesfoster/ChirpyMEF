namespace Chirpy
{
	using System;
	using Exports;
	using Microsoft.VisualStudio.Shell;

	public class ErrorListProviderWrapper : IErrorListProvider
	{
		public ErrorListProvider ErrorListProvider { get; set; }

		public ErrorListProviderWrapper(ErrorListProvider errorListProvider)
		{
			ErrorListProvider = errorListProvider;
		}

		public void Show()
		{
			ErrorListProvider.Show();
		}

		public void Navigate(ErrorTask task, Guid logicalView)
		{
			ErrorListProvider.Navigate(task, logicalView);
		}

		public void AddTask(ErrorTask task)
		{
			ErrorListProvider.Tasks.Add(task);
		}

		public void RemoveTask(ErrorTask task)
		{
			ErrorListProvider.Tasks.Remove(task);
		}

		public void ClearTasks()
		{
			ErrorListProvider.Tasks.Clear();
		}
	}
}