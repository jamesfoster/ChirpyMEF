namespace ChirpyTest.EngineSpecs.DotlessChirpyEngineSpecs
{
	using Chirpy.Engines;
	using ChirpyInterface;
	using Machine.Specifications;

	[Subject(typeof(DotlessEngine))]
	public class When_processing_a_simple_less_file
	{
		static IEngine engine;
		static string contents;
		static string filename;
		static string result;

		Establish context = () =>
			{
				engine = new DotlessEngine();

				contents = @"
@abc: 123px;

.test {
  width: @abc;
}
";
				filename = "demo.less";
			};

		Because of = () => { result = engine.Process(contents, filename); };

		It should_parse_the_less_into_css = () => result.Trim().ShouldEqual(".test {\n  width: 123px;\n}");
	}
}