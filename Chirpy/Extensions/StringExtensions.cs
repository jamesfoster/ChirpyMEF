namespace Chirpy.Extensions
{
	using System;

	public static class StringExtensions
	{
		public static bool Is(this string a, string b)
		{
			return string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}