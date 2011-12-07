namespace Chirpy
{
	using System;

	class Program
	{
		static void Main(string[] args)
		{
			var chirp = new Chirp();

			while (true)
			{
				var s = Console.ReadLine();

				if (s == "q")
					return;

				Console.WriteLine(chirp.Run("less", "@abc: 123px; .test { width: @abc; }", "test.chirp.less"));
				Console.WriteLine();
				Console.WriteLine(chirp.Run("myless", "@abc: 123px; .test { width: @abc; }", "test.my.less"));
			}
		}
	}
}
