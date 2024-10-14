using Coeli.Configuration;
using Serilog;

namespace Coeli.Core.Logging {
	
	public class LoggerFactory {
		
		public static LoggerConfiguration CreateDefaultConfiugration(LoggerPurpose purpose, string? path = null) {
			var config = new LoggerConfiguration()
			             .WriteTo.Console();

			if(path == null) {
				switch(purpose) {
					case LoggerPurpose.Engine:
						path = Path.Join(Directories.LoggingRoot, "engine");
						break;
					case LoggerPurpose.Application:
						path = Path.Join(Directories.LoggingRoot, "app");
						break;
				}

				// TODO only keep X newest logs
				path += $"_{DateTime.Now:yyyy-MM-dd+HH-mm-ss}.log";
				Console.WriteLine($"Current {purpose} log path: {path}");
			}
			
			if(!string.IsNullOrEmpty(path)) {
				config.WriteTo.File(path);
			}

			return config;
		}
	}
}