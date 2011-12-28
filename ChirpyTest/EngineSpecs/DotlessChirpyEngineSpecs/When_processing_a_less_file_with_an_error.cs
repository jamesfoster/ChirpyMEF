namespace ChirpyTest.EngineSpecs.DotlessChirpyEngineSpecs
{
	using System.Collections.Generic;
	using Chirpy.Engines;
	using ChirpyInterface;
	using Machine.Specifications;

	[Subject(typeof(DotlessEngine))]
	public class When_processing_a_less_file_with_an_error : DotlessEngine_context
	{
		static List<EngineResult> result;

		Establish context = () =>
			{
				Contents = @"
.test {
  width: @abc;
}
";
				Filename = "demo.less";
			};

		Because of = () => { result = Engine.Process(Contents, Filename); };

		It should_have_one_result = () => result.Count.ShouldEqual(1);
		It should_have_one_exception = () => result[0].Exceptions.Count.ShouldEqual(1);
		It file_name_should_be_same_as_input_file = () => result[0].Exceptions[0].FileName.ShouldEqual("demo.less");
		It line_should_be_the_line_with_the_error = () => result[0].Exceptions[0].Line.ShouldEqual("  width: @abc;");
		It line_number_should_be_3 = () => result[0].Exceptions[0].LineNumber.ShouldEqual(3);
		It position_should_be_10 = () => result[0].Exceptions[0].Position.ShouldEqual(9);
		
		It the_extension_should_be_null = () => result[0].Extension.ShouldBeNull();
		It the_contents_should_be_null = () => result[0].Contents.ShouldBeNull();
		It the_filename_should_be_null = () => result[0].FileName.ShouldBeNull();
	}
}