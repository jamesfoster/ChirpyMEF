namespace ChirpyTest.EngineSpecs.ConfigChirpyEngineSpecs
{
	using System.Collections.Generic;
	using System.IO;
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
		static IDictionary<string, string> files;

		Establish context = () =>
			{
				EngineResolverMock = new Mock<IEngineResolver>();
				FileHandlerMock = new Mock<IFileHandler>();
				files = new Dictionary<string, string>();

				Engine = new ConfigEngine
				         	{
				         		EngineResolver = EngineResolverMock.Object,
				         		FileHandler = FileHandlerMock.Object
				         	};

				FileHandlerMock
					.Setup(h => h.GetAbsoluteFileName(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns<string, string>((path, relativeTo) =>
						{
							var directory = Path.GetDirectoryName(relativeTo);
							return Path.Combine(directory, path).Replace('\\', '/');
						});

				FileHandlerMock
					.Setup(h => h.GetContents(Moq.It.IsAny<string>()))
					.Returns<string>(s => files.ContainsKey(s) ? files[s] : null);

				FileHandlerMock
					.Setup(h => h.FileExists(Moq.It.IsAny<string>()))
					.Returns<string>(s => files.ContainsKey(s));
			};

		protected static void AddFile(string contents, string filename)
		{
			files[filename] = contents;
		}
	}
}