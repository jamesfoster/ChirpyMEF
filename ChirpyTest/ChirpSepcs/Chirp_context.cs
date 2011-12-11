namespace ChirpyTest.ChirpSepcs
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Chirpy;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;

	public class Chirp_context
	{
		protected static Chirp Chirp;
		protected static Mock<IEngineResolver> EngineResolverMock;
		protected static Mock<ITaskList> TaskListMock;
		protected static Mock<IProjectItemManager> ProjectItemManagerMock;
		protected static Mock<IFileHandler> FileHandlerMock;
		protected static string Filename;
		protected static string Category;
		protected static string SubCategory;
		protected static string Result;
		static IList<Lazy<IChirpyEngine, IChirpyEngineMetadata>> engines;
		static IDictionary<string, string> files;

		Establish context = () =>
			{
				EngineResolverMock = new Mock<IEngineResolver>();
				FileHandlerMock = new Mock<IFileHandler>();
				TaskListMock = new Mock<ITaskList>();
				ProjectItemManagerMock = new Mock<IProjectItemManager>();

				engines = new List<Lazy<IChirpyEngine, IChirpyEngineMetadata>>();
				files  = new Dictionary<string, string>();

				FileHandlerMock
					.Setup(h => h.GetContents(Moq.It.IsAny<string>()))
					.Returns<string>(s => files.ContainsKey(s) ? files[s] : null);

				Chirp = new Chirp(EngineResolverMock.Object, TaskListMock.Object, ProjectItemManagerMock.Object, FileHandlerMock.Object);

				EngineResolverMock
					.Setup(r => r.GetEngine(Moq.It.IsAny<string>()))
					.Returns<string>(
						cat => new LazyMefEngine(
							engines
							.Where(e => e.Metadata.Category.Equals(cat, StringComparison.InvariantCultureIgnoreCase))
							));
			};

		protected static void AddFile(string contents, string filename)
		{
			files[filename] = contents;
		}

		protected static Mock<IChirpyEngine> AddEngine(string name, string category, string outputCategory)
		{
			var engineMock = new Mock<IChirpyEngine>();
			var metadata = new ChirpyEngineMetadataAttribute(name, category, outputCategory);

			engines.Add(new Lazy<IChirpyEngine, IChirpyEngineMetadata>(() => engineMock.Object, metadata));

			return engineMock;
		}
	}
}