using System.Text;
using Coelum.Configuration;
using Coelum.Debug;
using Coelum.Resources;
using Serilog;

namespace Coelum.Phoenix.OpenGL {
	
	public static class ShaderPreprocessor {

		public static void Preprocess(this Shader shader, ResourceManager resources) {
			Log.Debug("[SHADER PREPROCESSOR] Starting preprocessing");

			shader.Code = Definitions(shader);
			
		#region First pass
			string newCode = "";
			
			foreach(string line in shader.Code.Replace("\r\n", "\n").Split('\n')) {
				newCode += line.Replace("//$", "//-") + "\n";
				if(string.IsNullOrWhiteSpace(line)) continue;
				
				newCode += Include(shader, line.Trim(), resources);
				newCode += Overlay(shader, line.Trim());
			}
			
			shader.Code = newCode;
		#endregion
			
		#region Second pass
			int pass = 1;
			
			while(shader.Code.Contains("//$include")) {
				if(pass > 20) {
					throw new Exception("More than 20 shader preprocessor include passes detected. "
					                    + "Check for possible recursive includes");
				}
				
				newCode = "";
				
				foreach(string line in shader.Code.Replace("\r\n", "\n").Split('\n')) {
					newCode += line.Replace("//$", "//-") + "\n";
					if(string.IsNullOrWhiteSpace(line)) continue;
					newCode += Include(shader, line.Trim(), resources);
				}
				
				shader.Code = newCode;
				pass++;
			}
		#endregion
			
			Log.Debug("[SHADER PREPROCESSOR] Finished");

			if(Debugging.DumpShaders && Debugging.Enabled) {
				string shaderDumpDir = Path.Combine(Directories.DataRoot, "shader_dump");
				Directory.CreateDirectory(shaderDumpDir);

				string dumpFilename = $"{new Random().NextInt64()}.{shader.Type}.glsl";
				
				File.WriteAllText(
					Path.Combine(shaderDumpDir, dumpFilename),
					shader.Code
				);
				
				Console.WriteLine($"Dumped {shader.Type} shader to {dumpFilename}");
			}
		}

		private static string Definitions(Shader shader) {
			StringBuilder sb = new();
					
			foreach((string key, string value) in shader._definitions) {
				sb.Append("#define ");
				sb.Append(key);
				sb.Append(" ");
				sb.Append(value);
				sb.Append('\n');
				
				Log.Debug($"[SHADER PREPROCESSOR/DEFINES] {key}={value}");
			}

			return shader.Code.Replace("//$preprocessor-defines", sb.ToString());
		}

		private static string Include(Shader shader, string line, ResourceManager resources) {
			const string token = "//$include ";

			string result = "";
			
			if(line.StartsWith(token)) {
				string includeName = line.Replace(token, "");
				var resource = resources[ResourceType.SHADER, includeName];

				string? code = resource.ReadString();
				if(code == null) {
					throw new Shader.PreprocessingException("Could not read resource for included shader");
				}

				Log.Debug($"[SHADER PREPROCESSOR/INCLUDES] " +
				            $"Included shader [{includeName}]");
				result = code + "\n";
			}

			return result;
		}

		private static string Overlay(Shader shader, string line) {
			const string tokenHeader = "//$overlay_headers";
			const string tokenCall = "//$overlay_call";

			string[] args = line.Split(" ");
			string result = "";

			int totalHeaders = 0;
			int totalCalls = 0;

			switch(args[0]) {
				case tokenHeader:
					foreach(var overlay in shader._overlays) {
						if(overlay.HasCall) {
							result += $"uniform bool u_overlay_{overlay.Name};\n";
						}
						
						if(overlay.HasHeader) {
							result += Include(
								shader,
								$"//$include {overlay.Path}.header.{overlay.GetExtension()}",
								overlay.ResourceManager
							);
							result += "\n";
							
							totalHeaders++;
						}
					}
					break;
				case tokenCall:
					var pass = new ShaderPass(args[1]);

					foreach(var overlay in shader._overlays) {
						if(overlay.HasCall && overlay.Pass.Name == pass.Name) {
							result += $"if(u_overlay_{overlay.Name}) {{\n";
							result += Include(
								shader,
								$"//$include {overlay.Path}.call.{overlay.GetExtension()}",
								overlay.ResourceManager
							);
							result += "}\n";
							
							totalCalls++;
						}
					}
					break;
			}

			if(totalHeaders > 0 || totalCalls > 0) {
				Log.Debug("[SHADER PREPROCESSOR/OVERLAYS] " +
				            $"totalHeaders: {totalHeaders}, totalCalls: {totalCalls}");
			}
			return result;
		}
	}
}