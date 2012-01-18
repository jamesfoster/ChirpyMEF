namespace ChirpyTest.ChirpSepcs
{
	using System.Collections.Generic;
	using Chirpy;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof (Chirp))]
	public class When_removing_dependancies : Chirp_context
	{
		static Mock<IEngine> engineMock;

		Establish context = () =>
			{
				engineMock = EngineResolverContext.AddEngine("DemoEngine", "1.0", "abc");

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

				Chirp.CheckDependancies(ProjectItemMocks["file1.abc"].Object);
				Chirp.CheckDependancies(ProjectItemMocks["file2.abc"].Object);
				Chirp.CheckDependancies(ProjectItemMocks["file3.abc"].Object);
			};

		Because of = () => Chirp.RemoveDependancies(ProjectItemMocks["file1.abc"].Object);

		It should_have_2_dependancies = () => Chirp.Dependancies.Count.ShouldEqual(2);
		It dependancy_y_should_no_longer_exist = () => Chirp.Dependancies.ContainsKey("y").ShouldBeFalse();

		It dependancy_x_should_affect_file2 = () =>
			Chirp.Dependancies["x"].ShouldContainOnly(
				ProjectItemMocks["file2.abc"].Object
				);

		It dependancy_z_should_affect_file2_and_file3 = () =>
			Chirp.Dependancies["z"].ShouldContainOnly(
				ProjectItemMocks["file2.abc"].Object,
				ProjectItemMocks["file3.abc"].Object
				);
	}
}