namespace ChirpyTest.TaskListSpecs
{
	using System;
	using Chirpy;
	using Chirpy.Exports;
	using EnvDTE;
	using EnvDTE80;
	using Machine.Specifications;
	using Microsoft.VisualStudio.Shell.Interop;
	using Moq;
	using TaskList = Chirpy.Exports.TaskList;

	class TaskList_context
	{
		protected static TaskList TaskList;
		protected static Mock<DTE2> AppMock;
		protected static Mock<IServiceProvider> ServiceProviderMock;
		protected static Mock<IErrorListProvider> ErrorListProviderMock;
		protected static Mock<Solution> SolutionMock;
		protected static Mock<IVsSolution> VsSolutionMock;
		protected static Mock<IVsHierarchy> VsHierarchyMock;
		protected static Mock<ProjectItem> ProjectItemMock;
		protected static Mock<Project> ProjectMock;

		Establish context = () =>
			{
				AppMock = new Mock<DTE2>();
				ServiceProviderMock = new Mock<IServiceProvider>();
				ErrorListProviderMock = new Mock<IErrorListProvider>();

				SolutionMock = new Mock<Solution>();
				VsSolutionMock = new Mock<IVsSolution>();
				VsHierarchyMock = new Mock<IVsHierarchy>();
				ProjectItemMock = new Mock<ProjectItem>();
				ProjectMock = new Mock<Project>();

				TaskList = new TaskList
				           	{
				           		App = AppMock.Object,
				           		ServiceProvider = ServiceProviderMock.Object,
				           		ErrorListProvider = ErrorListProviderMock.Object
				           	};

				AppMock
					.Setup(a => a.Solution)
					.Returns(SolutionMock.Object);

				SolutionMock
					.Setup(s => s.FindProjectItem(Moq.It.IsAny<string>()))
					.Returns(ProjectItemMock.Object);

				ProjectItemMock
					.SetupGet(pi => pi.ContainingProject)
					.Returns(ProjectMock.Object);

				ProjectMock
					.SetupGet(p => p.UniqueName)
					.Returns("DemoProject");

				ServiceProviderMock
					.Setup(p => p.GetService(typeof (IVsSolution)))
					.Returns(VsSolutionMock.Object);

				var hierarchy = VsHierarchyMock.Object;
				VsSolutionMock.Setup(s => s.GetProjectOfUniqueName(Moq.It.IsAny<string>(), out hierarchy));

				TaskList.OnImportsSatisfied();
			};
	}
}