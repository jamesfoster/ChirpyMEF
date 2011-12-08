namespace ChirpyTest.ChirpSepcs
{
	using System;
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
		protected static string Contents;
		protected static string Filename;
		protected static string Category;
		protected static string SubCategory;
		protected static string Result;
		static IList<Lazy<IChirpyEngine, IChirpyEngineMetadata>> engines;

		Establish context = () =>
			{
				EngineResolverMock = new Mock<IEngineResolver>();

				engines = new List<Lazy<IChirpyEngine, IChirpyEngineMetadata>>();

				Chirp = new Chirp(EngineResolverMock.Object);

				EngineResolverMock
					.Setup(r => r.GetEngine(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns<string, string>(
						(cat, subCat) => new LazyMefEngine(
							engines
							.Where(e => e.Metadata.Category.Equals(cat, StringComparison.InvariantCultureIgnoreCase))
							.Where(e => e.Metadata.SubCategory.Equals(subCat, StringComparison.InvariantCultureIgnoreCase))
							));
			};

		protected static Mock<IChirpyEngine> AddEngine(string name, string category, string subCategory = "")
		{
			var engineMock = new Mock<IChirpyEngine>();
			var metadata = new ChirpyEngineMetadataAttribute(name, category, subCategory);

			engines.Add(new Lazy<IChirpyEngine, IChirpyEngineMetadata>(() => engineMock.Object, metadata));

			return engineMock;
		}
	}
}