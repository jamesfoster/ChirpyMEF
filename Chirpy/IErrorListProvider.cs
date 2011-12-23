namespace Chirpy
{
	using System;
	using Microsoft.VisualStudio.Shell;

	public interface IErrorListProvider
	{
		void Show();
		void Navigate(ErrorTask task, Guid logicalView);
		void AddTask(ErrorTask task);
		void RemoveTask(ErrorTask task);
		void ClearTasks();
	}
}