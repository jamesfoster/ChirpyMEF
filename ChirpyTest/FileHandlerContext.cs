namespace ChirpyTest
{
	using System.Collections.Generic;
	using System.IO;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;

	public class FileHandlerContext
	{
		public static Mock<IFileHandler> Mock;
		static IDictionary<string, string> files;

		public static Establish context = () =>
			{
				Mock = new Mock<IFileHandler>();
				files = new Dictionary<string, string>();

				Mock
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

				Mock
					.Setup(h => h.GetContents(Moq.It.IsAny<string>()))
					.Returns<string>(s => files.ContainsKey(s) ? files[s] : null);

				Mock
					.Setup(h => h.FileExists(Moq.It.IsAny<string>()))
					.Returns<string>(s => files.ContainsKey(s));
			};

		public static void AddFile(string contents, string filename)
		{
			files[filename] = contents;
		}
	}
}