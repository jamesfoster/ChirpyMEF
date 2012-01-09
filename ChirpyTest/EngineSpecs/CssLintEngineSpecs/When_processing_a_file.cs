namespace ChirpyTest.EngineSpecs.CssLintEngineSpecs
{
	using System.Collections.Generic;
	using Chirpy.Engines;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof (CssLintEngine))]
	public class When_processing_a_file
	{
		static CssLintEngine engine;
		static Mock<IJavascriptRunner> JavascriptRunnerMock;
		static JavascriptResult javascriptResult;
		static List<EngineResult> result;

		Establish context = () =>
			{
				JavascriptRunnerMock = new Mock<IJavascriptRunner>();

				engine = new CssLintEngine
				         	{
				         		JavascriptRunner = JavascriptRunnerMock.Object
				         	};

				javascriptResult = new JavascriptResult(null);

				javascriptResult.LogMessage("abc", 1, 2, "def", "ghi");

				JavascriptRunnerMock
					.Setup(j => j.Execute(Moq.It.IsAny<string>(), Moq.It.IsAny<Dictionary<string, object>>()))
					.Returns(javascriptResult);
			};

		Because of = () => { result = engine.Process("foo", "bar"); };

		It should_have_1_result = () => result.Count.ShouldEqual(1);
		It result_Filename_should_be_bar = () => result[0].FileName.ShouldEqual("bar");

		It should_call_Execute = () =>
			JavascriptRunnerMock.Verify(j => j.Execute(
				Moq.It.Is<string>(s => s.Contains("require('http://csslint.net/js/csslint.js');")),
				Moq.It.IsAny<Dictionary<string, object>>()));

		It result_should_contain_1_exception = () =>
			result[0].Exceptions.Count.ShouldEqual(1);

		It exception_shou_be_the_same_as_the_JavascriptResult_message = () =>
			result[0].Exceptions[0].ShouldBeTheSameAs(javascriptResult.Messages[0]);
	}
}