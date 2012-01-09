namespace ChirpyTest.JavascriptRunnerSpecs
{
	using System.Linq;
	using Chirpy.Javascript;
	using ChirpyInterface;
	using Machine.Specifications;

	[Subject(typeof (JavascriptRunner))]
	public class When_calling_multiple_require : WebFileHandler_context
	{
		static IJavascriptRunner JavascriptRunner;
		static string Script;
		static JavascriptResult Result;

		Establish context = () =>
			{
				JavascriptRunner = new JavascriptRunner
				                   	{
				                   		WebFileHandler = WebFileHandlerMock.Object
				                   	};

				Script = @"require('folder/a');";

				AddFile("require('b');", "folder/a");
				AddFile("external.Set('bar', 'baz');", "folder/b");
			};

		Because of = () => { Result = JavascriptRunner.Execute(Script); };
		
		It should_get_the_full_path_of_a = () =>
			WebFileHandlerMock.Verify(h => h.GetAbsoluteFileName("folder/a", ""));
		
		It should_get_the_full_path_of_b = () =>
			WebFileHandlerMock.Verify(h => h.GetAbsoluteFileName("b", "folder/a"));
		
		It should_get_the_contents_of_a = () =>
			WebFileHandlerMock.Verify(h => h.GetContents("folder/a"));

		It should_get_the_contents_of_b = () =>
			WebFileHandlerMock.Verify(h => h.GetContents("folder/b"));

		It should_add_a_property = () =>
			Result.Properties.Count.ShouldEqual(1);

		It property_name_should_be_bar = () =>
			Result.Properties.First().Key.ShouldEqual("bar");

		It property_value_should_be_baz = () =>
			Result.Properties.First().Value.ShouldEqual("baz");
	}
}