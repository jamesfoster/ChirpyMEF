namespace ChirpyTest.ChirpSepcs
{
	using System.Collections.Generic;
	using Chirpy;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof (Chirp))]
	public class When_removing_the_last_dependencies : Chirp_context
	{
		static Mock<IEngine> engineMock;

		Establish context = () =>
			{
				engineMock = EngineResolverContext.AddEngine("DemoEngine", "1.0", "abc");

				AddProjectItem("def", "file1.abc");

				engineMock
					.Setup(e => e.GetDependencies(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns(new List<string> {"x", "y", "z"});

				Chirp.CheckDependencies(ProjectItemMocks["file1.abc"].Object);
			};

		Because of = () => Chirp.RemoveDependencies(ProjectItemMocks["file1.abc"].Object);

		It should_have_0_dependencies = () => Chirp.Dependencies.Count.ShouldEqual(0);
	}
}