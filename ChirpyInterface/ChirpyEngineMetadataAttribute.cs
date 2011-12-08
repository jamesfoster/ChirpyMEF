namespace ChirpyInterface
{
	using System;
	using System.ComponentModel.Composition;

	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Module,
		AllowMultiple = false)]
	public class ChirpyEngineMetadataAttribute : Attribute, IChirpyEngineMetadata
	{
		public ChirpyEngineMetadataAttribute(string name, string category)
			: this(name, category, "", false)
		{
		}

		public ChirpyEngineMetadataAttribute(string name, string category, string subCategory)
			: this(name, category, subCategory, false)
		{
		}

		internal ChirpyEngineMetadataAttribute(string name, string category, bool @internal)
			: this(name, category, "", @internal)
		{
		}

		internal ChirpyEngineMetadataAttribute(string name, string category, string subCategory, bool @internal)
		{
			Name = name;
			Category = category;
			SubCategory = subCategory;
			Internal = @internal;
		}

		public string Name { get; private set; }
		public string Category { get; private set; }
		public string SubCategory { get; set; }
		public bool Internal { get; private set; }
	}
}