namespace ChirpyInterface
{
	public interface IChirpyEngineMetadata
	{
		string Name { get; }
		string Category { get; }
		string SubCategory { get; }
		bool Internal { get; }
	}
}