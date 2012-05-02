namespace ChirpyTest.ChirpSepcs
{
	using System.Collections.Generic;
	using Chirpy;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof (Chirp))]
	public class When_checking_multiple_dependencies : Chirp_context
	{
		static Mock<IEngine> engineMock;

		Establish context = () =>
			{
				engineMock = EngineResolverContext.AddEngine("DemoEngine", "1.0", "abc");

				AddProjectItem("def", "file1.abc");
				AddProjectItem("def", "file2.abc");
				AddProjectItem("def", "file3.abc");

				engineMock
					.Setup(e => e.GetDependencies(Moq.It.IsAny<string>(), "file1.abc"))
					.Returns(new List<string> {"x", "y", "z"});

				engineMock
					.Setup(e => e.GetDependencies(Moq.It.IsAny<string>(), "file2.abc"))
					.Returns(new List<string> {"x", "z"});

				engineMock
					.Setup(e => e.GetDependencies(Moq.It.IsAny<string>(), "file3.abc"))
					.Returns(new List<string> {"z"});
			};

		Because of = () =>
			{
				Chirp.CheckDependencies(ProjectItemMocks["file1.abc"].Object);
				Chirp.CheckDependencies(ProjectItemMocks["file2.abc"].Object);
				Chirp.CheckDependencies(ProjectItemMocks["file3.abc"].Object);
			};

		It should_get_the_engine_for_file1 = () => EngineResolverContext.Mock.Verify(r => r.GetEngineByFilename("file1.abc"));
		It should_get_the_engine_for_file2 = () => EngineResolverContext.Mock.Verify(r => r.GetEngineByFilename("file2.abc"));
		It should_get_the_engine_for_file3 = () => EngineResolverContext.Mock.Verify(r => r.GetEngineByFilename("file3.abc"));
		It should_call_Engine_GetDependencies_for_file1 = () => engineMock.Verify(e => e.GetDependencies("def", "file1.abc"));
		It should_call_Engine_GetDependencies_for_file2 = () => engineMock.Verify(e => e.GetDependencies("def", "file2.abc"));
		It should_call_Engine_GetDependencies_for_file3 = () => engineMock.Verify(e => e.GetDependencies("def", "file3.abc"));
		It should_get_the_contents_of_file1 = () => FileHandlerContext.Mock.Verify(h => h.GetContents("file1.abc"));
		It should_get_the_contents_of_file2 = () => FileHandlerContext.Mock.Verify(h => h.GetContents("file2.abc"));
		It should_get_the_contents_of_file3 = () => FileHandlerContext.Mock.Verify(h => h.GetContents("file3.abc"));

		It should_have_3_dependencies = () => Chirp.Dependencies.Count.ShouldEqual(3);

		It dependency_x_should_affect_file1_and_file2 = () =>
			Chirp.Dependencies["x"].ShouldContainOnly(
				ProjectItemMocks["file1.abc"].Object,
				ProjectItemMocks["file2.abc"].Object
				);

		It dependency_y_should_affect_file1 = () =>
			Chirp.Dependencies["y"].ShouldContainOnly(
				ProjectItemMocks["file1.abc"].Object
				);

		It dependency_z_should_affect_file1_file2_and_file3 = () =>
			Chirp.Dependencies["z"].ShouldContainOnly(
				ProjectItemMocks["file1.abc"].Object,
				ProjectItemMocks["file2.abc"].Object,
				ProjectItemMocks["file3.abc"].Object
				);
	}
}