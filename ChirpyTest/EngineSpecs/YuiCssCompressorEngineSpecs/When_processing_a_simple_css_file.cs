namespace ChirpyTest.EngineSpecs.YuiCssCompressorEngineSpecs
{
	using System.Collections.Generic;
	using Chirpy.Engines;
	using ChirpyInterface;
	using Machine.Specifications;

	[Subject(typeof(YuiCssCompressorEngine))]
	public class When_processing_a_simple_css_file
	{
		static IEngine engine;
		static string contents;
		static string filename;
		static List<EngineResult> result;

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

		It the_extension_should_be_min_css; // = () => result[0].Extension.ShouldEqual("min.css");
		It should_compress_the_css = () => result[0].Contents.ShouldEqual(".test{width:100px;height:200px}\n.test2{color:#123456}");
	}
}