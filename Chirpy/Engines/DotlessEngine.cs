namespace Chirpy.Engines
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using ChirpyInterface;
	using dotless.Core.Importers;
	using dotless.Core.Parser;
	using dotless.Core.Stylizers;

	[Export(typeof (IEngine))]
	[EngineMetadata("Dotless", "less", "css", true)]
	public class DotlessEngine : IEngine
	{
		public List<string> GetDependancies(string contents, string filename)
		{
			return Try(p =>
				{
					p.Parse(contents, filename);
					return p.Importer.Imports;
				});
		}

		public string Process(string contents, string filename)
		{
			return Try(p => p.Parse(contents, filename).AppendCSS());
		}
	

		T Try<T>(Func<Parser, T> action)
		{
			Parser parser = null;
			try
			{
				parser = new Parser(new ChirpyStyaliser(), new Importer());
				return action(parser);
			}
			catch (Exception e)
			{
				if(parser == null)
					throw;

				var stylizer = parser.Stylizer as ChirpyStyaliser;

				if (stylizer == null || stylizer.LastZone == null)
					throw;

				var zone = stylizer.LastZone;

				throw new ChirpyException(zone.Message, zone.FileName, zone.LineNumber, zone.Position, zone.Extract.Line);
			}
		}

		class ChirpyStyaliser : IStylizer
		{
			public Zone LastZone { get; private set; }

			public string Stylize(Zone zone)
			{
				LastZone = zone;

				return new PlainStylizer().Stylize(zone);
			}
		}
	}
}
