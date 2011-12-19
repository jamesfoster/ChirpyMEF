namespace Chirpy
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Windows.Forms;
	using EnvDTE;
	using EnvDTE80;
	using Exports;
	using Extensibility;
	using Extensions;

	public class ChirpyAddIn : IDTExtensibility2
	{
		const string OutputWindowName = "Chirpy";

		static readonly object @lock = new object();

		protected DTE2 App { get; set; }
		protected AddIn Instance { get; set; }
		protected Events2 Events { get; set; }
		protected Chirp Chirp { get; set; }

		protected DocumentEvents DocumentEvents { get; set; }
		protected BuildEvents BuildEvents { get; set; }
		protected ProjectItemsEvents ProjectItemsEvents { get; set; }
		protected SolutionEvents SolutionEvents { get; set; }

		public bool HasBoundEvents { get; set; }

		OutputWindowPane outputWindowPane;

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

			WriteToOutputWindow("Loading...");

			ComposeChirp();

			if(Chirp == null)
			{
				WriteToOutputWindow("Unable to load.");
				return;
			}

			BindEvents();

			PrintLoadedEngines();

			WriteToOutputWindow("Ready");
		}

		void ComposeChirp()
		{
			StaticPart.App = App; // store in static property

			try
			{
				Chirp = Chirp.CreateWithPlugins();
			}
			catch (Exception ex)
			{
				WriteToOutputWindow(string.Format("Error loading with plugins: {0}", ex.Message));

				if (Chirp != null)
					Chirp.Dispose();

				try
				{
					WriteToOutputWindow("Loading without plugins.");
					Chirp = Chirp.CreateWithoutPlugins();
				}
				catch (Exception ex2)
				{
					WriteToOutputWindow(string.Format("Error loading: {0}", ex2.Message));

					if (Chirp != null)
						Chirp.Dispose();

					Chirp = null;
				}
			}
		}

		void PrintLoadedEngines()
		{
			var engineResolver = Chirp.EngineResolver as EngineResolver;
			if (engineResolver == null)
				return;

			WriteToOutputWindow("Loaded Engines:");

			engineResolver.Engines
				.Select(e => string.Format("\t{0}", e.Metadata.Name))
				.Distinct()
				.OrderBy(s => s)
				.ToList()
				.ForEach(WriteToOutputWindow);
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

			// hold a reference to the event objects to prevent them being garbage collected
			SolutionEvents = Events.SolutionEvents;
			ProjectItemsEvents = Events.ProjectItemsEvents;
			BuildEvents = Events.BuildEvents;
			DocumentEvents = Events.DocumentEvents;

			Events.SolutionEvents.Opened += SolutionOpened;
			Events.SolutionEvents.ProjectAdded += ProjectAdded;
			Events.SolutionEvents.ProjectRemoved += ProjectRemoved;
			Events.SolutionEvents.AfterClosing += SolutionClosed;

			Events.ProjectItemsEvents.ItemAdded += ItemAdded;
			Events.ProjectItemsEvents.ItemRemoved += ItemRemoved;
			Events.ProjectItemsEvents.ItemRenamed += ItemRenamed;

			Events.BuildEvents.OnBuildDone += BuildDone;

			Events.DocumentEvents.DocumentSaved += DocumentSaved;
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
			if (Chirp != null)
				Chirp.Dispose();

			WriteToOutputWindow("Goodbye!");
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
			
		}

		void ProjectAdded(Project project)
		{
			
		}

		void ProjectRemoved(Project project)
		{
			
		}

		void SolutionClosed()
		{
			
		}

		void ItemAdded(ProjectItem projectItem)
		{
			// load dependancies

			// enqueue file?

			var output = Chirp.Run(projectItem.FileName());

			WriteToOutputWindow(output ?? "[null]");
		}

		void ItemRemoved(ProjectItem projectitem)
		{
			
		}

		void ItemRenamed(ProjectItem projectitem, string oldname)
		{
			
		}

		void BuildDone(vsBuildScope scope, vsBuildAction action)
		{
			
		}

		void DocumentSaved(Document document)
		{
			ItemAdded(document.ProjectItem);
		}

		void SetupOutputWindow()
		{
			var outputWindow = App.ToolWindows.OutputWindow;
			outputWindowPane = outputWindow.OutputWindowPanes.Cast<OutputWindowPane>().FirstOrDefault(x => x.Name == OutputWindowName);

			if (outputWindowPane == null)
				outputWindowPane = outputWindow.OutputWindowPanes.Add(OutputWindowName);

			outputWindowPane.Activate();
		}

		void WriteToOutputWindow(string messageText)
		{
			try
			{
				if (outputWindowPane == null)
				{
					lock (@lock)
					{
						if (outputWindowPane == null)
						{
							SetupOutputWindow();
						}
					}
				}

				Debug.Assert(outputWindowPane != null);

				outputWindowPane.OutputString(messageText + Environment.NewLine);
			}
			catch (Exception errorThrow)
			{
				MessageBox.Show(errorThrow.ToString());
			}
		}
	}
}
