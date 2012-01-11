namespace ChirpyTest.EngineSpecs.CssLintEngineSpecs
{
	using System.Collections.Generic;
	using Chirpy.Engines;
	using Chirpy.Exports;
	using Chirpy.Javascript;
	using ChirpyInterface;
	using Machine.Specifications;
	using It = Machine.Specifications.It;

	[Subject(typeof (CssLintEngine))]
	[Tags("Web", "Integration")]
	public class When_actually_processing_a_file
	{
		static CssLintEngine engine;
		static List<EngineResult> result;
		static string contents;

		Establish context = () =>
			{
				var fileHandler = new FileHandler();
				var webFileHandler = new WebFileHandler
				                     	{
				                     		FileHandler = fileHandler
				                     	};
				var javascriptRunner = new JavascriptRunner
				                       	{
				                       		WebFileHandler = webFileHandler
				                       	};

				engine = new CssLintEngine
				         	{
				         		JavascriptRunner = javascriptRunner
				         	};

				contents = @"
.class {
	width: 123px;
	width: 123px;
}
";

				contents = contents.Trim();
			};

		Because of = () => { result = engine.Process(contents, "bar"); };

		It should_have_1_result = () =>
			result.Count.ShouldEqual(1);

		It should_have_1_exceptions = () =>
			result[0].Exceptions.Count.ShouldEqual(1);

		It message_should_be_duplicate_property = () =>
			result[0].Exceptions[0].Message.ShouldStartWith("Duplicate property 'width' found.");

		It line_number_should_be_3 = () =>
			result[0].Exceptions[0].LineNumber.ShouldEqual(3);

		It position_should_be_2 = () =>
			result[0].Exceptions[0].Position.ShouldEqual(2);

		It category_should_be_message = () => 
			result[0].Exceptions[0].Category.ShouldEqual(ErrorCategory.Message);
	}
}