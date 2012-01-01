namespace ChirpyTest.EngineResolverSpecs
{
	using System.Linq;
	using Chirpy;
	using Chirpy.Exports;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof(EngineResolver))]
	public class When_resolving_an_engine_by_filename_when_subcategory_exists2 : EngineResolver_context
	{
		static EngineContainer result;

		Establish context = () =>
			{
				AddEngine("DemoEngine", "1.0", "cat");
				AddEngine("AwesomeEngine", "1.0", "awesome.cat");

				AddCategory("cat", ".cat");
				AddCategory("awesome.cat", ".awe.cat");
			};

		Because of = () => { result = EngineResolver.GetEngineByFilename("demo.awe.cat"); };

		It should_not_be_null = () => result.ShouldNotBeNull();
		It should_not_evaluate_the_engine = () => result.Engines.First().IsValueCreated.ShouldBeFalse();
		It should_contain_a_DemoEngine = () => result.Name.ShouldEqual("AwesomeEngine");

		It should_call_ExtensionResolver_GetExtensionFromCategory_awesome_cat = () =>
			ExtensionResolverMock.Verify(r => r.GetExtensionFromCategory("awesome.cat"), Times.Once());
	}
}