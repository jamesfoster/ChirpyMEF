namespace ChirpyTest.WebFileHandlerSpecs
{
	using Chirpy.Exports;
	using Machine.Specifications;

	[Subject(typeof(WebFileHandler))]
	public class When_getting_the_absolute_path_of_a_url : FileHandler_context
	{
		static WebFileHandler webFileHandler;
		static string result;

		Establish context = () =>
			{
				webFileHandler = new WebFileHandler
				                 	{
				                 		FileHandler = FileHandlerMock.Object
				                 	};
			};

		Because of = () => { result = webFileHandler.GetAbsoluteFileName("folder2/bar.txt", "http://example.com/folder/foo.txt"); };

		It should_return_the_url_of_bar_relative_to_foo = () => 
			result.ShouldEqual("http://example.com/folder/folder2/bar.txt");
	}
}