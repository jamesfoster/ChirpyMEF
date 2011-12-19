namespace ChirpyInterface
{
	using System;
	using System.ComponentModel.Composition;

	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Module,
		AllowMultiple = false)]
	public class EngineMetadataAttribute : Attribute, IEngineMetadata
	{
		public EngineMetadataAttribute(string name, string version, string category, string outputCategory)
			: this(name, version, category, outputCategory, false)
		{
		}

		internal EngineMetadataAttribute(string name, string version, string category, string outputCategory, bool @internal)
		{
			Name = name;
			Version = version;
			Category = category;
			OutputCategory = outputCategory;
			Internal = @internal;
		}

		public string Name { get; private set; }
		public string Version { get; private set; }
		public string Category { get; private set; }
		public string OutputCategory { get; private set; }
		public bool Internal { get; private set; }
		public bool Minifier { get; set; }
	}
}