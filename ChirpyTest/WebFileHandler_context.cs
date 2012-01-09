namespace ChirpyTest
{
	using System.Collections.Generic;
	using System.IO;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;

	public class WebFileHandler_context
	{
		protected static Mock<IWebFileHandler> WebFileHandlerMock;
		static IDictionary<string, string> files;

		Establish context = () =>
			{
				WebFileHandlerMock = new Mock<IWebFileHandler>();
				files = new Dictionary<string, string>();

				WebFileHandlerMock
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

				WebFileHandlerMock
					.Setup(h => h.GetContents(Moq.It.IsAny<string>()))
					.Returns<string>(s => files.ContainsKey(s) ? files[s] : null);
			};

		protected static void AddFile(string contents, string filename)
		{
			files[filename] = contents;
		}
	}
}