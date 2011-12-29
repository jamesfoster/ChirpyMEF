namespace Chirpy.Engines
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Linq;
	using System.Xml.Linq;
	using ChirpyInterface;

	[Export(typeof(IEngine))]
	[EngineMetadata("Config", "2.0", "config", true)]
	public class ConfigEngine : IEngine
	{
		[Import] public IFileHandler FileHandler { get; set; }
		[Import] public IEngineResolver EngineResolver { get; set; }

		public List<string> GetDependancies(string contents, string filename)
		{
			var doc = GetXml(contents);

			var paths = GetFiles(doc);

			return paths.ToList();
		}

		public List<EngineResult> Process(string contents, string filename)
		{
			var doc = GetXml(contents);

			IEnumerable<XElement> fileGroups = doc.Descendants("FileGroup").ToList();

			if(fileGroups.Count() == 0)
				fileGroups = new[] {new XElement("FileGroup", new XAttribute("Path", filename), doc)};

			return fileGroups
				.SelectMany(fileGroup => GetEngineResult(fileGroup, filename))
				.ToList();
		}

		IEnumerable<EngineResult> GetEngineResult(XElement fileGroup, string filename)
		{
			if (fileGroup.Attribute("Path") == null)
			{
				var result = new EngineResult();
				result.AddException("FileGroup element requires a Path attribute", filename);

				yield return result;
				yield break;
			}

			var fileGroupPath =  (string) fileGroup.Attribute("Path");

			var files = GetFiles(fileGroup);

			// get the contents of all files and join them together
			var fileGroupContents =
				string.Join("\n", files
				                  	.Select(file => FileHandler.GetAbsoluteFileName(file, filename))
				                  	.Select(path => FileHandler.GetContents(path)));

			yield return new EngineResult
			             	{
			             		FileName = fileGroupPath,
			             		Contents = fileGroupContents
			             	};

			var engine = EngineResolver.GetEngineByFilename(fileGroupPath);

			if(engine == null)
				yield break;

			var engineResults = engine.Process(fileGroupContents, fileGroupPath);

			foreach (var engineResult in engineResults)
			{
				if(string.IsNullOrEmpty(engineResult.FileName))
					engineResult.FileName = fileGroupPath;

				yield return engineResult;
			}
		}

		static IEnumerable<string> GetFiles(XElement element)
		{
			return from el in element.Descendants("File")
			       where el.Attribute("Path") != null
			       let path = (string) el.Attribute("Path")
			       where !string.IsNullOrEmpty(path)
			       select path;
		}

		static XElement GetXml(string contents)
		{
			return XElement.Parse(contents);
		}
	}
}