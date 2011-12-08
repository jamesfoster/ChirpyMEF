namespace ChirpyTest.ContainerSpecs
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition.Hosting;
	using System.ComponentModel.Composition.Primitives;
	using System.Linq;
	using ChirpyInterface;

	public class ListExportProvider : ExportProvider
	{
		IDictionary<ExportDefinition, object> exports = new Dictionary<ExportDefinition, object>(); 

		public void AddExport<T>(T export)
		{
			AddExport(export, null);
		}

		public void AddExport<T>(T export, IDictionary<string, object> metadata)
		{
			var contractName = typeof (T).FullName;

			if(metadata == null)
				metadata = new Dictionary<string, object>();

			metadata["ExportTypeIdentity"] = contractName;

			var definition = new ExportDefinition(contractName, metadata);

			exports[definition] = export;
		}

		public void AddEngine(IChirpyEngine engine)
		{
			var metadata = (ChirpyEngineMetadataAttribute) engine.GetType().GetCustomAttributes(typeof (ChirpyEngineMetadataAttribute), false)[0];

			AddEngine(engine, metadata);
		}

		public void AddEngine(IChirpyEngine engine, ChirpyEngineMetadataAttribute chripyMetadata)
		{
			var metadata = new Dictionary<string, object>();

			metadata["Name"] = chripyMetadata.Name;
			metadata["Category"] = chripyMetadata.Category;
			metadata["SubCategory"] = chripyMetadata.SubCategory;
			metadata["Internal"] = chripyMetadata.Internal;

			AddExport(engine, metadata);
		}

		protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
		{
			var condition = definition.Constraint.Compile();

			var result = exports.Keys
				.Where(condition)
				.Select(d => new Export(d, () => exports[d]));

			return result;
		}
	}
}