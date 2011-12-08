namespace ChirpyTest.ChirpSepcs
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
				Filename = "jkl";

				AddFile("ghi", Filename);

				engineMock
					.Setup(e => e.Process(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns("mno");
			};

		Because of = () => Chirp.Run(Category, SubCategory, Filename);

		It should_call_EngineResolver_GetEngines = () => EngineResolverMock.Verify(r => r.GetEngine("abc", "def"));
		It should_call_Engine_Process = () => engineMock.Verify(e => e.Process("ghi", "jkl"));

		It should_return_the_output_of_the_engine; // = () => Result.ShouldEqual("mno");
	}
}
