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
	}
}
