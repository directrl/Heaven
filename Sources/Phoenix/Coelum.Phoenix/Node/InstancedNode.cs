using System.Numerics;
using Coelum.Debug;
using Coelum.LanguageExtensions;
using Coelum.Node;
using Coelum.Phoenix.OpenGL;
using Coelum.Resources;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix.Node {
	
	// TODO fix this
	public class InstancedNode<TNode> : SpatialNode where TNode : SpatialNode {

		private bool _ready = false;

		private readonly List<TNode> _nodes;
		private readonly List<Matrix4x4> _models;
		private readonly List<Material> _materials;

		public override Model? Model { get; init; }
		
		public override Matrix4x4 GlobalTransform => throw new NotSupportedException();
		public override Matrix4x4 LocalTransform => throw new NotSupportedException();

		public int NodeCount => _nodes.Count;

		public InstancedNode(Model model, int capacity = 0) {
			Model = model;
			
			_nodes = new(capacity);
			_models = new(capacity);
			_materials = new(capacity);
		}
		
		public InstancedNode(Model model, List<TNode> nodes) {
			Model = model;
			
			_nodes = nodes;
			_models = new(nodes.Count);
			_materials = new(nodes.Count);

			for(int i = 0; i < nodes.Count; i++) {
				var obj = nodes[i];
				
				_models[i] = obj.GlobalTransform;
				//_materials[i] = obj.Model?.Material ?? new Material();
			}
		}
		
		public void Add(TNode obj) {
			_nodes.Add(obj);
			_models.Add(obj.GlobalTransform);
			//_materials.Add(obj.Model?.Material ?? new Material());
		}

		public void Clear() {
			_nodes.Clear();
			_models.Clear();
			_materials.Clear();
		}

		public unsafe void Build() {
			Tests.Assert(NodeCount > 1, "ObjectCount > 1");
			Tests.Assert(!_ready, "!_ready");

			uint i = 10;

		#region Model
			{
				var vbo = new VertexBufferObject(BufferTargetARB.ArrayBuffer);
				vbo.Bind();

				int size = _models.Count * sizeof(Matrix4x4);
			
				fixed(void* addr = &_models.ToArrayNoCopy()[0]) {
					Gl.BufferData(vbo.Target, (nuint) size, addr, GLEnum.StaticDraw);
				}
			
				foreach(var mesh in Model.Meshes) {
					mesh._vbos.Add(vbo);
					mesh.VAO.Bind();

					var type = VertexAttribPointerType.Float;
					size = sizeof(Matrix4x4);
				
					Gl.EnableVertexAttribArray(i);
					Gl.VertexAttribPointer(i, 4, type, false, (uint) size, null);
					Gl.EnableVertexAttribArray(i + 1);
					Gl.VertexAttribPointer(i + 1, 4, type, false, (uint) size, sizeof(Vector4));
					Gl.EnableVertexAttribArray(i + 2);
					Gl.VertexAttribPointer(i + 2, 4, type, false, (uint) size, 2 * sizeof(Vector4));
					Gl.EnableVertexAttribArray(i + 3);
					Gl.VertexAttribPointer(i + 3, 4, type, false, (uint) size, 3 * sizeof(Vector4));
				
					Gl.VertexAttribDivisor(i, 1);
					Gl.VertexAttribDivisor(++i, 1);
					Gl.VertexAttribDivisor(++i, 1);
					Gl.VertexAttribDivisor(++i, 1);
				
					Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
					Gl.BindVertexArray(0);
				}
			}
		#endregion

			i++;

		#region Material
			{
				var vbo = new VertexBufferObject(BufferTargetARB.ArrayBuffer);
				vbo.Bind();

				int size = _materials.Count * sizeof(Material);

				fixed(void* addr = &_materials.ToArrayNoCopy()[0]) {
					Gl.BufferData(vbo.Target, (nuint) size, addr, GLEnum.StaticDraw);
				}

				foreach(var mesh in Model.Meshes) {
					mesh._vbos.Add(vbo);
					mesh.VAO.Bind();

				#region Color
					Gl.EnableVertexAttribArray(i);
					Gl.VertexAttribPointer(i, 4, VertexAttribPointerType.Float, false,
						(uint) sizeof(Material), null);
					Gl.VertexAttribDivisor(i, 1);
				#endregion

					i++;

				#region Texture Layer
					Gl.EnableVertexAttribArray(i);
					Gl.VertexAttribIPointer(i, 1, VertexAttribIType.Int,
						(uint) sizeof(Material), 24);
					Gl.VertexAttribDivisor(i, 1);
				#endregion
				
					Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
					Gl.BindVertexArray(0);
				}
			}
		#endregion
			
			_ready = true;
		}

		public override void Render(ShaderProgram shader) {
			Tests.Assert(_ready, "Object is not ready! Did you forget to call Build() beforehand?");
			
			shader.SetUniform("instance", true);
			Model?.Render(shader);
		}

		public unsafe override void Render() {
			Tests.Assert(_ready, "Object is not ready! Did you forget to call Build() beforehand?");
			
			uint oc = (uint) NodeCount;
			
			foreach(var mesh in Model.Meshes) {
				mesh.VAO.Bind();
				Gl.DrawElementsInstanced(mesh.Type, mesh.VertexCount, DrawElementsType.UnsignedInt, null, oc);
			}
		}
		
		public static readonly IShaderOverlay[] OVERLAYS = {
			VertexShaderOverlay.OVERLAY,
			FragmentShaderOverlay.OVERLAY
		};
		
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