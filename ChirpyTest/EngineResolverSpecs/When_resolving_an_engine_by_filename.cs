namespace ChirpyTest.EngineResolverSpecs
{
	using Chirpy;
	using Chirpy.Exports;
	using Machine.Specifications;
	using It = Machine.Specifications.It;

	[Subject(typeof (EngineResolver))]
	public class When_resolving_an_engine_by_filename : EngineResolver_context
	{
		static EngineContainer result;

		Establish context = () =>
			{
				AddEngine("DemoEngine", "1.0", "cat");

				AddCategory("cat", ".cat");
			};

		Because of = () => { result = EngineResolver.GetEngineByFilename("demo.cat"); };

		It should_not_be_null = () => result.ShouldNotBeNull();

		It should_call_ExtensionResolver_GetExtensionFromCategory = () =>
			ExtensionResolverMock.Verify(r => r.GetExtensionFromCategory("cat"));
	}
}