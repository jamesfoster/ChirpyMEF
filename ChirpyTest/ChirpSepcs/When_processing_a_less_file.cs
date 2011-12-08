namespace ChirpyTest
{
	using Chirpy;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof (Chirp))]
	public class When_processing_a_file : Chirp_context
	{
		static Mock<IChirpyEngine> engineMock;

		Establish context = () =>
			{
				engineMock = AddEngine("DemoEngine", "abc", "def");

				Category = "abc";
				SubCategory = "def";
				Contents = "ghi";
				Filename = "jkl";

				engineMock
					.Setup(e => e.Process(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns("mno");
			};

		Because of = () => { Result = Chirp.Run(Category, SubCategory, Contents, Filename); };

		It should_call_EngineResolver_GetEngines = () => EngineResolverMock.Verify(r => r.GetEngines("abc", "def"));
		It should_call_Engine_Process = () => engineMock.Verify(e => e.Process("ghi", "jkl"));

		It should_return_the_output_of_the_engine = () => Result.ShouldEqual("mno");
	}
}
