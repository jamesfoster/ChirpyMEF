namespace ChirpyInterface
{
	public interface IEngineMetadata
	{
		string Name { get; }
		string Category { get; }
		string OutputCategory { get; }
		bool Internal { get; }
		bool Minifier { get; }
	}
}