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
				Contents = "ghi";
				Filename = "jkl";
			};

		Because of = () => { Result = Chirp.Run(Category, SubCategory, Contents, Filename); };

		It should_call_EngineResolver_GetEngines = () => EngineResolverMock.Verify(r => r.GetEngine("abc", "def"));

		It should_return_null = () => Result.ShouldEqual(null);
	}
}