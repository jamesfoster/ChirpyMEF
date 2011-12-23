namespace Chirpy
{
	using System.ComponentModel.Composition;
	using System.IO;
	using System.Reflection;
	using EnvDTE80;
	using Microsoft.VisualStudio.Shell;

	public class StaticPart
	{
		[Export]
		public static DTE2 App { get; set; }

		[Export]
		public System.IServiceProvider ServiceProvider
		{
			get
			{
				var serviceProvider = App as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

				if(serviceProvider == null)
					return null;

				return new ServiceProvider(serviceProvider);
			}
		}

		[Export]
		public IErrorListProvider ErrorListProvider
		{
			get
			{
				if(ServiceProvider == null)
					return null;

				var errorListProvider = new ErrorListProvider(ServiceProvider);

				return new ErrorListProviderWrapper(errorListProvider);
			}
		}

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
