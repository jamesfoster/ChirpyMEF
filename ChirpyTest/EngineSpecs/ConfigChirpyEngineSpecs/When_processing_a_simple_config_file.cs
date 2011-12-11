namespace ChirpyTest.EngineSpecs.ConfigChirpyEngineSpecs
{
	using Chirpy.Engines;
	using Machine.Specifications;
	using It = Machine.Specifications.It;

	[Subject(typeof(ConfigChirpyEngine))]
	public class When_processing_a_simple_config_file : ConfigChirpyEngine_context
	{
		Establish context = () =>
			{
				Contents = @"
<Root>
	<FileGroup Path='demo.js'>
		<File Path='abc.js' />
		<File Path='def.js' />
	</FileGroup>
</Root>
";
				Filename = "demo.chirp.config";
			};

		Because of = () => { Result = Engine.Process(Contents, Filename); };

		// DELETE THIS TEST
		It should_return_the_contents_of_the_files = () => Result.ShouldEqual("contents of 'abc.js'\ncontents of 'def.js'");

		It should_add_a_new_project_item; //= () =>
			//ProjectItemManagerMock.Verify(m => m.AddFile("demo.js", "demo.chirp.config", "contents of 'abc.js'\ncontents of 'def.js'"));
		It should_bypass_default_processing; //= () => result.ShouldEqual(null);
	}
}