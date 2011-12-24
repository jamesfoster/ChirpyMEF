namespace ChirpyTest.EngineSpecs.ConfigChirpyEngineSpecs
{
	using System.Collections.Generic;
	using Chirpy.Engines;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;

	public class ConfigEngine_context
	{
		protected static IEngine Engine;
		protected static Mock<IEngineResolver> EngineResolverMock;
		protected static Mock<IFileHandler> FileHandlerMock;
		protected static string Contents;
		protected static string Filename;
		protected static List<EngineResult> Result;

		Establish context = () =>
			{
				var configEngine = new ConfigEngine();
				Engine = configEngine;

				EngineResolverMock = new Mock<IEngineResolver>();
				FileHandlerMock = new Mock<IFileHandler>();

				configEngine.EngineResolver = EngineResolverMock.Object;
				configEngine.FileHandler = FileHandlerMock.Object;

				FileHandlerMock
					.Setup(h => h.GetAbsoluteFileName(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns<string, string>((f, s) => f);

				FileHandlerMock
					.Setup(h => h.GetContents(Moq.It.IsAny<string>()))
					.Returns<string>(f => string.Format("contents of '{0}'", f));
			};
	}
}