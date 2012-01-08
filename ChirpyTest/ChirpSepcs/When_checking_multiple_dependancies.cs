namespace ChirpyTest.ChirpSepcs
{
	using System.Collections.Generic;
	using Chirpy;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof (Chirp))]
	public class When_checking_multiple_dependancies : Chirp_context
	{
		static Mock<IEngine> engineMock;

		Establish context = () =>
			{
				engineMock = AddEngine("DemoEngine", "1.0", "abc");

				AddProjectItem("def", "file1.abc");
				AddProjectItem("def", "file2.abc");
				AddProjectItem("def", "file3.abc");

				engineMock
					.Setup(e => e.GetDependancies(Moq.It.IsAny<string>(), "file1.abc"))
					.Returns(new List<string> {"x", "y", "z"});

				engineMock
					.Setup(e => e.GetDependancies(Moq.It.IsAny<string>(), "file2.abc"))
					.Returns(new List<string> {"x", "z"});

				engineMock
					.Setup(e => e.GetDependancies(Moq.It.IsAny<string>(), "file3.abc"))
					.Returns(new List<string> {"z"});
			};

		Because of = () =>
			{
				Chirp.CheckDependancies(ProjectItemMocks["file1.abc"].Object);
				Chirp.CheckDependancies(ProjectItemMocks["file2.abc"].Object);
				Chirp.CheckDependancies(ProjectItemMocks["file3.abc"].Object);
			};

		It should_get_the_engine_for_file1 = () => EngineResolverMock.Verify(r => r.GetEngineByFilename("file1.abc"));
		It should_get_the_engine_for_file2 = () => EngineResolverMock.Verify(r => r.GetEngineByFilename("file2.abc"));
		It should_get_the_engine_for_file3 = () => EngineResolverMock.Verify(r => r.GetEngineByFilename("file3.abc"));
		It should_call_Engine_GetDependancies_for_file1 = () => engineMock.Verify(e => e.GetDependancies("def", "file1.abc"));
		It should_call_Engine_GetDependancies_for_file2 = () => engineMock.Verify(e => e.GetDependancies("def", "file2.abc"));
		It should_call_Engine_GetDependancies_for_file3 = () => engineMock.Verify(e => e.GetDependancies("def", "file3.abc"));
		It should_get_the_contents_of_file1 = () => FileHandlerMock.Verify(h => h.GetContents("file1.abc"));
		It should_get_the_contents_of_file2 = () => FileHandlerMock.Verify(h => h.GetContents("file2.abc"));
		It should_get_the_contents_of_file3 = () => FileHandlerMock.Verify(h => h.GetContents("file3.abc"));

		It should_have_3_dependancies = () => Chirp.Dependancies.Count.ShouldEqual(3);

		It dependancy_x_should_affect_file1_and_file2 = () =>
			Chirp.Dependancies["x"].ShouldContainOnly(
				ProjectItemMocks["file1.abc"].Object,
				ProjectItemMocks["file2.abc"].Object
				);

		It dependancy_y_should_affect_file1 = () =>
			Chirp.Dependancies["y"].ShouldContainOnly(
				ProjectItemMocks["file1.abc"].Object
				);

		It dependancy_z_should_affect_file1_file2_and_file3 = () =>
			Chirp.Dependancies["z"].ShouldContainOnly(
				ProjectItemMocks["file1.abc"].Object,
				ProjectItemMocks["file2.abc"].Object,
				ProjectItemMocks["file3.abc"].Object
				);
	}
}