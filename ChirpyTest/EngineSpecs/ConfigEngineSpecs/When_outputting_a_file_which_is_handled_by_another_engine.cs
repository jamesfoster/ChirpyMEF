namespace ChirpyTest.EngineSpecs.ConfigChirpyEngineSpecs
{
	using System.Collections.Generic;
	using Chirpy.Engines;
	using ChirpyInterface;
	using Machine.Specifications;
	using Moq;
	using It = Machine.Specifications.It;

	[Subject(typeof(ConfigEngine))]
	public class When_outputting_a_file_which_is_handled_by_another_engine : ConfigEngine_context
	{
		static Mock<IEngine> engineMock;
		static List<EngineResult> engineResult;

		Establish context = () =>
			{
				Contents = @"
<Root>
	<FileGroup Path='demo1.foo.js'>
		<File Path='abc.js' />
		<File Path='def.js' />
	</FileGroup>
</Root>
";
				Filename = "folder/demo.chirp.config";

				FileHandlerContext.AddFile("abc", "folder/abc.js");
				FileHandlerContext.AddFile("def", "folder/def.js");

				engineResult = new List<EngineResult>
				               	{
				               		new EngineResult {Contents = "xyz", Extension = ".xyz.js"}
				               	};

				engineMock = EngineResolverContext.AddEngine("Demo Engine", "1.0", "foo.js");

				engineMock
					.Setup(e => e.Process(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
					.Returns(engineResult);
			};

		Because of = () => { Result = Engine.Process(Contents, Filename); };

		It should_have_2_results = () => Result.Count.ShouldEqual(2);
		It result_1_filename_should_be_demo1_foo_js = () => Result[0].FileName.ShouldEqual("demo1.foo.js");
		It result_1_should_have_contents_of_abc_and_def = () => Result[0].Contents.ShouldEqual("abc\ndef");
		It result_1_extension_chould_be_null = () => Result[0].Extension.ShouldBeNull();
		It result_1_should_have_no_exceptions = () => Result[0].Exceptions.ShouldBeEmpty();

		It result_2_filename_should_be_demo1_xyz_js = () => Result[1].FileName.ShouldEqual("demo1.foo.js");
		It result_2_should_have_contents_xyz = () => Result[1].Contents.ShouldEqual("xyz");
		It result_2_extension_chould_be_xyz_js = () => Result[1].Extension.ShouldEqual(".xyz.js");
		It result_2_exceptions_should_be_empty = () => Result[1].Exceptions.ShouldBeEmpty();
	}
}