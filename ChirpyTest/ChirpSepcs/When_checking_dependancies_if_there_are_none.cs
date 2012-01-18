namespace ChirpyTest.ChirpSepcs
{
	using System.Collections.Generic;
	using Chirpy;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof (Chirp))]
	public class When_checking_dependancies_if_there_are_none : Chirp_context
	{
		static Mock<IEngine> engineMock;
		static bool result;

		Establish context = () =>
			{
				engineMock = EngineResolverContext.AddEngine("DemoEngine", "1.0", "abc");

				AddProjectItem("def", "file1.abc");

				engineMock
					.Setup(e => e.GetDependancies(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns(new List<string>());
			};

		Because of = () => { result = Chirp.CheckDependancies(ProjectItemMocks["file1.abc"].Object); };

		It should_return_true = () => result.ShouldBeTrue();

		It should_have_0_dependancies = () => Chirp.Dependancies.Count.ShouldEqual(0);
	}
}