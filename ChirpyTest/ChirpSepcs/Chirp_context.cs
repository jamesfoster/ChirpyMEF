namespace ChirpyTest.ChirpSepcs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Chirpy;
	using Chirpy.Imports;
	using Chirpy.Logging;
	using ChirpyInterface;
	using EnvDTE;
	using Machine.Specifications;
	using Moq;

	public class Chirp_context
	{
		protected static Chirp Chirp;
		protected static Mock<IExtensionResolver> ExtensionResolverMock;
		protected static Mock<ITaskList> TaskListMock;
		protected static Mock<ILogger> LoggerMock;
		protected static IDictionary<string, Mock<ProjectItem>> ProjectItemMocks;

		Establish context = () =>
			{
				FileHandlerContext.context();
				EngineResolverContext.context();

				ExtensionResolverMock = new Mock<IExtensionResolver>();
				TaskListMock = new Mock<ITaskList>();
				LoggerMock = new Mock<ILogger>();
				ProjectItemMocks = new Dictionary<string, Mock<ProjectItem>>();


				Chirp = new Chirp
				        	{
				        		EngineResolver = EngineResolverContext.Mock.Object,
				        		TaskList = TaskListMock.Object,
				        		FileHandler = FileHandlerContext.Mock.Object,
				        		ExtensionResolver = ExtensionResolverMock.Object,
				        		Logger = LoggerMock.Object
				        	};

				ExtensionResolverMock
					.Setup(r => r.GetExtensionFromCategory(Moq.It.IsAny<string>()))
					.Returns<string>(s => "." + s);
			};

		protected static void AddProjectItem(string contents, string filename)
		{
			FileHandlerContext.AddFile(contents, filename);

			var projectItemMock = new Mock<ProjectItem>();
			projectItemMock
				.Setup(pi => pi.get_FileNames(Moq.It.IsAny<short>()))
				.Returns<short>(s => filename);

			ProjectItemMocks[filename] = projectItemMock;
		}
	}
}