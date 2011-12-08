namespace ChirpyTest.EngineSpecs.ConfigChirpyEngineSpecs
{
	using Chirpy.Engines;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;

	public class ConfigChirpyEngine_context
	{
		protected static IChirpyEngine Engine;
		protected static Mock<IEngineResolver> EngineResolverMock;
		protected static Mock<IFileHandler> FileHandlerMock;
		protected static Mock<IProjectItemManager> ProjectItemManagerMock;
		protected static Mock<ITaskList> TaskListMock;
		protected static string Contents;
		protected static string Filename;
		protected static string Result;

		Establish context = () =>
			{
				var configEngine = new ConfigChirpyEngine();
				Engine = configEngine;

				EngineResolverMock = new Mock<IEngineResolver>();
				FileHandlerMock = new Mock<IFileHandler>();
				ProjectItemManagerMock = new Mock<IProjectItemManager>();
				TaskListMock = new Mock<ITaskList>();

				configEngine.EngineResolver = EngineResolverMock.Object;
				configEngine.FileHandler = FileHandlerMock.Object;
				configEngine.ProjectItemManager = ProjectItemManagerMock.Object;
				configEngine.TaskList = TaskListMock.Object;

				FileHandlerMock
					.Setup(h => h.GetFileName(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns<string, string>((f, s) => f);

				FileHandlerMock
					.Setup(h => h.GetContents(Moq.It.IsAny<string>()))
					.Returns<string>(f => string.Format("contents of '{0}'", f));
			};
	}
}