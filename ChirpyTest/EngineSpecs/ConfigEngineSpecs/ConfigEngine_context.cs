namespace ChirpyTest.EngineSpecs.ConfigChirpyEngineSpecs
{
	using System.Collections.Generic;
	using Chirpy.Engines;
	using ChirpyInterface;
	using Machine.Specifications;

	public class ConfigEngine_context
	{
		protected static IEngine Engine;
		protected static string Contents;
		protected static string Filename;
		protected static List<EngineResult> Result;

		Establish context = () =>
			{
				FileHandlerContext.context();
				EngineResolverContext.context();

				Engine = new ConfigEngine
				         	{
				         		EngineResolver = EngineResolverContext.PublicMock.Object,
				         		FileHandler = FileHandlerContext.Mock.Object
				         	};
			};
	}
}