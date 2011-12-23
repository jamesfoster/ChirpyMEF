namespace Chirpy.Exports
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Linq;
	using EnvDTE;
	using EnvDTE80;
	using Extensions;
	using Imports;
	using Microsoft.VisualStudio.Shell;
	using Microsoft.VisualStudio.Shell.Interop;
	using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

	[Export(typeof(ITaskList))]
	public class TaskList : ITaskList, IPartImportsSatisfiedNotification
	{
		[Import] public DTE2 App { get; set; }

		protected ServiceProvider ServiceProvider { get; set; }
		protected ErrorListProvider ErrorListProvider { get; set; }

		public List<ErrorTask> Tasks { get; set; }
		public Dictionary<ErrorTask, Project> TaskProjects { get; set; }

		public void OnImportsSatisfied()
		{
			var serviceProvider = App as IServiceProvider;

			if (serviceProvider == null)
				return; // throw?

			ServiceProvider = new ServiceProvider(serviceProvider);

			ErrorListProvider = new ErrorListProvider(ServiceProvider)
			                    	{
			                    		ProviderName = GetType().Assembly.FullName,
			                    		ProviderGuid = new Guid("F1415C4C-5D67-401F-A81C-71F0721BB6F0")
			                    	};

			ErrorListProvider.Show();

			Tasks = new List<ErrorTask>();
			TaskProjects = new Dictionary<ErrorTask, Project>();
		}

		public void Add(string message, string filename, int line, int column, TaskErrorCategory category)
		{
			if (message == null) return;
			if (filename == null) return;

			var projectItem = App.Solution.FindProjectItem(filename);

			if (projectItem == null) return;

			Add(projectItem.ContainingProject, message, filename, line, column, category);
		}

		public void Add(Project project, string message, string filename, int line, int column, TaskErrorCategory category)
		{
			if (message == null) return;
			if (filename == null) return;
			if (project == null) return;

			var task = new ErrorTask
			           	{
			           		ErrorCategory = category,
			           		Document = filename,
			           		Line = line,
			           		Column = column,
			           		Text = message
			           	};

			Add(project, task);
		}

		void Add(Project project, ErrorTask task)
		{
			IVsHierarchy hierarchy = null;
			if (project != null && ServiceProvider != null)
			{
				var solution = ServiceProvider.GetService(typeof (IVsSolution)) as IVsSolution;
				if (solution != null)
				{
					solution.GetProjectOfUniqueName(project.UniqueName, out hierarchy);
				}
			}

			task.HierarchyItem = hierarchy;
			task.Navigate += Navigate;

			if (ErrorListProvider != null)
			{
				ErrorListProvider.Tasks.Add(task);
			}

			Tasks.Add(task);

			if (project != null)
			{
				lock (TaskProjects)
				{
					TaskProjects.Add(task, project);
				}
			}

			if (App != null && App.ToolWindows != null)
			{
				App.ToolWindows.ErrorList.Parent.Activate();
			}
		}

		void Navigate(object sender, EventArgs e)
		{
			var task = (ErrorTask) sender;

			task.Line++;
			ErrorListProvider.Navigate(task, new Guid(EnvDTE.Constants.vsViewKindCode));
			task.Line--;
		}

		public void Remove(string filename)
		{
			foreach (var task in Tasks.Where(x => x.Document.Is(filename)).ToArray())
			{
				Remove(task);
			}
		}

		public void Remove(Project project)
		{
			lock (TaskProjects)
			{
				var tasks = TaskProjects
					.Where(x => x.Value == project)
					.Select(x => x.Key)
					.ToArray();

				foreach (var task in tasks)
				{
					Remove(task);
				}
			}
		}

		void Remove(ErrorTask task)
		{
			if (ErrorListProvider != null)
				ErrorListProvider.Tasks.Remove(task);

			Tasks.Remove(task);

			lock (TaskProjects)
			{
				if (TaskProjects.ContainsKey(task))
					TaskProjects.Remove(task);
			}
		}

		public void RemoveAll()
		{
			if (ErrorListProvider != null)
				ErrorListProvider.Tasks.Clear();

			Tasks.Clear();
			TaskProjects.Clear();
		}

		public bool HasError(string filename)
		{
			return Tasks.Any(x => x.Document.Is(filename));
		}
	}
}