namespace Chirpy
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Windows.Forms;
	using EnvDTE;
	using EnvDTE80;
	using Extensibility;

	public class ChirpyAddIn : IDTExtensibility2
	{
		const string OutputWindowName = "Chirpy";

		static readonly object @lock = new object();

		protected DTE2 App { get; set; }
		protected AddIn Instance { get; set; }
		protected Events2 Events { get; set; }
		protected Chirp Chirp { get; set; }

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

			try
			{
				Chirp = Chirp.CreateWithPlugins();
			}
			catch(Exception)
			{
				Chirp = Chirp.CreateWithoutPlugins();
			}

			WriteToOutputWindow("Ready");
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
		}

		/// <summary>
		/// Implements the OnStartupComplete method of the IDTExtensibility2 interface. 
		/// Occurs when the host application has completed loading.
		/// </summary>
		/// <param name="custom">Array of parameters that are host application specific.</param>
		/// <seealso cref="IDTExtensibility2"/>
		public void OnStartupComplete(ref Array custom)
		{
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
