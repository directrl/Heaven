using System.Numerics;
using Coeli.Debug;
using Coeli.Resources;
using Serilog;
using Silk.NET.Core;
using Silk.NET.OpenGL;

namespace Coeli.Graphics.OpenGL {
	
	public class ShaderProgram : IDisposable {

		private readonly GL _gl;
		private readonly Shader[] _program;
		private readonly List<Shader.Overlay> _overlays = new();
		
		private readonly ResourceManager? _preprocessorResources;
		
		private bool _ready;
		private bool _bound;
		
		public uint Id { get; }

		public ShaderProgram(ResourceManager? preprocessorResources, params Shader[] program) {
			_preprocessorResources = preprocessorResources;
			
			_gl = GLManager.Current;
			Id = _gl.CreateProgram();

			if(Id == 0) {
				throw new PlatformException("Could not create a GL shader program");
			}

			_program = program;
		}

		public void AddOverlay(Shader.Overlay overlay, ResourceManager resourceManager) {
			Tests.Assert(!_ready);
			_overlays.Add(overlay);

			foreach(var shader in _program) {
				if(shader.Type == overlay.Type) {
					// TODO do preprocessing stuff
				}
			}
		}

		public void EnableOverlay(Shader.Overlay overlay) {
			Tests.Assert(_bound);
			SetUniform($"overlay_{overlay.Name}", true);
		}
		
		public void DisableOverlay(Shader.Overlay overlay) {
			Tests.Assert(_bound);
			SetUniform($"overlay_{overlay.Name}", false);
		}

		public void Build() {
			List<uint> shaderIds = new();

			foreach(var shader in _program) {
				if(_preprocessorResources != null) shader.Preprocess(_preprocessorResources);
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
			int location = _gl.GetUniformLocation(Id, name);

			if(location < 0) {
				if(Debugging.Enabled) {
					Log.Warning($"Could not find the uniform location for [{name}]");
				} else {
					throw new PlatformException($"Could not find the uniform location for [{name}]");
				}
			}

			return location;
		}
		
		public unsafe void SetUniform(string name, Matrix4x4 value) {
			int location = GetUniformLocation(name);
			if(location < 0) return;
			
			_gl.UniformMatrix4(location, 1, false, (float*) &value);
		}
		
		public unsafe void SetUniform(string name, Vector3 value) {
			int location = GetUniformLocation(name);
			if(location < 0) return;
			
			_gl.Uniform3(location, 1, (float*) &value);
		}
		
		public unsafe void SetUniform(string name, Vector4 value) {
			int location = GetUniformLocation(name);
			if(location < 0) return;
			
			_gl.Uniform4(location, 1, (float*) &value);
		}
		
		public unsafe void SetUniform(string name, bool value) {
			int location = GetUniformLocation(name);
			if(location < 0) return;
			
			_gl.Uniform1(location, 1, (int*) &value);
		}
		
		public unsafe void SetUniform(string name, int value) {
			int location = GetUniformLocation(name);
			if(location < 0) return;
			
			_gl.Uniform1(location, 1, &value);
		}
		
		public unsafe void SetUniform(string name, float value) {
			int location = GetUniformLocation(name);
			if(location < 0) return;
			
			_gl.Uniform1(location, 1, &value);
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