namespace ChirpyTest.EngineResolverSpecs
{
	using System;
	using System.Collections.Generic;
	using Chirpy.Exports;
	using Chirpy.Imports;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;

	public class EngineResolver_context
	{
		protected static EngineResolver EngineResolver;
		protected static Mock<IExtensionResolver> ExtensionResolverMock;
		static List<Lazy<IEngine, IEngineMetadata>> engines;
		static Dictionary<string, string> extensions;

		Establish context = () =>
			{
				engines = new List<Lazy<IEngine, IEngineMetadata>>();
				extensions = new Dictionary<string, string>();
				ExtensionResolverMock = new Mock<IExtensionResolver>();

				EngineResolver = new EngineResolver {Engines = engines, ExtensionResolver = ExtensionResolverMock.Object};

				ExtensionResolverMock
					.Setup(r => r.GetExtensionFromCategory(Moq.It.IsAny<string>()))
					.Returns<string>(s => extensions.ContainsKey(s) ? extensions[s] : null);
			};

		protected static void AddCategory(string category, string extension)
		{
			extensions[category] = extension;
		}

		protected static Mock<IEngine> AddEngine(string name, string version, string category)
		{
			return AddEngine(name, version, category, false);
		}

		protected static Mock<IEngine> AddEngine(string name, string version, string category, bool @internal)
		{
			var engineMock = new Mock<IEngine>();
			var metadata = new EngineMetadataAttribute(name, version, category, @internal);

			engines.Add(new Lazy<IEngine, IEngineMetadata>(() => engineMock.Object, metadata));

			return engineMock;
		}
	}
}