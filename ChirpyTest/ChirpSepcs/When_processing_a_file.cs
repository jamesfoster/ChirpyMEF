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
	public class When_processing_a_file : Chirp_context
	{
		static Mock<IEngine> engineMock;
		static IEnumerable<FileAssociation> result;

		Establish context = () =>
			{
				engineMock = AddEngine("DemoEngine", "1.0", "def");

				AddProjectItem("ghi", "abc.def");

				engineMock
					.Setup(e => e.Process(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns(new List<EngineResult> {new EngineResult {Contents = "jkl", Extension = "xyz"}});
			};

		Because of = () => { result = Chirp.Run(ProjectItemMocks["abc.def"].Object); };

		It should_get_the_engine = () => EngineResolverMock.Verify(r => r.GetEngineByFilename("abc.def"));
		It should_call_Engine_Process = () => engineMock.Verify(e => e.Process("ghi", "abc.def"));
		It should_get_the_contents_of_the_file = () => FileHandlerMock.Verify(h => h.GetContents("abc.def"));
		It should_save_the_output_to_the_output_file = () => FileHandlerMock.Verify(h => h.SaveFile("abc.xyz", "jkl"));

		It should_return_1_FileAssociation = () => result.Count().ShouldEqual(1);
		It FileAssociation_Parent_should_be_the_ProjectItem = () => result.ElementAt(0).Parent.ShouldBeTheSameAs(ProjectItemMocks["abc.def"].Object);
		It FileAssociation_FileName_should_be_the_output_file_name = () => result.ElementAt(0).FileName.ShouldEqual("abc.xyz");

		It should_log_success = () =>
			LoggerMock.Verify(l => l.Log(Moq.It.IsAny<string>()));

		It the_log_message_should_contain_the_filename = () =>
			LoggerMock.Verify(l => l.Log(Moq.It.Is<string>(s => s.Contains("abc.xyz"))));

		It the_log_message_should_contain_the_engine_name = () =>	
			LoggerMock.Verify(l => l.Log(Moq.It.Is<string>(s => s.Contains("DemoEngine"))));
	}
}
