namespace ChirpyTest
{
	using System.Collections;
	using Chirpy;
	using EnvDTE;
	using EnvDTE80;
	using Machine.Specifications;
	using Microsoft.VisualStudio.Shell.Interop;
	using Moq;

	public class AddIn_context
	{
		protected static Mock<DTE2> AppMock;
		protected static Mock<Solution> SolutionMock;
		protected static Mock<IVsSolution> VsSolutionMock;
		protected static Mock<IVsHierarchy> VsHierarchyMock;
		protected static Mock<ProjectItem> ProjectItemMock;
		protected static Mock<Project> ProjectMock;

		protected static Mock<ToolWindows> ToolsWindowMock;
		protected static Mock<OutputWindow> OutputWindowMock;
		protected static Mock<OutputWindowPanes> OutputWindowPanesMock;
		protected static Mock<OutputWindowPane> OutputWindowPaneMock;

		protected static Mock<ErrorList> ErrorListMock;
		protected static Mock<Window> ErrorWindowMock;

		Establish context = () =>
			{
				AppMock = new Mock<DTE2>();

				SetupSolution();

				SetupOutputWindow();

				SetupErrorList();
			};

		static void SetupSolution()
		{
			SolutionMock = new Mock<Solution>();
			VsSolutionMock = new Mock<IVsSolution>();
			VsHierarchyMock = new Mock<IVsHierarchy>();
			ProjectItemMock = new Mock<ProjectItem>();
			ProjectMock = new Mock<Project>();

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

			var hierarchy = VsHierarchyMock.Object;
			VsSolutionMock.Setup(s => s.GetProjectOfUniqueName(Moq.It.IsAny<string>(), out hierarchy));
		}

		static void SetupOutputWindow()
		{
			ToolsWindowMock = new Mock<ToolWindows>();
			OutputWindowMock = new Mock<OutputWindow>();
			OutputWindowPanesMock = new Mock<OutputWindowPanes>();
			OutputWindowPaneMock = new Mock<OutputWindowPane>();

			var panes = OutputWindowPanesMock.As<IEnumerable>();

			AppMock
				.SetupGet(a => a.ToolWindows)
				.Returns(ToolsWindowMock.Object);

			ToolsWindowMock
				.SetupGet(t => t.OutputWindow)
				.Returns(OutputWindowMock.Object);

			OutputWindowMock
				.SetupGet(w => w.OutputWindowPanes)
				.Returns(OutputWindowPanesMock.Object);

			panes
				.Setup(p => p.GetEnumerator())
				.Returns(() => new object[0].GetEnumerator());

			OutputWindowPanesMock
				.Setup(p => p.Add(Moq.It.IsAny<string>()))
				.Returns(OutputWindowPaneMock.Object);
		}

		static void SetupErrorList()
		{
			ErrorListMock = new Mock<ErrorList>();
			ErrorWindowMock = new Mock<Window>();

			ToolsWindowMock
				.SetupGet(t => t.ErrorList)
				.Returns(ErrorListMock.Object);

			ErrorListMock
				.SetupGet(e => e.Parent)
				.Returns(ErrorWindowMock.Object);
		}
	}
}