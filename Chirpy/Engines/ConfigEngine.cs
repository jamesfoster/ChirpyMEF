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
			var result = new EngineResult();

			if (fileGroup.Attribute("Path") == null)
			{
				result.AddException("FileGroup element requires a Path attribute", filename, ErrorCategory.Error);

				yield return result;
				yield break;
			}

			var fileGroupPath = (string) fileGroup.Attribute("Path");

			var files = GetFiles(fileGroup)
				.Select(file => FileHandler.GetAbsoluteFileName(file, filename))
				.ToLookup(FileHandler.FileExists);

			// add a warning for each missing file
			foreach (var file in files[false])
			{
				result.AddException(string.Format("File does not exist '{0}'", file), filename, ErrorCategory.Warning);
			}

			// get the contents of all existing files and join them together
			var fileGroupContents = string.Join("\n", files[true].Select(file => FileHandler.GetContents(file)));

			result.FileName = fileGroupPath;
			result.Contents = fileGroupContents;

			yield return result;

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