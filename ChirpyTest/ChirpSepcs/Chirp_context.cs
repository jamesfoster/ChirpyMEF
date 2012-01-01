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
		protected static Mock<IEngineResolver> EngineResolverMock;
		protected static Mock<IExtensionResolver> ExtensionResolverMock;
		protected static Mock<ITaskList> TaskListMock;
		protected static Mock<IFileHandler> FileHandlerMock;
		protected static Mock<ILogger> LoggerMock;
		protected static IDictionary<string, Mock<ProjectItem>> ProjectItemMocks;
		static IList<Lazy<IEngine, IEngineMetadata>> engines;
		static IDictionary<string, string> files;

		Establish context = () =>
			{
				EngineResolverMock = new Mock<IEngineResolver>();
				ExtensionResolverMock = new Mock<IExtensionResolver>();
				TaskListMock = new Mock<ITaskList>();
				LoggerMock = new Mock<ILogger>();
				FileHandlerMock = new Mock<IFileHandler>();
				ProjectItemMocks = new Dictionary<string, Mock<ProjectItem>>();

				engines = new List<Lazy<IEngine, IEngineMetadata>>();
				files  = new Dictionary<string, string>();

				FileHandlerMock
					.Setup(h => h.GetContents(Moq.It.IsAny<string>()))
					.Returns<string>(s => files.ContainsKey(s) ? files[s] : null);

				FileHandlerMock
					.Setup(h => h.GetAbsoluteFileName(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns<string, string>((path, relativeTo) => path);

				Chirp = new Chirp
				        	{
				        		EngineResolver = EngineResolverMock.Object,
				        		TaskList = TaskListMock.Object,
				        		FileHandler = FileHandlerMock.Object,
				        		ExtensionResolver = ExtensionResolverMock.Object,
				        		Logger = LoggerMock.Object
				        	};

				EngineResolverMock
					.Setup(r => r.GetEngine(Moq.It.IsAny<string>()))
					.Returns<string>(
						cat =>
							{
								var es = engines.Where(e => e.Metadata.Category.Equals(cat, StringComparison.InvariantCultureIgnoreCase));
								if(!es.Any())
									return null;

								return new EngineContainer(es);
							});

				EngineResolverMock
					.Setup(r => r.GetEngineByFilename(Moq.It.IsAny<string>()))
					.Returns<string>(
						fn =>
							{
								var cat = fn.Substring(fn.IndexOf('.') + 1);
								var es = engines.Where(e => e.Metadata.Category.Equals(cat, StringComparison.InvariantCultureIgnoreCase));
								if(!es.Any())
									return null;

								return new EngineContainer(es);
							});

				ExtensionResolverMock
					.Setup(r => r.GetExtensionFromCategory(Moq.It.IsAny<string>()))
					.Returns<string>(s => "." + s);
			};

		protected static void AddFile(string contents, string filename)
		{
			files[filename] = contents;

			var projectItemMock = new Mock<ProjectItem>();
			projectItemMock
				.Setup(pi => pi.get_FileNames(Moq.It.IsAny<short>()))
				.Returns<short>(s => filename);

			ProjectItemMocks[filename] = projectItemMock;
		}

		protected static Mock<IEngine> AddEngine(string name, string version, string category)
		{
			var engineMock = new Mock<IEngine>();
			var metadata = new EngineMetadataAttribute(name, version, category);

			engines.Add(new Lazy<IEngine, IEngineMetadata>(() => engineMock.Object, metadata));

			return engineMock;
		}
	}
}