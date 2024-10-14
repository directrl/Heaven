using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coeli.Configuration {
	
	public class GameOptions {
		
		public const string FORMAT = ".json";
		
		private readonly JObject _json;
		
		public FileInfo ConfigFile { get; }

		public GameOptions(string configFilePath) {
			var configFile = new FileInfo(configFilePath);
			
			if(!configFile.Exists) {
				using(var writer = configFile.CreateText()) {
					writer.WriteLine("{}");
				}
			}

			ConfigFile = configFile;
			_json = JObject.Parse(File.ReadAllText(configFile.FullName));
		}

		~GameOptions() {
			Save();
		}

		public bool Has(string key) => _json.ContainsKey(key);
		
		public T? Get<T>(string key) {
			var value = _json[key];
			if(value != null) return value.Value<T>();
			return default;
		}
		
		public T GetOrDefault<T>(string key, T defaultValue) {
			var value = _json[key];
			if(value != null) return value.Value<T>();

			_json[key] = JToken.FromObject(defaultValue);
			return defaultValue;
		}
		
		public T GetObject<T>(string key) {
			var value = _json[key];
			if(value != null) return value.ToObject<T>();
			return default;
		}
		
		public T GetObjectOrDefault<T>(string key, T defaultValue) {
			var value = _json[key];
			if(value != null) return value.ToObject<T>();

			_json[key] = JToken.FromObject(defaultValue);
			return defaultValue;
		}

		public void Set<T>(string key, T value) {
			_json[key] = JToken.FromObject(value);
		}
		
		public void SetObject<T>(string key, T value) {
			_json[key] = JToken.FromObject(value);
		}

		public void Save() {
			using(var writer = ConfigFile.CreateText())
			using(var json = new JsonTextWriter(writer)) {
				json.Indentation = 4;
				json.Formatting = Formatting.Indented;
				
				_json.WriteTo(json);
			}
		}
	}
}