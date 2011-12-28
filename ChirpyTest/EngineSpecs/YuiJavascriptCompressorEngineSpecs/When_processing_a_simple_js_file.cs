namespace ChirpyTest.EngineSpecs.YuiJavascriptCompressorEngineSpecs
{
	using System.Collections.Generic;
	using Chirpy.Engines;
	using ChirpyInterface;
	using Machine.Specifications;

	[Subject(typeof(YuiJavascriptCompressorEngine))]
	public class When_processing_a_simple_js_file
	{
		static IEngine engine;
		static string contents;
		static string filename;
		static List<EngineResult> result;

		Establish context = () =>
			{
				engine = new YuiJavascriptCompressorEngine();

				contents = @"
(function() {
	function abc(value) {
		alert('hello ' + value);
	}

	abc('world');
})();
";
				filename = "demo.css";
			};

		Because of = () => { result = engine.Process(contents, filename); };

		It should_contain_one_result = () => result.Count.ShouldEqual(1);
		It the_extension_should_be_min_css = () => result[0].Extension.ShouldEqual("min.js");
		It the_contents_should_be_compressed = () => result[0].Contents.ShouldEqual("(function(){function a(b){alert(\"hello \"+b)}a(\"world\")})();");
		It the_filename_should_be_null = () => result[0].FileName.ShouldBeNull();
		It the_exceptions_should_be_empty = () => result[0].Exceptions.ShouldBeEmpty();
	}
}