namespace Coelum.Configuration {
	
	public static class Directories {
		
		public static string DataRoot { get; private set; }
		public static string ConfigurationRoot { get; private set; }
		public static string LoggingRoot { get; private set; }

		public static void Resolve(string subName) {
			string? systemDataRoot = null;

			if(OperatingSystem.IsWindows()) {
				var appdata = Environment.GetEnvironmentVariable("APPDATA");
				if(!string.IsNullOrEmpty(appdata)) systemDataRoot = appdata;
			} else if(OperatingSystem.IsMacOS()) {
				var home = Environment.GetEnvironmentVariable("HOME");

				if(!string.IsNullOrEmpty(home)) {
					systemDataRoot = Path.Join(
						home, "Library", "Application Support");
				}
			} else if(OperatingSystem.IsLinux()) {
				var data = Environment.GetEnvironmentVariable("XDG_DATA_HOME");

				if(string.IsNullOrEmpty(data)) {
					data = Environment.GetEnvironmentVariable("HOME");

					if(!string.IsNullOrEmpty(data)) {
						systemDataRoot = Path.Join(data, ".local", "share");
					}
				} else {
					systemDataRoot = data;
				}
			}

			if(string.IsNullOrEmpty(systemDataRoot)) {
				throw new SystemException("Could not get the system data root");
			}

			DataRoot = Directory.CreateDirectory(Path.Combine(systemDataRoot,
				EngineOptions.BASENAME, subName)).FullName;
			
			ConfigurationRoot = Directory.CreateDirectory(Path.Join(DataRoot, "config")).FullName;
			LoggingRoot = Directory.CreateDirectory(Path.Join(DataRoot, "logs")).FullName;
		}

		public static bool Create(string primary, string sub) {
			var path = Path.Join(primary, sub);
			if(Directory.Exists(path)) return false;

			Directory.CreateDirectory(path);
			return true;
		}
	}
}