namespace ChirpyTest.JavascriptRunnerSpecs
{
	using Chirpy.Javascript;
	using ChirpyInterface;
	using Machine.Specifications;

	[Subject(typeof (JavascriptRunner))]
	public class When_throwing_an_exception
	{
		protected static IJavascriptRunner JavascriptRunner;
		protected static string Script;
		protected static JavascriptResult Result;

		Establish context = () =>
			{
				JavascriptRunner = new JavascriptRunner();

				Script = @"throw { message: 'Something went wrong.', line: 1, column: 2 }";
			};

		Because of = () => { Result = JavascriptRunner.Execute(Script); };

		It should_add_a_message = () =>
			Result.Messages.Count.ShouldEqual(1);

		It message_should_be_something_went_wrong = () =>
			Result.Messages[0].Message.ShouldEqual("Something went wrong.");

		It message_line_should_be_null = () =>
			Result.Messages[0].Line.ShouldBeNull();

		It message_lineNumber_should_be_1 = () =>
			Result.Messages[0].LineNumber.ShouldEqual(1);

		It message_position_should_be_2 = () =>
			Result.Messages[0].Position.ShouldEqual(2);

		It message_filename_should_be_bar_js = () =>
			Result.Messages[0].FileName.ShouldBeNull();

		It message_category_should_be_Error = () =>
			Result.Messages[0].Category.ShouldEqual(ErrorCategory.Error);
	}
}