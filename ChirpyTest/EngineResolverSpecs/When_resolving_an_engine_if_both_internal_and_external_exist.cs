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
	public class When_resolving_an_engine_if_both_internal_and_external_exist : EngineResolver_context
	{
		static IEngine result;

		Establish context = () =>
			{
				AddEngine("DemoEngine", "1.0", "cat", "txt", true);
				AddEngine("DemoEngine", "1.0", "cat", "txt", false);

				AddCategory("cat", ".cat");
			};

		Because of = () => { result = engineResolver.GetEngineByFilename("demo.cat"); };

		It should_not_be_null = () => result.ShouldNotBeNull();
		It should_be_an_EngineContainer = () => result.ShouldBeOfType<EngineContainer>();
		It should_contain_2_engines = () => ((EngineContainer) result).Engines.Count().ShouldEqual(2);
		It should_contain_a_DemoEngine = () => ((EngineContainer) result).Name.ShouldEqual("DemoEngine");

		It should_not_evaluate_the_Lazy_objects = () =>
			((EngineContainer) result).Engines.ShouldEachConformTo(e => !e.IsValueCreated);

		It should_call_ExtensionResolver_GetExtensionFromCategory_cat = () =>
			extensionResolverMock.Verify(r => r.GetExtensionFromCategory("cat"), Times.Once());
	}
}