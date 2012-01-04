namespace Chirpy.Composition
{
	using System;
	using System.ComponentModel.Composition.Hosting;
	using System.ComponentModel.Composition.Primitives;
	using System.IO;
	using System.Linq;

	public class DirectoryWatcherCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
	{
		protected FileSystemWatcher FileSystemWatcher { get; set; }
		protected DirectoryCatalog DirectoryCatalog { get; set; }

		public DirectoryWatcherCatalog(string path) : this(path, "*.dll")
		{
		}

		public DirectoryWatcherCatalog(string path, string searchPattern)
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			DirectoryCatalog = new DirectoryCatalog(path, searchPattern);

			FileSystemWatcher = new FileSystemWatcher(path, searchPattern);
			FileSystemWatcher.Created += OnChanged;
			FileSystemWatcher.Deleted += OnChanged;
			FileSystemWatcher.Changed += OnChanged;
			FileSystemWatcher.EnableRaisingEvents = true;
		}

		void OnChanged(object o, FileSystemEventArgs a)
		{
			DirectoryCatalog.Refresh();
		}

		public override IQueryable<ComposablePartDefinition> Parts
		{
			get { return DirectoryCatalog.Parts; }
		}

		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed
		{
			add { DirectoryCatalog.Changed += value; }
			remove { DirectoryCatalog.Changed -= value; }
		}

		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing
		{
			add { DirectoryCatalog.Changing += value; }
			remove { DirectoryCatalog.Changing -= value; }
		}

		protected override void Dispose(bool disposing)
		{
			FileSystemWatcher.Dispose();
			DirectoryCatalog.Dispose();

			base.Dispose(disposing);
		}
	}
}