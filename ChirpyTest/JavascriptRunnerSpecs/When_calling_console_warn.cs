namespace ChirpyTest.JavascriptRunnerSpecs
{
	using Chirpy.Javascript;
	using ChirpyInterface;
	using Machine.Specifications;

	[Subject(typeof (JavascriptRunner))]
	public class When_calling_console_warning
	{
		protected static IJavascriptRunner JavascriptRunner;
		protected static string Script;
		protected static JavascriptResult Result;

		Establish context = () =>
			{
				JavascriptRunner = new JavascriptRunner();

				Script = @"console.warning('foo')";
			};

		Because of = () => { Result = JavascriptRunner.Execute(Script); };

		It should_add_a_message = () =>
			Result.Messages.Count.ShouldEqual(1);

		It message_category_should_be_Warning = () =>
			Result.Messages[0].Category.ShouldEqual(ErrorCategory.Warning);

		It message_should_be_foo = () =>
			Result.Messages[0].Message.ShouldEqual("foo");
	}
}