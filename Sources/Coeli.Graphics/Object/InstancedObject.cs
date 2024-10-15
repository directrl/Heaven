using System.Numerics;
using System.Reflection.Metadata;
using Coeli.Debug;
using Coeli.Graphics.OpenGL;
using Silk.NET.OpenGL;

namespace Coeli.Graphics.Object {
	
	public class InstancedObject<TObject> : Model where TObject : ObjectBase {

		private bool _ready = false;
		/*private List<TObject> _objects;
		private List<Matrix4x4> _matrices;*/
		private TObject[] _objects;
		
		private Matrix4x4[] _matrices;
		private Vector4[] _colors;

		public int ObjectCount => _objects.Length;

		/*public InstancedObject() {
			_objects = new();
			_matrices = new();
		}

		public InstancedObject(int capacity) {
			_objects = new(capacity);
			_matrices = new(capacity);
		}

		public InstancedObject(TObject[] objects) {
			_objects = new();
			_objects.AddRange(objects);

			_matrices = new(_objects.Count);
		}
		
		public void Add(TObject o) {
			_objects.Add(o);
			_matrices.Add(o.ModelMatrix);
		}

		public void Clear() {
			_objects.Clear();
			_matrices.Clear();
		}*/

		public InstancedObject(TObject[] objects, Vector4[] colors) {
			_objects = objects;
			_matrices = new Matrix4x4[objects.Length];
			_colors = colors;

			for(int i = 0; i < objects.Length; i++) {
				var obj = objects[i];
				_matrices[i] = obj.ModelMatrix;
			}
		}

		public unsafe void Build() {
			Tests.Assert(ObjectCount > 1);
			Tests.Assert(!_ready);
			
			var gl = GLManager.Current;

		#region Model
			{
				var vbo = new VertexBufferObject(gl, BufferTargetARB.ArrayBuffer);
				vbo.Bind();

				int size = _matrices.Length * sizeof(Matrix4x4);
			
				fixed(void* addr = &_matrices[0]) {
					gl.BufferData(vbo.Target, (nuint) size, addr, GLEnum.StaticDraw);
				}
			
				foreach(var mesh in Meshes) {
					Tests.Assert(mesh._gl == gl);
				
					mesh._vbos.Add(vbo);
					mesh.VAO.Bind();

					var type = VertexAttribPointerType.Float;
					size = sizeof(Matrix4x4);
				
					gl.EnableVertexAttribArray(3);
					gl.VertexAttribPointer(3, 4, type, false, (uint) size, null);
					gl.EnableVertexAttribArray(4);
					gl.VertexAttribPointer(4, 4, type, false, (uint) size, sizeof(Vector4));
					gl.EnableVertexAttribArray(5);
					gl.VertexAttribPointer(5, 4, type, false, (uint) size, 2 * sizeof(Vector4));
					gl.EnableVertexAttribArray(6);
					gl.VertexAttribPointer(6, 4, type, false, (uint) size, 3 * sizeof(Vector4));
				
					gl.VertexAttribDivisor(3, 1);
					gl.VertexAttribDivisor(4, 1);
					gl.VertexAttribDivisor(5, 1);
					gl.VertexAttribDivisor(6, 1);
				
					gl.BindVertexArray(0);
				}
			}
		#endregion

		#region Colors
			{
				var vbo = new VertexBufferObject(gl, BufferTargetARB.ArrayBuffer);
				vbo.Bind();

				int size = _colors.Length * sizeof(Vector4);

				fixed(void* addr = &_colors[0]) {
					gl.BufferData(vbo.Target, (nuint) size, addr, GLEnum.StaticDraw);
				}

				foreach(var mesh in Meshes) {
					Tests.Assert(mesh._gl == gl);
				
					mesh._vbos.Add(vbo);
					mesh.VAO.Bind();

					var type = VertexAttribPointerType.Float;
					size = sizeof(Vector4);
				
					gl.EnableVertexAttribArray(7);
					gl.VertexAttribPointer(7, 4, type, false, (uint) size, null);
				
					gl.VertexAttribDivisor(7, 1);
				
					gl.BindVertexArray(0);
				}
			}
		#endregion
			
			_ready = true;
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
	}
}