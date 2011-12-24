namespace ChirpyTest.EngineSpecs.ConfigChirpyEngineSpecs
{
	using Chirpy.Engines;
	using Machine.Specifications;
	using Moq;
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
				Filename = "demo.chirp.config";
			};

		Because of = () => { Result = Engine.Process(Contents, Filename); };

		It should_add_EngineResult_for_demo1_js = () =>
			Result.ShouldContain(r => r.FileName == "demo1.js");

		It demo1_should_have_contents_of_abc_and_def = () =>
			Result.ShouldContain(r => r.FileName == "demo1.js" && r.Contents == "contents of 'abc.js'\ncontents of 'def.js'");

		It should_add_EngineResult_for_demo2_js = () =>
			Result.ShouldContain(r => r.FileName == "demo2.js");

		It demo2_should_have_contents_of_ghi = () =>
			Result.ShouldContain(r => r.FileName == "demo2.js" && r.Contents == "contents of 'ghi.js'");
	}
}