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
				engineMock = AddEngine("DemoEngine", "abc.def", "xyz");

				Category = "abc.def";
				Filename = "jkl.abc.def";

				AddFile("ghi", Filename);

				engineMock
					.Setup(e => e.Process(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns("mno");
			};

		Because of = () => Chirp.Run(Filename);

		It should_call_EngineResolver_GetEngines = () => EngineResolverMock.Verify(r => r.GetEngineForFile("jkl.abc.def"));
		It should_call_Engine_Process = () => engineMock.Verify(e => e.Process("ghi", "jkl.abc.def"));
	}
}
