namespace ChirpyTest.ChirpSepcs
{
	using Chirpy;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof (Chirp))]
	public class When_processing_a_file_with_no_matching_engine : Chirp_context
	{
		static Mock<IEngine> engineMock;

		Establish context = () =>
			{
				engineMock = AddEngine("DemoEngine", "xxx.xxx", "xyz");

				Category = "abc.def";
				Filename = "jkl.abc.def";

				AddFile("ghi", Filename);
			};

		Because of = () => Chirp.Run(Filename);

		It should_call_EngineResolver_GetEngines = () => EngineResolverMock.Verify(r => r.GetEngineForFile("jkl.abc.def"));
		It should_not_call_Engine_Process = () =>
			engineMock.Verify(e => e.Process(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()), Times.Never());
	}
}