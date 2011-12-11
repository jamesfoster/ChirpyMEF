namespace ChirpyTest.EngineSpecs.YuiJavascriptCompressorEngineSpecs
{
	using Chirpy.Engines;
	using ChirpyInterface;
	using Machine.Specifications;

	[Subject(typeof(YuiJavascriptCompressorEngine))]
	public class When_processing_a_simple_js_file
	{
		static IChirpyEngine engine;
		static string contents;
		static string filename;
		static string result;

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

		It should_compress_the_javascript = () => result.ShouldEqual("(function(){function a(b){alert(\"hello \"+b)}a(\"world\")})();");
	}
}