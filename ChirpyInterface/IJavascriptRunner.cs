namespace ChirpyInterface
{
	using System.Collections.Generic;

	public interface IJavascriptRunner
	{
		JavascriptResult Execute(string script, Dictionary<string, object> properties = null);
	}
}