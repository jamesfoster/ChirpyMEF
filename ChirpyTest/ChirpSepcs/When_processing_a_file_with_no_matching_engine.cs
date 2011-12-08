namespace ChirpyTest.ChirpSepcs
{
	using Chirpy;
	using Machine.Specifications;

	[Subject(typeof (Chirp))]
	public class When_processing_a_file_with_no_matching_engine : Chirp_context
	{
		Establish context = () =>
			{
				AddEngine("DemoEngine", "xxx", "xxx");

				Category = "abc";
				SubCategory = "def";
				Filename = "jkl";

				AddFile("ghi", Filename);
			};

		Because of = () => Chirp.Run(Category, SubCategory, Filename);

		It should_call_EngineResolver_GetEngines = () => EngineResolverMock.Verify(r => r.GetEngine("abc", "def"));

		It should_return_null; // = () => Result.ShouldEqual(null);
	}
}