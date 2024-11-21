using System.Numerics;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix.OpenGL.UBO {
	
	// TODO
	public unsafe class MaterialUBO : UniformBufferObject {

		public InnerMaterial Material;

		public MaterialUBO() : base("MaterialUBO") {
			Bind();

			int size = (4 * sizeof(Vector4))
				+ (2 * sizeof(float))
				+ 4;

			//Allocate(size, BufferUsageARB.DynamicDraw);
			BindRange(Binding, 0, size);

			fixed(void* ptr = &Material) {
				Gl.BufferData(Target, (uint) size, ptr, BufferUsageARB.DynamicDraw);
			}
			
			Unbind();
		}

		public override void Upload() {
			base.Upload();
			// Bind();
			//
			// SubUpload(Material);
			//
			// Unbind();
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct InnerMaterial {
			
			[FieldOffset(0)] public Vector4 Albedo;
			[FieldOffset(16)] public Vector4 AmbientColor;
			[FieldOffset(32)] public Vector4 DiffuseColor;
			[FieldOffset(48)] public Vector4 SpecularColor;

			[FieldOffset(64)] public float Shininess;
			[FieldOffset(68)] public float Reflectivity;

			[FieldOffset(72)] public bool HasTextures;
		}
	}
}