namespace ChirpyTest.WebFileHandlerSpecs
{
	using Chirpy.Exports;
	using Machine.Specifications;

	[Subject(typeof(WebFileHandler))]
	public class When_getting_the_absolute_path_of_local_file
	{
		static WebFileHandler webFileHandler;
		static string result;

		Establish context = () =>
			{
				FileHandlerContext.context();

				webFileHandler = new WebFileHandler
				                 	{
				                 		FileHandler = FileHandlerContext.Mock.Object
				                 	};
			};

		Because of = () => { result = webFileHandler.GetAbsoluteFileName("folder2/bar.txt", "c:/folder/foo.txt"); };

		It should_return_the_url_of_bar_relative_to_foo = () => 
			result.ShouldEqual("file:///c:/folder/folder2/bar.txt");
	}
}