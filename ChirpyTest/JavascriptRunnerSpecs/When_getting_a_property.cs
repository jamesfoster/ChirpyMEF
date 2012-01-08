namespace ChirpyTest.JavascriptRunnerSpecs
{
	using System.Collections.Generic;
	using Chirpy.Javascript;
	using ChirpyInterface;
	using Machine.Specifications;
	
	[Subject(typeof (JavascriptRunner))]
	public class When_getting_a_property
	{
		static IJavascriptRunner JavascriptRunner;
		static string Script;
		static Dictionary<string, object> Properties;
		static JavascriptResult result;

		Establish context = () =>
			{
				JavascriptRunner = new JavascriptRunner();
				Properties = new Dictionary<string, object>();

				Properties["foo"] = "bar";

				Script = @"
var foo = external.Get('foo');

external.Set('out', foo);
";
			};

		Because of = () => { result = JavascriptRunner.Execute(Script, Properties); };

		It should_add_a_second_properties = () => result.Properties.Count.ShouldEqual(2);
		It new_property_name_should_be_out = () => result.Properties.ContainsKey("out");
		It new_property_value_should_be_bar = () => result.Get("out").ShouldEqual("bar");
	}
}