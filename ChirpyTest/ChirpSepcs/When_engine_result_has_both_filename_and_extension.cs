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
	public class When_engine_result_has_both_filename_and_extension : Chirp_context
	{
		static Mock<IEngine> engineMock;
		static IEnumerable<FileAssociation> result;

		Establish context = () =>
			{
				engineMock = EngineResolverContext.AddEngine("DemoEngine", "1.0", "foo.js");

				AddProjectItem("ghi", "example.foo.js");

				engineMock
					.Setup(e => e.Process(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns(new List<EngineResult> {new EngineResult {FileName = "example.foo.js", Extension = "bar.js", Contents = "..."}});
			};

		Because of = () => { result = Chirp.Run(ProjectItemMocks["example.foo.js"].Object); };

		It should_return_1_FileAssociation = () =>
			result.Count().ShouldEqual(1);

		It FileAssociation_Parent_should_be_the_ProjectItem = () =>
			result.ElementAt(0).Parent.ShouldBeTheSameAs(ProjectItemMocks["example.foo.js"].Object);

		It FileAssociation_FileName_should_be_the_output_file_name = () =>
			result.ElementAt(0).FileName.ShouldEqual("example.bar.js");
	}
}