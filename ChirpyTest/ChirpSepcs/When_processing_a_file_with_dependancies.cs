namespace ChirpyTest.ChirpSepcs
{
	using System.Collections.Generic;
	using System.Linq;
	using Chirpy;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof (Chirp))]
	public class When_processing_a_file_with_dependancies : Chirp_context
	{
		static Mock<IEngine> engineMock;
		static string dependancyFilename;
		static IEnumerable<FileAssociation> result;

		Establish context = () =>
			{
				engineMock = AddEngine("DemoEngine", "1.0", "def");

				dependancyFilename = "pqr";

				AddFile("ghi", "abc.def");

				engineMock
					.Setup(e => e.Process(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns(new List<EngineResult> {new EngineResult {Contents = "jkl", Extension = "xyz"}});

				engineMock
					.Setup(e => e.GetDependancies(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns(new List<string> {dependancyFilename});

				Chirp.CheckDependancies(ProjectItemMocks["abc.def"].Object);
			};

		Because of = () => { result = Chirp.RunDependancies(dependancyFilename); };

		It should_call_EngineResolver_GetEngines = () => EngineResolverMock.Verify(r => r.GetEngineByFilename("abc.def"));
		It should_call_Engine_Process = () => engineMock.Verify(e => e.Process("ghi", "abc.def"));
		It should_get_the_contents_of_the_file = () => FileHandlerMock.Verify(h => h.GetContents("abc.def"));
		It should_save_the_output_to_the_output_file = () => FileHandlerMock.Verify(h => h.SaveFile("abc.xyz", "jkl"));

		It should_return_1_FileAssociation = () => result.Count().ShouldEqual(1);
		It FileAssociation_Parent_should_be_the_ProjectItem = () => result.ElementAt(0).Parent.ShouldBeTheSameAs(ProjectItemMocks["abc.def"].Object);
		It FileAssociation_FileName_should_be_the_output_file_name = () => result.ElementAt(0).FileName.ShouldEqual("abc.xyz");
	}
}