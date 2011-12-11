﻿namespace ChirpyInterface
{
	using System.Collections.Generic;

	public interface IEngine
	{
		List<string> GetDependancies(string contents, string filename);
		string Process(string contents, string filename);
	}
}