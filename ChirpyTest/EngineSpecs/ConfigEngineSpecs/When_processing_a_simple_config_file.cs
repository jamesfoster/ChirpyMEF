namespace ChirpyTest.EngineSpecs.ConfigChirpyEngineSpecs
{
	using Chirpy.Engines;
	using Machine.Specifications;
	using It = Machine.Specifications.It;

	[Subject(typeof(ConfigEngine))]
	public class When_processing_a_simple_config_file : ConfigEngine_context
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
				Filename = "folder/demo.chirp.config";

				AddFile("abc", "folder/abc.js");
				AddFile("def", "folder/def.js");
				AddFile("ghi", "folder/ghi.js");
			};

		Because of = () => { Result = Engine.Process(Contents, Filename); };

		It should_get_absolute_filename_for_abc =() => FileHandlerMock.Verify(h => h.GetAbsoluteFileName("abc.js", Filename));
		It should_get_absolute_filename_for_def =() => FileHandlerMock.Verify(h => h.GetAbsoluteFileName("def.js", Filename));
		It should_get_absolute_filename_for_ghi =() => FileHandlerMock.Verify(h => h.GetAbsoluteFileName("ghi.js", Filename));

		It should_check_abc_exists =() => FileHandlerMock.Verify(h => h.FileExists("folder/abc.js"));
		It should_check_def_exists =() => FileHandlerMock.Verify(h => h.FileExists("folder/def.js"));
		It should_check_ghi_exists =() => FileHandlerMock.Verify(h => h.FileExists("folder/ghi.js"));

		It should_have_2_results = () => Result.Count.ShouldEqual(2);
		It result_1_filename_should_be_demo1_js = () => Result[0].FileName.ShouldEqual("demo1.js");
		It result_1_should_have_contents_of_abc_and_def = () => Result[0].Contents.ShouldEqual("abc\ndef");
		It result_1_extension_chould_be_null = () => Result[0].Extension.ShouldBeNull();
		It result_1_exceptions_should_be_empty = () => Result[0].Exceptions.ShouldBeEmpty();

		It result_2_filename_should_be_demo1_js = () => Result[1].FileName.ShouldEqual("demo2.js");
		It result_2_should_have_contents_of_ghi = () => Result[1].Contents.ShouldEqual("ghi");
		It result_2_extension_chould_be_null = () => Result[1].Extension.ShouldBeNull();
		It result_2_exceptions_should_be_empty = () => Result[1].Exceptions.ShouldBeEmpty();
	}
}