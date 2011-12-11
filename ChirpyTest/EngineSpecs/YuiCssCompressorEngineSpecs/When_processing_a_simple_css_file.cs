namespace ChirpyTest.EngineSpecs.YuiCssCompressorEngineSpecs
{
	using Chirpy.Engines;
	using ChirpyInterface;
	using Machine.Specifications;

	[Subject(typeof(YuiCssCompressorEngine))]
	public class When_processing_a_simple_css_file
	{
		static IChirpyEngine engine;
		static string contents;
		static string filename;
		static string result;

		Establish context = () =>
			{
				engine = new YuiCssCompressorEngine();

				contents = @"
.test {
  width: 100px;
  height: 200px;
}
.test2 {
  color: #123456;
}
";
				filename = "demo.css";
			};

		Because of = () => { result = engine.Process(contents, filename); };

		It should_compress_the_css = () => result.ShouldEqual(".test{width:100px;height:200px}\n.test2{color:#123456}");
	}
}