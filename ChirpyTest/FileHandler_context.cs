namespace ChirpyTest
{
	using System.Collections.Generic;
	using System.IO;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;

	public class FileHandler_context
	{
		protected static Mock<IFileHandler> FileHandlerMock;
		static IDictionary<string, string> files;

		Establish context = () =>
			{
				FileHandlerMock = new Mock<IFileHandler>();
				files = new Dictionary<string, string>();

				FileHandlerMock
					.Setup(h => h.GetAbsoluteFileName(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns<string, string>((path, relativeTo) =>
						{
							if(string.IsNullOrEmpty(relativeTo))
								return path;

							var relDirectory = Path.GetDirectoryName(relativeTo);

							if (relDirectory == null)
								return path;

							return Path.Combine(relDirectory, path).Replace('\\', '/');
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