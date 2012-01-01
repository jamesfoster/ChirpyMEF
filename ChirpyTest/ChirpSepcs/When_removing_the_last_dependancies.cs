namespace ChirpyTest.ChirpSepcs
{
	using System.Collections.Generic;
	using Chirpy;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof (Chirp))]
	public class When_removing_the_last_dependancies : Chirp_context
	{
		static Mock<IEngine> engineMock;

		Establish context = () =>
			{
				engineMock = AddEngine("DemoEngine", "1.0", "abc");

				AddFile("def", "file1.abc");

				engineMock
					.Setup(e => e.GetDependancies(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns(new List<string> {"x", "y", "z"});

				Chirp.CheckDependancies(ProjectItemMocks["file1.abc"].Object);
			};

		Because of = () => Chirp.RemoveDependancies(ProjectItemMocks["file1.abc"].Object);

		It should_have_0_dependancies = () => Chirp.Dependancies.Count.ShouldEqual(0);
	}
}