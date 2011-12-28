namespace ChirpyTest.EngineContainerSpecs
{
	using System;
	using System.Collections.Generic;
	using Chirpy;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;

	public class EngineContainer_context
	{
		protected static EngineContainer EngineContainer;
		protected static List<Lazy<IEngine, IEngineMetadata>> Engines;

		Establish context = () =>
			{
				Engines = new List<Lazy<IEngine, IEngineMetadata>>();
			};

		protected static Mock<IEngine> AddEngine(string name, string version, string category, string outputCategory)
		{
			return AddEngine(name, version, category, outputCategory, false);
		}

		protected static Mock<IEngine> AddEngine(string name, string version, string category, string outputCategory, bool @internal)
		{
			var engineMock = new Mock<IEngine>();
			var metadata = new EngineMetadataAttribute(name, version, category, @internal);

			Engines.Add(new Lazy<IEngine, IEngineMetadata>(() => engineMock.Object, metadata));

			return engineMock;
		}
	}
}