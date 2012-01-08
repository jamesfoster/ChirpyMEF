namespace ChirpyTest.JavascriptRunnerSpecs
{
	using Chirpy.Javascript;
	using ChirpyInterface;
	using Machine.Specifications;
	
	[Subject(typeof (JavascriptRunner))]
	public class When_setting_a_property
	{
		static IJavascriptRunner JavascriptRunner;
		static string Script;
		static JavascriptResult result;

		Establish context = () =>
			{
				JavascriptRunner = new JavascriptRunner();


				Script = "external.Set('foo', 'bar')";
			};

		Because of = () => { result = JavascriptRunner.Execute(Script); };

		It should_contain_1_property = () => result.Properties.Count.ShouldEqual(1);
		It the_property_name_should_be_foo = () => result.Properties.ContainsKey("foo");
		It the_property_value_should_be_bar = () => result.Get("foo").ShouldEqual("bar");
	}
}