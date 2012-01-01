namespace ChirpyTest.EngineResolverSpecs
{
	using System.Linq;
	using Chirpy;
	using Chirpy.Exports;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof (EngineResolver))]
	public class When_resolving_an_engine_by_filename_when_subcategory_exists1 : EngineResolver_context
	{
		static IEngine result;

		Establish context = () =>
			{
				AddEngine("DemoEngine", "1.0", "cat");
				AddEngine("AwesomeEngine", "1.0", "awesome.cat");

				AddCategory("cat", ".cat");
				AddCategory("awesome.cat", ".awe.cat");
			};

		Because of = () => { result = engineResolver.GetEngineByFilename("demo.cat"); };

		It should_not_be_null = () => result.ShouldNotBeNull();
		It should_be_an_EngineContainer = () => result.ShouldBeOfType<EngineContainer>();
		It should_contain_one_engine = () => ((EngineContainer) result).Engines.Count().ShouldEqual(1);
		It should_contain_a_DemoEngine = () => ((EngineContainer) result).Name.ShouldEqual("DemoEngine");

		It should_not_evaluate_the_Lazy_object = () =>
			((EngineContainer) result).Engines.First().IsValueCreated.ShouldBeFalse();

		It should_call_ExtensionResolver_GetExtensionFromCategory_cat = () =>
			extensionResolverMock.Verify(r => r.GetExtensionFromCategory("cat"), Times.Once());

		It should_call_ExtensionResolver_GetExtensionFromCategory_awesome_cat = () =>
			extensionResolverMock.Verify(r => r.GetExtensionFromCategory("awesome.cat"), Times.Once());
	}
}