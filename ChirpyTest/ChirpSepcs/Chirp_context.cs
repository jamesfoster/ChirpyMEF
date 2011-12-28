namespace ChirpyTest.ChirpSepcs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Chirpy;
	using Chirpy.Imports;
	using ChirpyInterface;
	using EnvDTE;
	using Machine.Specifications;
	using Moq;

	public class Chirp_context
	{
		protected static Chirp Chirp;
		protected static Mock<IEngineResolver> EngineResolverMock;
		protected static Mock<IExtensionResolver> ExtensionResolverMock;
		protected static Mock<ITaskList> TaskListMock;
		protected static Mock<IFileHandler> FileHandlerMock;
		protected static Mock<ProjectItem> ProjectItemMock;
		protected static string Filename;
		protected static string Category;
		protected static string SubCategory;
		static IList<Lazy<IEngine, IEngineMetadata>> engines;
		static IDictionary<string, string> files;

		Establish context = () =>
			{
				EngineResolverMock = new Mock<IEngineResolver>();
				ExtensionResolverMock = new Mock<IExtensionResolver>();
				FileHandlerMock = new Mock<IFileHandler>();
				TaskListMock = new Mock<ITaskList>();
				ProjectItemMock = new Mock<ProjectItem>();

				engines = new List<Lazy<IEngine, IEngineMetadata>>();
				files  = new Dictionary<string, string>();

				FileHandlerMock
					.Setup(h => h.GetContents(Moq.It.IsAny<string>()))
					.Returns<string>(s => files.ContainsKey(s) ? files[s] : null);

				Chirp = new Chirp(EngineResolverMock.Object, TaskListMock.Object, FileHandlerMock.Object, ExtensionResolverMock.Object);

				EngineResolverMock
					.Setup(r => r.GetEngine(Moq.It.IsAny<string>()))
					.Returns<string>(
						cat => new EngineContainer(
							engines
							.Where(e => e.Metadata.Category.Equals(cat, StringComparison.InvariantCultureIgnoreCase))
							));

				EngineResolverMock
					.Setup(r => r.GetEngineByFilename(Moq.It.IsAny<string>()))
					.Returns<string>(
						fn =>
							{
								var cat = fn.Substring(fn.IndexOf('.') + 1);
								return new EngineContainer(
									engines.Where(e => e.Metadata.Category.Equals(cat, StringComparison.InvariantCultureIgnoreCase))
									);
							});

				ExtensionResolverMock
					.Setup(r => r.GetExtensionFromCategory(Moq.It.IsAny<string>()))
					.Returns<string>(s => "." + s);

				ProjectItemMock
					.Setup(pi => pi.get_FileNames(Moq.It.IsAny<short>()))
					.Returns<short>(s => Filename);
			};

		protected static void AddFile(string contents, string filename)
		{
			files[filename] = contents;
		}

		protected static Mock<IEngine> AddEngine(string name, string version, string category, string outputCategory)
		{
			var engineMock = new Mock<IEngine>();
			var metadata = new EngineMetadataAttribute(name, version, category);

			engines.Add(new Lazy<IEngine, IEngineMetadata>(() => engineMock.Object, metadata));

			return engineMock;
		}
	}
}