namespace Chirpy
{
	using System.ComponentModel.Composition;
	using EnvDTE80;

	public class AppPart
	{
		public static DTE2 App { get; set; }

		[Export] DTE2 app { get { return App; } }
	}
}
