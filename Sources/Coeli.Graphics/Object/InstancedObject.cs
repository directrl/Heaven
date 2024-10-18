using System.Numerics;
using System.Reflection.Metadata;
using Coeli.Debug;
using Coeli.Graphics.OpenGL;
using Coeli.Graphics.Texture;
using Coeli.LanguageExtensions;
using Coeli.Resources;
using Silk.NET.OpenGL;

namespace Coeli.Graphics.Object {
	
	public class InstancedObject<TObject> : Model where TObject : ObjectBase {

		private bool _ready = false;

		private List<TObject> _objects;
		
		private List<Matrix4x4> _models;
		private List<Material> _materials;

		public int ObjectCount => _objects.Count;

		public InstancedObject(int capacity = 0) {
			_objects = new(capacity);
			_models = new(capacity);
			_materials = new(capacity);
		}
		
		public InstancedObject(List<TObject> objects) {
			_objects = objects;
			_models = new(objects.Count);
			_materials = new(objects.Count);

			for(int i = 0; i < objects.Count; i++) {
				var obj = objects[i];
				
				_models[i] = obj.ModelMatrix;
				_materials[i] = obj.Material;
			}
		}
		
		public void Add(TObject obj) {
			_objects.Add(obj);
			_models.Add(obj.ModelMatrix);
			_materials.Add(obj.Material);
		}

		public void Clear() {
			_objects.Clear();
			_models.Clear();
			_materials.Clear();
		}

		public unsafe void Build() {
			Tests.Assert(ObjectCount > 1, "ObjectCount > 1");
			Tests.Assert(!_ready, "!_ready");
			
			var gl = GLManager.Current;
			uint i = 10;

		#region Model
			{
				var vbo = new VertexBufferObject(gl, BufferTargetARB.ArrayBuffer);
				vbo.Bind();

				int size = _models.Count * sizeof(Matrix4x4);
			
				fixed(void* addr = &_models.ToArrayNoCopy()[0]) {
					gl.BufferData(vbo.Target, (nuint) size, addr, GLEnum.StaticDraw);
				}
			
				foreach(var mesh in Meshes) {
					Tests.Assert(mesh._gl == gl);
				
					mesh._vbos.Add(vbo);
					mesh.VAO.Bind();

					var type = VertexAttribPointerType.Float;
					size = sizeof(Matrix4x4);
				
					gl.EnableVertexAttribArray(i);
					gl.VertexAttribPointer(i, 4, type, false, (uint) size, null);
					gl.EnableVertexAttribArray(i + 1);
					gl.VertexAttribPointer(i + 1, 4, type, false, (uint) size, sizeof(Vector4));
					gl.EnableVertexAttribArray(i + 2);
					gl.VertexAttribPointer(i + 2, 4, type, false, (uint) size, 2 * sizeof(Vector4));
					gl.EnableVertexAttribArray(i + 3);
					gl.VertexAttribPointer(i + 3, 4, type, false, (uint) size, 3 * sizeof(Vector4));
				
					gl.VertexAttribDivisor(i, 1);
					gl.VertexAttribDivisor(++i, 1);
					gl.VertexAttribDivisor(++i, 1);
					gl.VertexAttribDivisor(++i, 1);
				
					gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
					gl.BindVertexArray(0);
				}
			}
		#endregion

			i++;

		#region Material
			{
				var vbo = new VertexBufferObject(gl, BufferTargetARB.ArrayBuffer);
				vbo.Bind();

				int size = _materials.Count * sizeof(Material);

				fixed(void* addr = &_materials.ToArrayNoCopy()[0]) {
					gl.BufferData(vbo.Target, (nuint) size, addr, GLEnum.StaticDraw);
				}

				foreach(var mesh in Meshes) {
					Tests.Assert(mesh._gl == gl);
				
					mesh._vbos.Add(vbo);
					mesh.VAO.Bind();

				#region Color
					gl.EnableVertexAttribArray(i);
					gl.VertexAttribPointer(i, 4, VertexAttribPointerType.Float, false,
						(uint) sizeof(Material), null);
					gl.VertexAttribDivisor(i, 1);
				#endregion

					i++;

				#region Texture Layer
					gl.EnableVertexAttribArray(i);
					gl.VertexAttribIPointer(i, 1, VertexAttribIType.Int,
						(uint) sizeof(Material), 24);
					gl.VertexAttribDivisor(i, 1);
				#endregion
				
					gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
					gl.BindVertexArray(0);
				}
			}
		#endregion
			
			_ready = true;
		}

		public override void Load(ShaderProgram shader) {
			Tests.Assert(_ready, "Object is not ready! Did you forget to call Build() beforehand?");
			
			base.Load(shader);
			shader.SetUniform("instance", true);
		}

		public unsafe override void Render() {
			Tests.Assert(_ready, "Object is not ready! Did you forget to call Build() beforehand?");

			var gl = GLManager.Current;
			uint oc = (uint) ObjectCount;
			
			foreach(var mesh in Meshes) {
				Tests.Assert(mesh._gl == gl);
				
				mesh.VAO.Bind();
				gl.DrawElementsInstanced(mesh.Type, mesh.VertexCount, DrawElementsType.UnsignedInt, null, oc);
			}
		}
		
		public static readonly IShaderOverlay[] OVERLAYS = [
			VertexShaderOverlay.OVERLAY,
			FragmentShaderOverlay.OVERLAY
		];
		
		public class VertexShaderOverlay : IShaderOverlay, ILazySingleton<VertexShaderOverlay> {

			public static VertexShaderOverlay OVERLAY
				=> ILazySingleton<VertexShaderOverlay>._instance.Value;

			public string Name => "instancing";
			public string Path => "Overlays.Instancing";
			public ShaderType Type => ShaderType.FragmentShader;
			public ShaderPass Pass => ShaderPass.COLOR_POST;
			public ResourceManager ResourceManager => Module.RESOURCES;

			public void Load(ShaderProgram shader) { }
		}
		
		public class FragmentShaderOverlay : IShaderOverlay, ILazySingleton<FragmentShaderOverlay> {

			public static FragmentShaderOverlay OVERLAY
				=> ILazySingleton<FragmentShaderOverlay>._instance.Value;

			public string Name => "instancing";
			public string Path => "Overlays.Instancing";
			public ShaderType Type => ShaderType.VertexShader;
			public ShaderPass Pass => ShaderPass.POSITION_POST;
			public ResourceManager ResourceManager => Module.RESOURCES;

			public void Load(ShaderProgram shader) { }
		}
	}
}