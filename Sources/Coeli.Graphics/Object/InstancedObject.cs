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

		public InstancedObject(TObject[] objects, Matrix4x4[] matrices) {
			_objects = objects;
			_matrices = matrices;
		}

		public unsafe void Build() {
			Tests.Assert(ObjectCount > 1);
			Tests.Assert(!_ready);
			
			var gl = GLManager.Current;
			
			var vbo = new VertexBufferObject(gl, BufferTargetARB.ArrayBuffer);
			vbo.Bind();
			
			fixed(void* m = &_matrices[0]) {
				gl.BufferData(vbo.Target, (nuint) (_matrices.Length * sizeof(Matrix4x4)), m, GLEnum.StaticDraw);
			}

			_ready = true;
			
			/*
			 * TODO normally, we should be able to modify the model matrices after we build,
			 * as this (i think) only takes the memory address (though it probably [definitely] uploads it
			 * to the gpu so cant do anything anyway) but perhaps if we make a static Matrix4x4 array
			 * that can be referenced natively with a fixed address it could work? (probably not)
			 */
			//foreach(var o in _objects) {
				foreach(var mesh in Meshes) {
					Tests.Assert(mesh._gl == gl);
					
					mesh.VAO.Bind();

					var type = VertexAttribPointerType.Float;
					uint size = (uint) sizeof(Matrix4x4);
					
					gl.EnableVertexAttribArray(3);
					gl.VertexAttribPointer(3, 4, type, false, size, null);
					gl.EnableVertexAttribArray(4);
					gl.VertexAttribPointer(4, 4, type, false, size, sizeof(Vector4));
					gl.EnableVertexAttribArray(5);
					gl.VertexAttribPointer(5, 4, type, false, size, (2 * sizeof(Vector4)));
					gl.EnableVertexAttribArray(6);
					gl.VertexAttribPointer(6, 4, type, false, size, (3 * sizeof(Vector4)));
					
					gl.VertexAttribDivisor(3, 1);
					gl.VertexAttribDivisor(4, 1);
					gl.VertexAttribDivisor(5, 1);
					gl.VertexAttribDivisor(6, 1);
					
					gl.BindVertexArray(0);
				}
			//}
		}

		public unsafe void Render(ShaderProgram shader) {
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