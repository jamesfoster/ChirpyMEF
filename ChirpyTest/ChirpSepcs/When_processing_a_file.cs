namespace ChirpyTest.ChirpSepcs
{
	using System.Collections.Generic;
	using Chirpy;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof (Chirp))]
	public class When_processing_a_file : Chirp_context
	{
		static Mock<IEngine> engineMock;

		Establish context = () =>
			{
				engineMock = AddEngine("DemoEngine", "1.0", "abc.def", "xyz");

				Category = "abc.def";
				Filename = "jkl.abc.def";

				AddFile("ghi", Filename);

				engineMock
					.Setup(e => e.Process(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns(new List<EngineResult> {new EngineResult {Contents = "mno"}});
			};

		Because of = () => Chirp.Run(ProjectItemMock.Object);

		It should_call_EngineResolver_GetEngines = () => EngineResolverMock.Verify(r => r.GetEngineByFilename("jkl.abc.def"));
		It should_call_Engine_Process = () => engineMock.Verify(e => e.Process("ghi", "jkl.abc.def"));
	}
}
