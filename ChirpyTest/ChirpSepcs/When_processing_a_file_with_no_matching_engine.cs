namespace ChirpyTest.ChirpSepcs
{
	using System.Collections.Generic;
	using Chirpy;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof (Chirp))]
	public class When_processing_a_file_with_no_matching_engine : Chirp_context
	{
		static Mock<IEngine> engineMock;
		static IEnumerable<FileAssociation> result;

		Establish context = () =>
			{
				engineMock = AddEngine("DemoEngine", "1.0", "xxx.xxx");

				AddProjectItem("ghi", "abc.def");
			};

		Because of = () => { result = Chirp.Run(ProjectItemMocks["abc.def"].Object); };

		It should_get_the_engine = () => EngineResolverMock.Verify(r => r.GetEngineByFilename("abc.def"));

		It should_not_call_Engine_Process = () =>
			engineMock.Verify(e => e.Process(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()), Times.Never());

		It should_return_no_FileAssociations = () => result.ShouldBeEmpty();
	}
}