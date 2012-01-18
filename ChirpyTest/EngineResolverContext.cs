namespace ChirpyTest
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Chirpy;
	using Chirpy.Imports;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;

	public static class EngineResolverContext
	{
		public static Mock<IInternalEngineResolver> Mock;
		public static Mock<IEngineResolver> PublicMock;
		static IList<Lazy<IEngine, IEngineMetadata>> engines;

		public static Establish context = () =>
			{
				Mock = new Mock<IInternalEngineResolver>();
				PublicMock = new Mock<IEngineResolver>();
				engines = new List<Lazy<IEngine, IEngineMetadata>>();

				Mock
					.Setup(r => r.GetEngine(Moq.It.IsAny<string>()))
					.Returns<string>(
						cat =>
							{
								var es = engines.Where(e => e.Metadata.Category.Equals(cat, StringComparison.InvariantCultureIgnoreCase));
								if(!es.Any())
									return null;

								return new EngineContainer(es);
							});

				Mock
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

				PublicMock
					.Setup(r => r.GetEngine(Moq.It.IsAny<string>()))
					.Returns<string>(cat => Mock.Object.GetEngine(cat));

				PublicMock
					.Setup(r => r.GetEngineByFilename(Moq.It.IsAny<string>()))
					.Returns<string>(fn => Mock.Object.GetEngineByFilename(fn));
			};

		public static Mock<IEngine> AddEngine(string name, string version, string category)
		{
			var engineMock = new Mock<IEngine>();
			var metadata = new EngineMetadataAttribute(name, version, category);

			engines.Add(new Lazy<IEngine, IEngineMetadata>(() => engineMock.Object, metadata));

			return engineMock;
		}
	}
}