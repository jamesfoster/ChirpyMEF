namespace ChirpyTest.EngineResolverSpecs
{
	using System.Linq;
	using Chirpy;
	using Chirpy.Exports;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof (EngineResolver))]
	public class When_resolving_an_engine_if_both_internal_and_external_exist : EngineResolver_context
	{
		static EngineContainer result;

		Establish context = () =>
			{
				AddEngine("DemoEngine", "1.0", "cat", true);
				AddEngine("DemoEngine", "1.0", "cat", false);

				AddCategory("cat", ".cat");
			};

		Because of = () => { result = EngineResolver.GetEngineByFilename("demo.cat"); };

		It should_not_be_null = () => result.ShouldNotBeNull();
		It should_contain_2_engines = () => result.Engines.Count().ShouldEqual(2);
		It should_not_evaluate_the_engines = () => result.Engines.ShouldEachConformTo(e => !e.IsValueCreated);
		It should_contain_a_DemoEngine = () => result.Name.ShouldEqual("DemoEngine");
		It should_have_both_internal_and_external = () => result.Internal.ShouldContainOnly(true, false);

		It should_call_ExtensionResolver_GetExtensionFromCategory_cat = () =>
			ExtensionResolverMock.Verify(r => r.GetExtensionFromCategory("cat"), Times.Once());
	}
}