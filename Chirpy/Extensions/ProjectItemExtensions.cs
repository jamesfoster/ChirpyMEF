namespace Chirpy.Extensions
{
	using System;
	using EnvDTE;

	public static class ProjectItemExtensions
	{
		public static string FileName(this ProjectItem item)
		{
			try
			{
				//regular project
				return item.FileNames[1];
			}
			catch (Exception)
			{
				//VS.Php
				return item.FileNames[0];
			}
		}

		public static bool IsFolder(this ProjectItem item)
		{
			return item.Kind == Constants.vsProjectItemKindPhysicalFolder;
		}

		public static bool IsSolutionFolder(this ProjectItem item)
		{
			return item.SubProject != null;
		}
	}
}
