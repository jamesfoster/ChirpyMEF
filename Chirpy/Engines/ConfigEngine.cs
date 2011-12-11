namespace Chirpy.Engines
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Linq;
	using System.Xml.Linq;
	using ChirpyInterface;

	[Export(typeof(IEngine))]
	[EngineMetadata("Config", "config", "", true)]
	public class ConfigEngine : IEngine
	{
		[Import] public ITaskList TaskList { get; set; }
		[Import] public IProjectItemManager ProjectItemManager { get; set; }
		[Import] public IFileHandler FileHandler { get; set; }
		[Import] public IEngineResolver EngineResolver { get; set; }

		public List<string> GetDependancies(string contents, string filename)
		{
			var doc = GetXml(contents);

			var paths = GetFiles(doc);

			return paths.ToList();
		}

		public string Process(string contents, string filename)
		{
			var doc = GetXml(contents);

			IEnumerable<XElement> fileGroups = doc.Descendants("FileGroup").ToList();

			if(fileGroups.Count() == 0)
				fileGroups = new[] {new XElement("FileGroup", new XAttribute("Path", filename), doc)};

			foreach (var fileGroup in fileGroups)
			{
				var fileGroupPath = fileGroup.Attribute("Path") == null ? null : (string) fileGroup.Attribute("Path");

				if(string.IsNullOrEmpty(fileGroupPath))
				{
					// log error
					// TaskList.Add(filename);

					continue;
				}

				var files = GetFiles(fileGroup);

				// get the contents of all files and join them together
				var fileGroupContents =
					string.Join("\n", files
					                  	.Select(file => FileHandler.GetFileName(file, filename))
					                  	.Select(path => FileHandler.GetContents(path)));

				return fileGroupContents; // DELETE ME (debug purposes only)

				// var engine = EngineResolver.GetEngineForFile(fileGroupPath);

				// var dependancies = engine.GetDependancies(fileGroupContents, fileGroupPath);
				// FileHandler.RefreshMany(dependancies);

				// fileGroupContents = engine.Process(fileGroupContents, fileGroupPath);

				// ProjectItemManager.AddFile(fileGroupPath, filename, fileGroupContents);
			}

			return null; // bypass normal processing
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