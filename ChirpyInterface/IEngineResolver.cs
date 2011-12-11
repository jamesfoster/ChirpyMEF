namespace ChirpyInterface
{
	public interface IEngineResolver
	{
		IEngine GetEngine(string category);
		IEngine GetEngineByName(string name);
		IEngine GetEngineByFilename(string filename);
	}
}
