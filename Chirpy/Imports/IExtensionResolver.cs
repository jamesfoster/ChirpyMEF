namespace Chirpy.Imports
{
	public interface IExtensionResolver
	{
		string GetExtensionFromCategory(string category);
		string GetCategoryFromExtension(string extension);
	}
}
