namespace ChirpyTest.EngineResolverSpecs
{
	using Chirpy.Exports;
	using ChirpyInterface;
	using Machine.Specifications;
	using It = Machine.Specifications.It;

	[Subject(typeof (EngineResolver))]
	public class When_resolving_an_engine_by_filename : EngineResolver_context
	{
		static IEngine result;

		Establish context = () =>
			{
				AddEngine("DemoEngine", "cat", "txt");

				AddCategory("cat", ".cat");
			};

		Because of = () => { result = engineResolver.GetEngineByFilename("demo.cat"); };

		It should_not_be_null = () => result.ShouldNotBeNull();

		It should_call_ExtensionResolver_GetExtensionFromCategory = () =>
			extensionResolverMock.Verify(r => r.GetExtensionFromCategory("cat"));
	}
}