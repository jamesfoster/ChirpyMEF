namespace Chirpy.Logging
{
	using System;
	using System.ComponentModel.Composition;
	using System.Linq;
	using System.Windows.Forms;
	using EnvDTE;
	using EnvDTE80;

	[Export(typeof(ILogger))]
	class OutputWindowLogger : ILogger, IPartImportsSatisfiedNotification
	{
		const string OutputWindowName = "Chirpy";

		[Import] public DTE2 App { get; set; }

		static readonly object @lock = new object();

		volatile OutputWindowPane outputWindowPane;

		public OutputWindowLogger()
		{
		}

		public OutputWindowLogger(DTE2 app)
		{
			App = app;
			OnImportsSatisfied();
		}

		public void OnImportsSatisfied()
		{
			if (outputWindowPane == null)
				lock (@lock)
					if (outputWindowPane == null)
						outputWindowPane = SetupOutputWindow();
		}

		OutputWindowPane SetupOutputWindow()
		{
			var window = App.ToolWindows.OutputWindow;
			var pane = window.OutputWindowPanes.Cast<OutputWindowPane>().FirstOrDefault(x => x.Name == OutputWindowName);

			if (pane == null)
				pane = window.OutputWindowPanes.Add(OutputWindowName);

			pane.Activate();

			return pane;
		}

		public void Log(string messageText)
		{
			try
			{
				outputWindowPane.OutputString(messageText + Environment.NewLine);
			}
			catch (Exception errorThrow)
			{
				MessageBox.Show(errorThrow.ToString());
			}
		}
	}
}