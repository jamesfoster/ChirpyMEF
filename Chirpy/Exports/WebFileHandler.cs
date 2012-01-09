namespace Chirpy.Exports
{
	using System;
	using System.ComponentModel.Composition;
	using System.IO;
	using System.Net;
	using ChirpyInterface;
	using System.Linq;
	using Logging;

	[Export(typeof(IWebFileHandler))]
	public class WebFileHandler : IWebFileHandler
	{
		readonly object locker = new object();

		[Import] public IFileHandler FileHandler { get; set; }
		[Import] public ILogger Logger { get; set; }

		public string GetContents(string filename)
		{
			Uri uri;
			if (!Uri.TryCreate(filename, UriKind.Absolute, out uri))
				return GetLocalFileContents(filename);

			if (uri.Scheme == Uri.UriSchemeFile)
				return GetLocalFileContents(uri.LocalPath);

//			if (uri.Scheme == Uri.UriSchemeFtp)
//				return GetFtpFileContents(uri);

			if(uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
				return GetWebFileContents(uri);

			throw new InvalidOperationException(string.Format("Unsupported uri scheme '{0}'", uri.Scheme));
		}

		string GetLocalFileContents(string filename)
		{
			var contents = FileHandler.GetContents(filename);
			if (contents != null)
				return contents;

			throw new FileNotFoundException(null, filename);
		}

		string GetFtpFileContents(Uri uri)
		{
//			var ftpRequest = (FtpWebRequest) WebRequest.Create(uri);

			throw new NotImplementedException();
		}

		string GetWebFileContents(Uri uri)
		{
			var tempFilePath = GetTempFilePath(uri);
			var tempFileExists = FileHandler.FileExists(tempFilePath);

			var lastAccessed = tempFileExists ? File.GetLastWriteTimeUtc(tempFilePath) : (DateTime?)null;

			// if accessed less than 1 hour ago return cache
			if(tempFileExists && lastAccessed.Value.AddHours(1) > DateTime.UtcNow)
				return FileHandler.GetContents(tempFilePath);

			var webRequest = (HttpWebRequest) WebRequest.Create(uri);

			webRequest.AllowAutoRedirect = true;
			webRequest.AutomaticDecompression = DecompressionMethods.GZip;
			if(lastAccessed.HasValue)
				webRequest.IfModifiedSince = lastAccessed.Value;

			using (var webResponse = GetResponse(webRequest))
			{
				if (webResponse == null)
					return null;

				switch (webResponse.StatusCode)
				{
					case HttpStatusCode.OK:
						Download(webResponse, tempFilePath);
						break;

					case HttpStatusCode.NotModified:
						// Don't check again for an hour
						if (tempFileExists)
							File.SetLastWriteTime(tempFilePath, DateTime.UtcNow);
						break;

					default:
						// Something bad happened. Don't check again for 15 minutes
						if (tempFileExists)
							File.SetLastWriteTime(tempFilePath, DateTime.UtcNow.AddMinutes(-45));
						break;
				}
			}

			return FileHandler.GetContents(tempFilePath);
		}

		void Download(HttpWebResponse webResponse, string tempFilePath)
		{
			lock (locker)
			{
				using (var stream = webResponse.GetResponseStream())
				using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
				{
					var data = new byte[8096];
					int read;
					do
					{
						read = stream.Read(data, 0, data.Length);
						fileStream.Write(data, 0, read);
					} while (read > 0);
				}
			}
		}

		static string GetTempFilePath(Uri uri)
		{
			var tempPath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);

			var validFileName = Path.GetInvalidFileNameChars()
				.Aggregate(uri.ToString(), (s, c) => s.Replace(c, '_'));

			return Path.Combine(tempPath, validFileName);
		}

		static HttpWebResponse GetResponse(HttpWebRequest req)
		{
			try
			{
				return (HttpWebResponse) req.GetResponse();
			}
			catch (WebException ex)
			{
				return (HttpWebResponse) ex.Response;
			}
		}

		public string GetAbsoluteFileName(string path, string relativeTo)
		{
			Uri uri;
			if (Uri.TryCreate(path, UriKind.Absolute, out uri))
				return path;

			if (!Uri.TryCreate(relativeTo, UriKind.Absolute, out uri)
				|| !Uri.TryCreate(uri, path, out uri))
				throw new InvalidOperationException(string.Format("Unable to get absolute url of '{0}' relative to '{1}'", path, relativeTo));

			return uri.ToString();
		}
	}
}