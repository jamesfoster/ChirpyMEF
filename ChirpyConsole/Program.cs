﻿namespace ChirpyConsole
{
	using System;
	using System.IO;
	using Chirpy;

	class Program
	{
		static void Main(string[] args)
		{
			string path;

			if (args.Length > 0)
			{
				var currentDirectory = Environment.CurrentDirectory;

				path = Path.Combine(currentDirectory, args[0]);
			}
			else
				path = "test.my.less";

			var chirp = MefComposer
				.CreateComposerWithPlugins()
				.GetExport<Chirp>();

			Console.WriteLine(chirp.Value.RunDependancies(path));

			Console.ReadLine();
		}
	}
}
