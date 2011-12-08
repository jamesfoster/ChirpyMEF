namespace ChirpyTest.EngineSpecs.DotlessChirpyEngineSpecs
{
	using Chirpy.Engines;
	using ChirpyInterface;
	using Machine.Specifications;

	public class When_processing_a_simple_less_file
	{
		static IChirpyEngine engine;
		static string contents;
		static string filename;
		static string result;

		Establish context = () =>
			{
				engine = new DotlessChirpyEngine();

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