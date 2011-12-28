namespace ChirpyTest.EngineSpecs.DotlessChirpyEngineSpecs
{
	using System.Collections.Generic;
	using Chirpy.Engines;
	using ChirpyInterface;
	using Machine.Specifications;

	[Subject(typeof(DotlessEngine))]
	public class When_processing_a_simple_less_file : DotlessEngine_context
	{
		static List<EngineResult> result;

		Establish context = () =>
			{
				Contents = @"
@abc: 123px;

.test {
  width: @abc;
}
";
				Filename = "demo.less";
			};

		Because of = () => { result = Engine.Process(Contents, Filename); };

		It should_contain_one_result = () => result.Count.ShouldEqual(1);
		It the_extension_should_be_min_css = () => result[0].Extension.ShouldEqual("css");
		It the_contents_should_be_the_parsed_css = () => result[0].Contents.Trim().ShouldEqual(".test {\n  width: 123px;\n}");
		It the_filename_should_be_null = () => result[0].FileName.ShouldBeNull();
		It the_exceptions_should_be_empty = () => result[0].Exceptions.ShouldBeEmpty();
	}
}