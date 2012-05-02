namespace ChirpyTest.ChirpSepcs
{
	using System.Collections.Generic;
	using Chirpy;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof (Chirp))]
	public class When_checking_dependencies : Chirp_context
	{
		static Mock<IEngine> engineMock;
		static bool result;

		Establish context = () =>
			{
				engineMock = EngineResolverContext.AddEngine("DemoEngine", "1.0", "abc");

				AddProjectItem("def", "file1.abc");

				engineMock
					.Setup(e => e.GetDependencies(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns(new List<string> {"x", "y", "z"});
			};

		Because of = () => { result = Chirp.CheckDependencies(ProjectItemMocks["file1.abc"].Object); };

		It should_get_the_engine = () => EngineResolverContext.Mock.Verify(r => r.GetEngineByFilename("file1.abc"));
		It should_call_Engine_GetDependencies = () => engineMock.Verify(e => e.GetDependencies("def", "file1.abc"));
		It should_get_the_contents_of_the_file = () => FileHandlerContext.Mock.Verify(h => h.GetContents("file1.abc"));

		It should_return_true = () => result.ShouldBeTrue();

		It should_have_3_dependencies = () => Chirp.Dependencies.Count.ShouldEqual(3);
		It dependency_x_should_affect_file1 = () => Chirp.Dependencies["x"].ShouldContainOnly(ProjectItemMocks["file1.abc"].Object);
		It dependency_y_should_affect_file1 = () => Chirp.Dependencies["y"].ShouldContainOnly(ProjectItemMocks["file1.abc"].Object);
		It dependency_z_should_affect_file1 = () => Chirp.Dependencies["z"].ShouldContainOnly(ProjectItemMocks["file1.abc"].Object);
	}
}