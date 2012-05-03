namespace Chirpy
{
	using System;
	using System.ComponentModel.Composition;
	using System.Linq;
	using Composition;
	using EnvDTE;
	using EnvDTE80;
	using Exports;
	using Extensibility;
	using Imports;
	using Logging;

	public class ChirpyAddIn : IDTExtensibility2
	{
		protected ILogger Logger { get; set; } 
		protected DTE2 App { get; set; }
		protected AddIn Instance { get; set; }
		protected Events2 Events { get; set; }

		protected MefComposer Composer { get; set; }

		[Import] public Chirp Chirp { get; set; }
		[Import] public IProjectItemManager ProjectItemManager { get; set; }
		[Import] public ITaskList TaskList { get; set; }

		protected DocumentEvents DocumentEvents { get; set; }
		protected BuildEvents BuildEvents { get; set; }
		protected ProjectItemsEvents ProjectItemsEvents { get; set; }
		protected SolutionEvents SolutionEvents { get; set; }

		public bool HasBoundEvents { get; set; }

		/// <summary>
		/// Implements the OnDisconnection method of the IDTExtensibility2 interface. 
		/// Occurs when he Add-in is loaded into Visual Studio.
		/// </summary>
		/// <param name="application">A reference to an instance of the IDE, DTE.</param>
		/// <param name="connectMode">
		///   An <see cref="ext_ConnectMode"/> enumeration value that indicates 
		///   the way the add-in was loaded into Visual Studio.
		/// </param>
		/// <param name="instance">An <see cref="AddIn"/> reference to the add-in's own instance.</param>
		/// <param name="custom">An empty array that you can use to pass host-specific data for use in the add-in.</param>
		public void OnConnection(object application, ext_ConnectMode connectMode, object instance, ref Array custom)
		{
			App = (DTE2) application;
			Instance = (AddIn) instance;
			Events = App.Events as Events2;
			Logger = new OutputWindowLogger(App);

			Logger.Log("Loading...");

			Compose(); // Do not attempt to use [Import]s before this line!

			if(Chirp == null)
			{
				Logger.Log("Unable to load.");
				return;
			}

			BindEvents();

			PrintLoadedEngines();

			Logger.Log("Ready");

			if(App.Solution.IsOpen)
			{
				SolutionOpened();

				foreach (var project in App.Solution.Projects.Cast<Project>())
				{
					ProjectAdded(project);
				}
			}
		}

		void Compose()
		{
			StaticPart.App = App; // store in static property

			try
			{
				Composer = MefComposer.ComposeWithPlugins(this);
			}
			catch (Exception ex)
			{
				Logger.Log(string.Format("Error loading with plugins: {0}", ex.Message));

				if (Composer != null)
					Composer.Dispose();

				try
				{
					Logger.Log("Loading without plugins.");
					Composer = MefComposer.ComposeWithoutPlugins(this);
				}
				catch (Exception ex2)
				{
					Logger.Log(string.Format("Error loading: {0}", ex2.Message));

					if (Composer != null)
						Composer.Dispose();

					Composer = null;
				}
			}
		}

		void PrintLoadedEngines()
		{
			var engineResolver = Chirp.EngineResolver as ChirpyInterface.IEngineResolver;
			if (engineResolver == null)
				return;

			Logger.Log("Loaded Engines:");

			engineResolver.Engines
				.Select(
					e => string.Format("\t{0} [{1}]{2}",
					                   e.Metadata.Name,
					                   e.Metadata.Version,
					                   e.Metadata.Internal ? "" : " (plugin)"))
				.Distinct()
				.OrderBy(s => s)
				.ToList()
				.ForEach(Logger.Log);
		}

		/// <summary>
		/// Implements the OnStartupComplete method of the IDTExtensibility2 interface. 
		/// Occurs when the host application has completed loading.
		/// </summary>
		/// <param name="custom">Array of parameters that are host application specific.</param>
		/// <seealso cref="IDTExtensibility2"/>
		public void OnStartupComplete(ref Array custom)
		{
			BindEvents();
		}

		void BindEvents()
		{
			if (Chirp == null || HasBoundEvents)
				return;

			HasBoundEvents = true;

			Logger.Log("Binging events");

			// hold a reference to the event objects to prevent them being garbage collected
			SolutionEvents = Events.SolutionEvents;
			ProjectItemsEvents = Events.ProjectItemsEvents;
			BuildEvents = Events.BuildEvents;
			DocumentEvents = Events.DocumentEvents;

			SolutionEvents.Opened += SolutionOpened;
			SolutionEvents.ProjectAdded += ProjectAdded;
			SolutionEvents.ProjectRemoved += ProjectRemoved;
			SolutionEvents.AfterClosing += SolutionClosed;

			ProjectItemsEvents.ItemAdded += ItemAdded;
			ProjectItemsEvents.ItemRemoved += ItemRemoved;
			ProjectItemsEvents.ItemRenamed += ItemRenamed;

			BuildEvents.OnBuildDone += BuildDone;

			DocumentEvents.DocumentSaved += DocumentSaved;
      DocumentEvents.DocumentClosing += DocumentClosed;
    }

		/// <summary>
		/// Implements the OnDisconnection method of the IDTExtensibility2 interface. 
		/// Occurs when the Add-in is being unloaded.
		/// </summary>
		/// <param name="disconnectMode">Describes how the Add-in is being unloaded.</param>
		/// <param name="custom">Array of parameters that are host application specific.</param>
		/// <seealso cref="IDTExtensibility2"/>
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
			if (Composer != null)
				Composer.Dispose();

			Logger.Log("Goodbye!");
		}

		/// <summary>
		/// Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. 
		/// Occurs when the collection of Add-ins has changed.
		/// </summary>
		/// <param name="custom">Array of parameters that are host application specific.</param>
		/// <seealso cref="IDTExtensibility2"/>
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>
		/// Implements the OnBeginShutdown method of the IDTExtensibility2 interface. 
		/// Occurs when the host application beings unloaded.
		/// </summary>
		/// <param name="custom">Array of parameters that are host application specific.</param>
		/// <seealso cref="IDTExtensibility2"/>
		public void OnBeginShutdown(ref Array custom)
		{
		}

		void SolutionOpened()
		{
			ProjectItemManager.SolutionOpened();
		}

		void ProjectAdded(Project project)
		{
			ProjectItemManager.ProjectAdded(project);
		}

		void ProjectRemoved(Project project)
		{
			ProjectItemManager.ProjectRemoved(project);
		}

		void SolutionClosed()
		{
			
		}

		void ItemAdded(ProjectItem projectItem)
		{
			ProjectItemManager.ItemAdded(projectItem);
		}

		void ItemRemoved(ProjectItem projectItem)
		{
			ProjectItemManager.ItemRemoved(projectItem);
		}

		void ItemRenamed(ProjectItem projectItem, string oldname)
		{
			ProjectItemManager.ItemRenamed(projectItem, oldname);
		}

		void BuildDone(vsBuildScope scope, vsBuildAction action)
		{
			
		}

		void DocumentSaved(Document document)
		{
			ProjectItemManager.ItemSaved(document.ProjectItem);
		}

    void DocumentClosed(Document document)
    {
      ProjectItemManager.ItemClosed(document.ProjectItem);
    }
	}
}
