namespace ChirpyTest.EngineResolverSpecs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Chirpy.Exports;
	using Chirpy.Imports;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;

	public class EngineResolver_context
	{
		protected static EngineResolver engineResolver;
		protected static Mock<IExtensionResolver> extensionResolverMock;
		static List<Lazy<IEngine, IEngineMetadata>> engines;
		static Dictionary<string, string> extensions;

		Establish context = () =>
			{
				engines = new List<Lazy<IEngine, IEngineMetadata>>();
				extensions = new Dictionary<string, string>();
				extensionResolverMock = new Mock<IExtensionResolver>();

				engineResolver = new EngineResolver {Engines = engines, ExtensionResolver = extensionResolverMock.Object};

				extensionResolverMock
					.Setup(r => r.GetCategoryFromExtension(Moq.It.IsAny<string>()))
					.Returns<string>(s => extensions.ContainsValue(s) ? extensions.FirstOrDefault(e => e.Value == s).Key : null);

				extensionResolverMock
					.Setup(r => r.GetExtensionFromCategory(Moq.It.IsAny<string>()))
					.Returns<string>(s => extensions.ContainsKey(s) ? extensions[s] : null);
			};

		protected static void AddCategory(string category, string extension)
		{
			extensions[category] = extension;
		}

		protected static Mock<IEngine> AddEngine(string name, string category, string outputCategory)
		{
			return AddEngine(name, category, outputCategory, false);
		}

		protected static Mock<IEngine> AddEngine(string name, string category, string outputCategory, bool @internal)
		{
			var engineMock = new Mock<IEngine>();
			var metadata = new EngineMetadataAttribute(name, category, outputCategory, @internal);

			engines.Add(new Lazy<IEngine, IEngineMetadata>(() => engineMock.Object, metadata));

			return engineMock;
		}
	}
}