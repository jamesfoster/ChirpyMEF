namespace Chirpy.Exports
{
	using System.ComponentModel.Composition;
	using Imports;

	[Export(typeof(IExtensionResolver))]
	class ExtensionResolver : IExtensionResolver
	{
		public string GetExtensionFromCategory(string category)
		{
			return "." + category;
		}

		public string GetCategoryFromExtension(string extension)
		{
			return extension.Trim('.');
		}
	}
}