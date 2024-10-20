using System.Numerics;
using Coelum.Debug;
using Coelum.Graphics.OpenGL;
using Coelum.LanguageExtensions;
using Coelum.Node;
using Coelum.Resources;
using Silk.NET.OpenGL;

namespace Coelum.Graphics.Node {
	
	public class InstancedNode<TNode> : SpatialNode where TNode : SpatialNode {

		private bool _ready = false;

		private readonly List<TNode> _nodes;
		private readonly List<Matrix4x4> _models;
		private readonly List<Material> _materials;

		public override Model? Model { get; init; }
		public override Matrix4x4 ModelMatrix => throw new NotSupportedException();

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
				
				_models[i] = obj.ModelMatrix;
				_materials[i] = obj.Model?.Material ?? new Material();
			}
		}
		
		public void Add(TNode obj) {
			_nodes.Add(obj);
			_models.Add(obj.ModelMatrix);
			_materials.Add(obj.Model?.Material ?? new Material());
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
				var vbo = new VertexBufferObject(GL, BufferTargetARB.ArrayBuffer);
				vbo.Bind();

				int size = _models.Count * sizeof(Matrix4x4);
			
				fixed(void* addr = &_models.ToArrayNoCopy()[0]) {
					GL.BufferData(vbo.Target, (nuint) size, addr, GLEnum.StaticDraw);
				}
			
				foreach(var mesh in Model.Meshes) {
					Tests.Assert(mesh._gl == GL);
				
					mesh._vbos.Add(vbo);
					mesh.VAO.Bind();

					var type = VertexAttribPointerType.Float;
					size = sizeof(Matrix4x4);
				
					GL.EnableVertexAttribArray(i);
					GL.VertexAttribPointer(i, 4, type, false, (uint) size, null);
					GL.EnableVertexAttribArray(i + 1);
					GL.VertexAttribPointer(i + 1, 4, type, false, (uint) size, sizeof(Vector4));
					GL.EnableVertexAttribArray(i + 2);
					GL.VertexAttribPointer(i + 2, 4, type, false, (uint) size, 2 * sizeof(Vector4));
					GL.EnableVertexAttribArray(i + 3);
					GL.VertexAttribPointer(i + 3, 4, type, false, (uint) size, 3 * sizeof(Vector4));
				
					GL.VertexAttribDivisor(i, 1);
					GL.VertexAttribDivisor(++i, 1);
					GL.VertexAttribDivisor(++i, 1);
					GL.VertexAttribDivisor(++i, 1);
				
					GL.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
					GL.BindVertexArray(0);
				}
			}
		#endregion

			i++;

		#region Material
			{
				var vbo = new VertexBufferObject(GL, BufferTargetARB.ArrayBuffer);
				vbo.Bind();

				int size = _materials.Count * sizeof(Material);

				fixed(void* addr = &_materials.ToArrayNoCopy()[0]) {
					GL.BufferData(vbo.Target, (nuint) size, addr, GLEnum.StaticDraw);
				}

				foreach(var mesh in Model.Meshes) {
					Tests.Assert(mesh._gl == GL);
				
					mesh._vbos.Add(vbo);
					mesh.VAO.Bind();

				#region Color
					GL.EnableVertexAttribArray(i);
					GL.VertexAttribPointer(i, 4, VertexAttribPointerType.Float, false,
						(uint) sizeof(Material), null);
					GL.VertexAttribDivisor(i, 1);
				#endregion

					i++;

				#region Texture Layer
					GL.EnableVertexAttribArray(i);
					GL.VertexAttribIPointer(i, 1, VertexAttribIType.Int,
						(uint) sizeof(Material), 24);
					GL.VertexAttribDivisor(i, 1);
				#endregion
				
					GL.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
					GL.BindVertexArray(0);
				}
			}
		#endregion
			
			_ready = true;
		}

		public override void Load(ShaderProgram shader) {
			Tests.Assert(_ready, "Object is not ready! Did you forget to call Build() beforehand?");
			
			Model.Load(shader);
			shader.SetUniform("instance", true);
		}

		public unsafe override void Render() {
			Tests.Assert(_ready, "Object is not ready! Did you forget to call Build() beforehand?");
			
			uint oc = (uint) NodeCount;
			
			foreach(var mesh in Model.Meshes) {
				Tests.Assert(mesh._gl == GL);
				
				mesh.VAO.Bind();
				GL.DrawElementsInstanced(mesh.Type, mesh.VertexCount, DrawElementsType.UnsignedInt, null, oc);
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