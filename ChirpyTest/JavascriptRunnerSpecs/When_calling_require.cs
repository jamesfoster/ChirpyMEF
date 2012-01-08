namespace ChirpyTest.JavascriptRunnerSpecs
{
	using System.Linq;
	using Chirpy.Javascript;
	using ChirpyInterface;
	using Machine.Specifications;
	using It = Machine.Specifications.It;

	[Subject(typeof (JavascriptRunner))]
	public class When_calling_require : FileHandler_context
	{
		static IJavascriptRunner JavascriptRunner;
		static string Script;
		static JavascriptResult Result;

		Establish context = () =>
			{
				JavascriptRunner = new JavascriptRunner
				                   	{
				                   		FileHandler = FileHandlerMock.Object
				                   	};

				AddFile("external.Set('bar', 'baz')", "foo");

				Script = @"require('foo')";
			};

		Because of = () => { Result = JavascriptRunner.Execute(Script); };
		
		It should_get_the_full_path_of_the_file = () =>
			FileHandlerMock.Verify(h => h.GetAbsoluteFileName("foo", null));

		It should_get_the_contents_of_the_file = () =>
			FileHandlerMock.Verify(h => h.GetContents("foo"));

		It should_add_a_property = () =>
			Result.Properties.Count.ShouldEqual(1);

		It property_name_should_be_bar = () =>
			Result.Properties.First().Key.ShouldEqual("bar");

		It property_value_should_be_baz = () =>
			Result.Properties.First().Value.ShouldEqual("baz");
	}
}