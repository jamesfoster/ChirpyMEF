namespace ChirpyTest.FileHandlerSpecs
{
	using System;
	using Chirpy.Exports;
	using Machine.Specifications;

	[Subject(typeof(FileHandler))]
	public class When_not_possible_to_get_the_absolute_path_of_a_file
	{
		static FileHandler fileHandler;
		static string path;
		static string relativeTo;
		static Exception exception;

		Establish context = () =>
			{
				fileHandler = new FileHandler();

				path = @"folder\file.txt";
				relativeTo = @"path\to\other.txt";
			};

		Because of = () => { exception = Catch.Exception(() => fileHandler.GetAbsoluteFileName(path, relativeTo)); };

		It should_throw_an_exception = () => exception.ShouldNotBeNull();
		It should_throw_an_InvalidOperationException = () => exception.ShouldBeOfType<InvalidOperationException>();

		It the_message_should_be_correct = () =>
			exception.Message.ShouldEqual(string.Format("Unable to get absolute path of '{0}' relative to '{1}'", path, relativeTo));
	}
}