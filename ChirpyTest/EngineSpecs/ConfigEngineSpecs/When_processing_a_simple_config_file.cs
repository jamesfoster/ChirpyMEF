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

		It should_add_a_new_project_item1 = () =>
			ProjectItemManagerMock.Verify(m => m.AddFile("demo1.js", "demo.chirp.config", "contents of 'abc.js'\ncontents of 'def.js'"), Times.Once());

		It should_add_a_new_project_item2 = () =>
			ProjectItemManagerMock.Verify(m => m.AddFile("demo2.js", "demo.chirp.config", "contents of 'ghi.js'"), Times.Once());

		It should_bypass_default_processing = () => Result.ShouldEqual(null);
	}
}