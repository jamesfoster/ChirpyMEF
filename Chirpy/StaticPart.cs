namespace Chirpy
{
	using System.ComponentModel.Composition;
	using System.IO;
	using System.Reflection;
	using EnvDTE80;

	public class StaticPart
	{
		[Export]
		public static DTE2 App { get; set; }

		[Export("ChirpyDirectory")]
		public static string ChirpyDirectory
		{
			get
			{
				return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			}
		}

		[Export("PluginDirectory")]
		public static string PluginDirectory
		{
			get
			{
				return Path.Combine(ChirpyDirectory, "Plugins");
			}
		}
	}
}
