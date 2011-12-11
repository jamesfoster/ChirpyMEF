namespace Chirpy.Imports
{
	internal interface IExtensionResolver
	{
		string GetExtensionFromCategory(string category);
		string GetCategoryFromExtension(string extension);
	}
}
