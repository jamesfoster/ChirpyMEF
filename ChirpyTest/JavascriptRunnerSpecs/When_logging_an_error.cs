namespace ChirpyTest.JavascriptRunnerSpecs
{
	using Chirpy.Javascript;
	using ChirpyInterface;
	using Machine.Specifications;

	[Subject(typeof (JavascriptRunner))]
	public class When_logging_an_error
	{
		protected static IJavascriptRunner JavascriptRunner;
		protected static string Script;
		protected static JavascriptResult Result;

		Establish context = () =>
			{
				JavascriptRunner = new JavascriptRunner();

				Script = @"external.LogError('foo', 1, 2, 'bar.js', 'baz')";
			};

		Because of = () => { Result = JavascriptRunner.Execute(Script); };

		Behaves_like<Logging_a_standard_message> Logging_a_standard_message;

		It message_category_should_be_Message = () =>
			Result.Messages[0].Category.ShouldEqual(ErrorCategory.Error);
	}
}