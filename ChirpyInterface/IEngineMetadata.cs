namespace ChirpyInterface
{
	public interface IEngineMetadata
	{
		string Name { get; }
		string Version { get; }
		string Category { get; }
		bool Internal { get; }
		bool Minifier { get; }
	}
}