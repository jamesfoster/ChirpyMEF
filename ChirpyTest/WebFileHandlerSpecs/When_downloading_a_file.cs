namespace ChirpyTest.WebFileHandlerSpecs
{
	using Chirpy.Exports;
	using Machine.Specifications;

	[Subject(typeof(WebFileHandler))]
	public class When_downloading_a_file
	{
		static WebFileHandler webFileHandler;
		static string result;

		Establish context = () =>
			{
				webFileHandler = new WebFileHandler
				                 	{
				                 		FileHandler = new FileHandler()
				                 	};
			};

		Because of = () => { result = webFileHandler.GetContents("https://raw.github.com/jamesfoster/ChirpyMEF/master/Chirpy/Chirp.cs"); };

		It should_download_the_file = () => 
			result.ShouldContain("public class Chirp");
	}
}