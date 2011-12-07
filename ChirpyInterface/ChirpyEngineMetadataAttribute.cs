namespace ChirpyInterface
{
	using System;
	using System.ComponentModel.Composition;

	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Module,
		AllowMultiple = false)]
	public class ChirpyEngineMetadataAttribute : Attribute, IChirpyEngineMetadata
	{
		public ChirpyEngineMetadataAttribute(string name, string category, string defaultExtension)
			: this(name, category, defaultExtension, false)
		{
		}

		internal ChirpyEngineMetadataAttribute(string name, string category, string defaultExtension, bool @internal)
		{
			Name = name;
			Category = category;
			DefaultExtension = defaultExtension;
			Internal = @internal;
		}

		public string Name { get; private set; }
		public string Category { get; private set; }
		public string DefaultExtension { get; private set; }
		public bool Internal { get; private set; }
	}
}