namespace ChirpyTest.EngineContainerSpecs
{
	using System.Linq;
	using Chirpy;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof (EngineContainer))]
	public class When_processing_an_EngineContainer_with_internal_and_external : EngineContainer_context
	{
		static EngineContainer engineContainer;
		static Mock<IEngine> internalEngineMock;
		static Mock<IEngine> externalEngineMock;
		static string contents;
		static string filename;
		static string result;

		Establish context = () =>
			{
				internalEngineMock = AddEngine("DemoEngine", "1.0", "abc", "def", true);
				externalEngineMock = AddEngine("DemoEngine", "1.0", "abc", "def", false);

				internalEngineMock
					.Setup(e => e.Process(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns("internal");

				externalEngineMock
					.Setup(e => e.Process(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns("external");

				engineContainer = new EngineContainer(Engines);

				contents = "ghi";
				filename = "jkl";
			};

		Because of = () => { result = engineContainer.Process(contents, filename); };

		It should_not_call_Process_on_the_internal_engine = () =>
			internalEngineMock.Verify(e => e.Process(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()), Times.Never());

		It should_call_Process_on_the_external_engine = () =>
			externalEngineMock.Verify(e => e.Process(contents, filename), Times.Once());

		It should_not_evaluate_the_internal_engine = () =>
			engineContainer.Engines.First(e => e.Metadata.Internal).IsValueCreated.ShouldBeFalse();

		It should_return_the_output_of_the_external_engine = () => result.ShouldEqual("external");
	}
}