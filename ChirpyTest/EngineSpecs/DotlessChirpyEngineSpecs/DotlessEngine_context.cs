namespace ChirpyTest.EngineSpecs.DotlessChirpyEngineSpecs
{
	using Chirpy.Engines;
	using ChirpyInterface;
	using Machine.Specifications;

	public class DotlessEngine_context
	{
		protected static IEngine Engine;
		protected static string Contents;
		protected static string Filename;

		Establish context = () =>
			{
				Engine = new DotlessEngine();
			};
	}
}