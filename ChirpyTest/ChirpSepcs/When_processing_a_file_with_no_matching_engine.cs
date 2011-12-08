namespace ChirpyTest
{
	using Chirpy;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof (Chirp))]
	public class When_processing_a_file_with_no_matching_engine : Chirp_context
	{
		static Mock<IChirpyEngine> engineMock;

		Establish context = () =>
			{
				AddEngine("DemoEngine", "xxx", "xxx");

				Category = "abc";
				SubCategory = "def";
				Contents = "ghi";
				Filename = "jkl";
			};

		Because of = () => { Result = Chirp.Run(Category, SubCategory, Contents, Filename); };

		It should_call_EngineResolver_GetEngines = () => EngineResolverMock.Verify(r => r.GetEngines("abc", "def"));

		It should_return_the_input = () => Result.ShouldEqual("ghi");
	}
}