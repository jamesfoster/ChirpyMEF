namespace ChirpyTest.EngineSpecs.ConfigChirpyEngineSpecs
{
	using Chirpy.Engines;
	using ChirpyInterface;
	using Machine.Specifications;

	[Subject(typeof(ConfigEngine))]
	public class When_processing_a_config_file_if_file_doesnt_exist : ConfigEngine_context
	{
		Establish context = () =>
			{
				Contents = @"
<Root>
	<FileGroup Path='demo1.js'>
		<File Path='abc.js' />
		<File Path='def.js' />
	</FileGroup>
	<FileGroup Path='demo2.js'>
		<File Path='ghi.js' />
	</FileGroup>
</Root>
";
				Filename = "demo.chirp.config";

				AddFile("abc", "abc.js");
				AddFile("ghi", "ghi.js");
			};

		Because of = () => { Result = Engine.Process(Contents, Filename); };

		It should_have_2_results = () => Result.Count.ShouldEqual(2);
		It result_1_filename_should_be_demo1_js = () => Result[0].FileName.ShouldEqual("demo1.js");
		It result_1_should_have_contents_of_abc = () => Result[0].Contents.ShouldEqual("abc");
		It result_1_extension_chould_be_null = () => Result[0].Extension.ShouldBeNull();
		It result_1_should_have_one_exception = () => Result[0].Exceptions.Count.ShouldEqual(1);
		It result_1_exception_should_be_a_warning = () => Result[0].Exceptions[0].Category.ShouldEqual(ErrorCategory.Warning);
		It result_1_exception_filename_should_be_config_file = () => Result[0].Exceptions[0].FileName.ShouldEqual(Filename);

		It result_2_filename_should_be_demo1_js = () => Result[1].FileName.ShouldEqual("demo2.js");
		It result_2_should_have_contents_of_ghi = () => Result[1].Contents.ShouldEqual("ghi");
		It result_2_extension_chould_be_null = () => Result[1].Extension.ShouldBeNull();
		It result_2_exceptions_should_be_empty = () => Result[1].Exceptions.ShouldBeEmpty();
	}
}