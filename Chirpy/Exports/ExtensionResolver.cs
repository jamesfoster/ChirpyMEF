namespace Chirpy.Exports
{
	using System.ComponentModel.Composition;
	using Imports;

	[Export(typeof(IExtensionResolver))]
	class ExtensionResolver : IExtensionResolver
	{
		public string GetExtensionFromCategory(string category)
		{
			if(!category.Contains("."))
				return ".chirp." + category;

			return "." + category;
		}
	}
}