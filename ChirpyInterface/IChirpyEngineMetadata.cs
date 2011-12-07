namespace ChirpyInterface
{
	public interface IChirpyEngineMetadata
	{
		string Name { get; }
		string Category { get; }
		bool Internal { get; }
		string DefaultExtension { get; }
	}
}