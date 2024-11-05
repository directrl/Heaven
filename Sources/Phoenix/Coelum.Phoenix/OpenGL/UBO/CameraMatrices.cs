using System.Numerics;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix.OpenGL.UBO {
	
	public unsafe class CameraMatrices : UniformBufferObject {

		public Matrix4x4 Projection;
		public Matrix4x4 View;
		public Vector3 CameraPos;

		public CameraMatrices() : base("CameraMatrices") {
			Bind();

			int size = (2 * sizeof(Matrix4x4)) + sizeof(Vector4);
			
			Allocate(size, BufferUsageARB.StreamDraw);
			BindRange(Binding, 0, size);
			
			Unbind();
		}

		public override void Upload() {
			base.Upload();
			Bind();
			
			SubUpload(Projection);
			SubUpload(View);

			var cameraPosVec4 = new Vector4(CameraPos.X, CameraPos.Y, CameraPos.Z, 0);
			SubUpload(&cameraPosVec4);
			
			Unbind();
		}
	}
}