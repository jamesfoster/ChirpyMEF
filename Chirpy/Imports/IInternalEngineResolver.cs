namespace Chirpy.Imports
{
	public interface IInternalEngineResolver
	{
		EngineContainer GetEngine(string category);
		EngineContainer GetEngineByName(string name);
		EngineContainer GetEngineByFilename(string filename);
	}
}