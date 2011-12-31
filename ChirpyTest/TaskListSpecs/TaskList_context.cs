namespace ChirpyTest.TaskListSpecs
{
	using System;
	using Chirpy;
	using Chirpy.Exports;
	using Machine.Specifications;
	using Microsoft.VisualStudio.Shell.Interop;
	using Moq;

	class TaskList_context : AddIn_context
	{
		protected static TaskList TaskList;
		protected static Mock<IServiceProvider> ServiceProviderMock;
		protected static Mock<IErrorListProvider> ErrorListProviderMock;

		Establish context = () =>
			{
				ServiceProviderMock = new Mock<IServiceProvider>();
				ErrorListProviderMock = new Mock<IErrorListProvider>();

				TaskList = new TaskList
				           	{
				           		App = AppMock.Object,
				           		ServiceProvider = ServiceProviderMock.Object,
				           		ErrorListProvider = ErrorListProviderMock.Object
				           	};

				ServiceProviderMock
					.Setup(p => p.GetService(typeof (IVsSolution)))
					.Returns(VsSolutionMock.Object);

				TaskList.OnImportsSatisfied();
			};
	}
}