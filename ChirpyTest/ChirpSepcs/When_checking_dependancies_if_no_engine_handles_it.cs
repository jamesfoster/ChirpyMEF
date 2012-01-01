namespace ChirpyTest.ChirpSepcs
{
	using Chirpy;
	using Machine.Specifications;
	using It = Machine.Specifications.It;

	[Subject(typeof (Chirp))]
	public class When_checking_dependancies_if_no_engine_handles_it : Chirp_context
	{
		static bool result;

		Establish context = () => AddFile("def", "file1.abc");

		Because of = () => { result = Chirp.CheckDependancies(ProjectItemMocks["file1.abc"].Object); };

		It should_return_false = () => result.ShouldBeFalse();

		It should_have_0_dependancies = () => Chirp.Dependancies.Count.ShouldEqual(0);
	}
}