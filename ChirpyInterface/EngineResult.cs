namespace ChirpyInterface
{
	using System.Collections.Generic;

	public class EngineResult
	{
		public string Category { get; set; }
		public string FileName { get; set; }
		public string Contents { get; set; }
		public List<ChirpyException> Exceptions { get; set; }
	}
}