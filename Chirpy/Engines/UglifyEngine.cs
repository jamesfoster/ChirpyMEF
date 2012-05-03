namespace Chirpy.Engines
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using ChirpyInterface;

	[Export(typeof (IEngine))]
	[EngineMetadata("Uglify", "1.0", ".js", true)]
  public class UglifyEngine : IEngine
	{
		[Import] public IJavascriptRunner JavascriptRunner { get; set; }
 

    List<EngineResult> IEngine.Process(string contents, string filename)
    {
      var result = new EngineResult { FileName = filename, Extension = "min.js" };
      var properties = new Dictionary<string, object>();
      properties["js"] = contents;

      const string script = @"
var uglify = require('https://raw.github.com/mishoo/UglifyJS/master/uglify-js.js');
var js = external.Get('js');
var result = uglify(js);
external.Set('result', result);
";

			try 
      {
        var javascriptResult = JavascriptRunner.Execute(script, properties);
        result.Contents = javascriptResult.Get("result") as string;
        result.Exceptions.AddRange(javascriptResult.Messages);

      } catch (ChirpyException cex) {
        result.Exceptions.Add(cex);
      }

      return new List<EngineResult> { result };
    }

    public List<string> GetDependencies(string contents, string filename) 
    {
      return null;
    }
  }
}