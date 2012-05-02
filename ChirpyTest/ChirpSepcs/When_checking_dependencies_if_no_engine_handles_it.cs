namespace ChirpyTest.ChirpSepcs
{
	using Chirpy;
	using Machine.Specifications;
	using It = Machine.Specifications.It;

	[Subject(typeof (Chirp))]
	public class When_checking_dependencies_if_no_engine_handles_it : Chirp_context
	{
		static bool result;

		Establish context = () => AddProjectItem("def", "file1.abc");

		Because of = () => { result = Chirp.CheckDependencies(ProjectItemMocks["file1.abc"].Object); };

		It should_return_false = () => result.ShouldBeFalse();

		It should_have_0_dependencies = () => Chirp.Dependencies.Count.ShouldEqual(0);
	}
}