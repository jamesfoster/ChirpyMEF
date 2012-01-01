namespace ChirpyTest.EngineResolverSpecs
{
	using Chirpy;
	using Chirpy.Exports;
	using ChirpyInterface;
	using Machine.Specifications;

	[Subject(typeof (EngineResolver))]
	public class When_resolving_an_engine_by_name : EngineResolver_context
	{
		static IEngine result;

		Establish context = () =>
			{
				AddEngine("DemoEngine1", "1.0", "a");
				AddEngine("DemoEngine2", "1.0", "b");
			};

		Because of = () => { result = engineResolver.GetEngineByName("DemoEngine2"); };

		It should_not_be_null = () => result.ShouldNotBeNull();
		It should_be_DemoEngine1 = () => ((EngineContainer)result).Name.ShouldEqual("DemoEngine2");
	}
}