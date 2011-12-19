namespace Chirpy
{
	using System;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.IO;

	public class MefComposer : IDisposable
	{
		protected CompositionContainer Container { get; set; }
		protected bool IsDisposed { get; set; }

		private MefComposer(CompositionContainer container)
		{
			Container = container;
		}

		public Lazy<T> GetExport<T>()
		{
			return Container.GetExport<T>();
		}

		public static MefComposer ComposeWithPlugins(object part)
		{
			var composer = CreateComposerWithPlugins();

			composer.Container.ComposeParts(part);

			return composer;
		}

		public static MefComposer ComposeWithoutPlugins(object part)
		{
			var composer = CreateComposerWithoutPlugins();

			composer.Container.ComposeParts(part);

			return composer;
		}

		public static MefComposer CreateComposerWithPlugins()
		{
			var pluginDirectory = StaticPart.PluginDirectory;

			if (!Directory.Exists(pluginDirectory))
				Directory.CreateDirectory(pluginDirectory);

			var assemblyCatalog = new AssemblyCatalog(typeof (Chirp).Assembly);
			var directoryCatalog = new DirectoryCatalog(pluginDirectory);
			var catalog = new AggregateCatalog(assemblyCatalog, directoryCatalog);

			var container = new CompositionContainer(catalog);

			return new MefComposer(container);
		}

		public static MefComposer CreateComposerWithoutPlugins()
		{
			var catalog = new AssemblyCatalog(typeof (Chirp).Assembly);

			var container = new CompositionContainer(catalog);

			return new MefComposer(container);
		}

		public void Dispose()
		{
			lock(this)
			{
				if (IsDisposed)
					return;

				IsDisposed = true;

				if(Container != null)
					Container.Dispose();
			}
		}
	}
}
