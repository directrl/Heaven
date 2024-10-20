using System.Diagnostics;
using System.Numerics;
using Coelum.Debug;
using Coelum.Resources;
using Serilog;
using Silk.NET.Core;
using Silk.NET.OpenGL;

namespace Coelum.Graphics.OpenGL {
	
	public class ShaderProgram : IDisposable {

		private readonly GL _gl;
		private readonly Shader[] _program;
		private readonly List<IShaderOverlay> _overlays = new();
		
		private readonly ResourceManager? _preprocessorResources;
		
		private bool _ready;
		private bool _bound;
		
		public uint Id { get; }

		public string UniformPrefix { get; set; } = "u_";
		public string UniformSuffix { get; set; } = "";

		public ShaderProgram(ResourceManager? preprocessorResources, params Shader[] program) {
			_preprocessorResources = preprocessorResources;
			
			_gl = GLManager.Current;
			Id = _gl.CreateProgram();

			if(Id == 0) {
				throw new PlatformException("Could not create a GL shader program");
			}

			_program = program;
		}
		
		public void AddOverlays(params IShaderOverlay[] overlays)
			=> EnableOverlays((IEnumerable<IShaderOverlay>) overlays);

		public void AddOverlays(IEnumerable<IShaderOverlay> overlays) {
			Tests.Assert(!_ready, "Can't add overlays to a shader that has already been built");

			foreach(var overlay in overlays) {
				_overlays.Add(overlay);
			}
		}

		public void EnableOverlays(params IShaderOverlay[] overlays)
			=> EnableOverlays((IEnumerable<IShaderOverlay>) overlays);
		
		public void EnableOverlays(IEnumerable<IShaderOverlay> overlays) {
			Tests.Assert(_bound, "Can't enable overlays for a shader that is not bound");

			foreach(var overlay in overlays) {
				Tests.Assert(SetUniform($"overlay_{overlay.Name}", true),
				             $"Overlay [{overlay.Name}] not included in shader");
			
				overlay.Load(this);
			}
		}
		
		public void DisableOverlays(params IShaderOverlay[] overlays)
			=> DisableOverlays((IEnumerable<IShaderOverlay>) overlays);
		
		public void DisableOverlays(IEnumerable<IShaderOverlay> overlays) {
			Tests.Assert(_bound, "Can't enable overlays for a shader that is not bound");

			foreach(var overlay in overlays) {
				Tests.Assert(SetUniform($"overlay_{overlay.Name}", false),
				             $"Overlay [{overlay.Name}] not included in shader");
			}
		}

		public void Build() {
			List<uint> shaderIds = new();

			var sw = Stopwatch.StartNew();

			foreach(var shader in _program) {
				var overlays = _overlays
				               .Where(overlay => overlay != null && overlay.Type == shader.Type)
				               .ToArray();
				shader.Overlays = overlays;

				if(_preprocessorResources != null) {
					shader.Preprocess(_preprocessorResources);
				} else {
					Log.Warning("[SHADER PROGRAM] " +
					            "Skipping preprocessing on shader due to no ResourceManager provided");
				}
				
				var shaderId = shader.Compile(_gl);

				if(shaderId != 0) {
					_gl.AttachShader(Id, shaderId);
					shaderIds.Add(shaderId);
				}
			}

			_gl.LinkProgram(Id);

			if(_gl.GetProgram(Id, GLEnum.LinkStatus) == 0) {
				throw new LinkingException(_gl, Id);
			}
			
			// cleanup
			foreach(var shaderId in shaderIds) {
				_gl.DetachShader(Id, shaderId);
				_gl.DeleteShader(shaderId);
			}

			_ready = true;
			
			sw.Stop();
			Log.Information($"[SHADER PROGRAM] Building took {sw.ElapsedMilliseconds}ms");
		}

		public void Validate() {
			_gl.ValidateProgram(Id);

			if(_gl.GetProgram(Id, GLEnum.ValidateStatus) != 0) {
				throw new ValidationException(_gl, Id);
			}
		}

		public void Bind() {
			if(!_ready) Build();
			_gl.UseProgram(Id);
			_bound = true;
		}
		
		public int GetUniformLocation(string name) {
			int location = _gl.GetUniformLocation(Id, UniformPrefix + name + UniformSuffix);

			if(location < 0) {
				if(Debugging.Enabled && Debugging.IgnoreMissingShaderUniforms) {
					Log.Warning($"[SHADER PROGRAM] " +
					            $"Could not find the uniform location for [{name}]");
				} else {
					throw new PlatformException($"Could not find the uniform location for [{name}]");
				}
			}

			return location;
		}
		
		public unsafe bool SetUniform(string name, Matrix4x4 value) {
			int location = GetUniformLocation(name);
			if(location < 0) return false;
			
			_gl.UniformMatrix4(location, 1, false, (float*) &value);
			return true;
		}
		
		public unsafe bool SetUniform(string name, Vector3 value) {
			int location = GetUniformLocation(name);
			if(location < 0) return false;
			
			_gl.Uniform3(location, 1, (float*) &value);
			return true;
		}
		
		public unsafe bool SetUniform(string name, Vector4 value) {
			int location = GetUniformLocation(name);
			if(location < 0) return false;
			
			_gl.Uniform4(location, 1, (float*) &value);
			return true;
		}
		
		public unsafe bool SetUniform(string name, bool value) {
			int location = GetUniformLocation(name);
			if(location < 0) return false;
			
			_gl.Uniform1(location, 1, (int*) &value);
			return true;
		}
		
		public unsafe bool SetUniform(string name, int value) {
			int location = GetUniformLocation(name);
			if(location < 0) return false;
			
			_gl.Uniform1(location, 1, &value);
			return true;
		}
		
		public unsafe bool SetUniform(string name, float value) {
			int location = GetUniformLocation(name);
			if(location < 0) return false;
			
			_gl.Uniform1(location, 1, &value);
			return true;
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
			
			if(!_ready) return;
			_gl.DeleteProgram(Id);
		}

		public class LinkingException : Exception {
			
			public LinkingException(GL gl, uint id)
				: base($"Error occured during program linking: {gl.GetProgramInfoLog(id)}") { }
		}
		
		public class ValidationException : Exception {
			
			public ValidationException(GL gl, uint id)
				: base($"Shader validation failed: {gl.GetProgramInfoLog(id)}") { }
		}
	}
}