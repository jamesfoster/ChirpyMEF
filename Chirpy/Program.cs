namespace Chirpy
{
	using System;

	class Program
	{
		static void Main(string[] args)
		{
			var chirp = new Chirp();

			Console.WriteLine(chirp.Run("less", "", "@abc: 123px; .test { width: @abc; }", "test.chirp.less"));
			Console.WriteLine("----------------------");
			Console.WriteLine(chirp.Run("less", "my", "@abc: 123px; .test { width: @abc; }", "test.my.less"));
			Console.WriteLine("----------------------");
			Console.WriteLine(chirp.Run("config", "", "<Root><FileGroup Path='output.js'><File Path='abc.js' /><File Path='def.js'/></FileGroup></Root>", "test.my.less"));

			var s = Console.ReadLine();
		}
	}
}
