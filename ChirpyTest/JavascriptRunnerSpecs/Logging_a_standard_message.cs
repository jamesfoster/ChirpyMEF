namespace ChirpyTest.JavascriptRunnerSpecs
{
	using ChirpyInterface;
	using Machine.Specifications;

	[Behaviors]
	public class Logging_a_standard_message
	{
		protected static JavascriptResult Result;

		It should_add_a_message = () =>
			Result.Messages.Count.ShouldEqual(1);

		It message_should_be_foo = () =>
			Result.Messages[0].Message.ShouldEqual("foo");

		It message_line_should_be_baz = () =>
			Result.Messages[0].Line.ShouldEqual("baz");

		It message_lineNumber_should_be_1 = () =>
			Result.Messages[0].LineNumber.ShouldEqual(1);

		It message_position_should_be_2 = () =>
			Result.Messages[0].Position.ShouldEqual(2);

		It message_filename_should_be_bar_js = () =>
			Result.Messages[0].FileName.ShouldEqual("bar.js");
	}
}