namespace ChirpyTest.FileHandlerSpecs
{
	using Chirpy.Exports;
	using Machine.Specifications;

	[Subject(typeof(FileHandler))]
	public class When_getting_the_absolute_path_of_a_file_already_absolute
	{
		static FileHandler fileHandler;
		static string path;
		static string relativeTo;
		static string result;

		Establish context = () =>
			{
				fileHandler = new FileHandler();

				path = @"c:\folder\file.txt";
				relativeTo = @"c:\path\to\other.txt";
			};

		Because of = () => { result = fileHandler.GetAbsoluteFileName(path, relativeTo); };

		It should_be_correct_path = () => result.ShouldBeEqualIgnoringCase(@"c:\folder\file.txt");
	}
}