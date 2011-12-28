namespace ChirpyInterface
{
	using System;
	using System.ComponentModel.Composition;

	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Module,
		AllowMultiple = false)]
	public class EngineMetadataAttribute : Attribute, IEngineMetadata
	{
		public EngineMetadataAttribute(string name, string version, string category)
			: this(name, version, category, false)
		{
		}

		internal EngineMetadataAttribute(string name, string version, string category, bool @internal)
		{
			Name = name;
			Version = version;
			Category = category;
			Internal = @internal;
		}

		public string Name { get; private set; }
		public string Version { get; private set; }
		public string Category { get; private set; }
		public bool Internal { get; private set; }
		public bool Minifier { get; set; }
	}
}