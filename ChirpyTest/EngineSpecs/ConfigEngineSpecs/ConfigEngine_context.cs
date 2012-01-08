namespace ChirpyTest.EngineSpecs.ConfigChirpyEngineSpecs
{
	using System.Collections.Generic;
	using Chirpy.Engines;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;

	public class ConfigEngine_context : FileHandler_context
	{
		protected static IEngine Engine;
		protected static Mock<IEngineResolver> EngineResolverMock;
		protected static string Contents;
		protected static string Filename;
		protected static List<EngineResult> Result;

		Establish context = () =>
			{
				EngineResolverMock = new Mock<IEngineResolver>();

				Engine = new ConfigEngine
				         	{
				         		EngineResolver = EngineResolverMock.Object,
				         		FileHandler = FileHandlerMock.Object
				         	};
			};
	}
}