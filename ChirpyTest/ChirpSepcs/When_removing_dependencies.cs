namespace ChirpyTest.ChirpSepcs
{
	using System.Collections.Generic;
	using Chirpy;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof (Chirp))]
	public class When_removing_dependencies : Chirp_context
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

				Chirp.CheckDependencies(ProjectItemMocks["file1.abc"].Object);
				Chirp.CheckDependencies(ProjectItemMocks["file2.abc"].Object);
				Chirp.CheckDependencies(ProjectItemMocks["file3.abc"].Object);
			};

		Because of = () => Chirp.RemoveDependencies(ProjectItemMocks["file1.abc"].Object);

		It should_have_2_dependencies = () => Chirp.Dependencies.Count.ShouldEqual(2);
		It dependency_y_should_no_longer_exist = () => Chirp.Dependencies.ContainsKey("y").ShouldBeFalse();

		It dependency_x_should_affect_file2 = () =>
			Chirp.Dependencies["x"].ShouldContainOnly(
				ProjectItemMocks["file2.abc"].Object
				);

		It dependency_z_should_affect_file2_and_file3 = () =>
			Chirp.Dependencies["z"].ShouldContainOnly(
				ProjectItemMocks["file2.abc"].Object,
				ProjectItemMocks["file3.abc"].Object
				);
	}
}