using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks.Dataflow;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix.OpenGL.UBO {
	
	public unsafe class Lights : UniformBufferObject {

		public const int MAX_DIRECTIONAL_LIGHTS = 4;
		public const int MAX_LIGHTS = 100;
		
		public int DirectionalLightCount = 0;
		public int PointLightCount = 0;
		public int SpotLightCount = 0;

		public readonly DirectionalLight[] DirectionalLights;
		public readonly PointLight[] PointLights;
		public readonly SpotLight[] SpotLights;

		public Lights() : base("Lights") {
			DirectionalLights = new DirectionalLight[MAX_DIRECTIONAL_LIGHTS];
			PointLights = new PointLight[MAX_LIGHTS];
			SpotLights = new SpotLight[MAX_LIGHTS];

			int size =
				DirectionalLights.Length * sizeof(DirectionalLight)
				+ PointLights.Length * sizeof(PointLight)
				+ SpotLights.Length * sizeof(SpotLight)
				+ 3 * sizeof(int)
				+ 6; // some random offset between ints and arrays
			
			Bind();
			Allocate(size, BufferUsageARB.StreamDraw);
			BindRange(Binding, 0, size);
			Unbind();
		}

		public override void Upload() {
			base.Upload();
			
			void ArrayUpload<T>(T[] array) {
				int size = array.Length * sizeof(T);
				
				fixed(T* ptr = &array[0]) {
					SubData(Offset, size, ptr);
				}

				Offset += size;
			}
			
			Bind();
			
			SubUpload(DirectionalLightCount);
			SubUpload(PointLightCount);
			SubUpload(SpotLightCount);

			Offset = 16;
			
			ArrayUpload(DirectionalLights);
			ArrayUpload(PointLights);
			ArrayUpload(SpotLights);
			
			Unbind();
		}
		
		[StructLayout(LayoutKind.Explicit)]
		public struct DirectionalLight {

			[FieldOffset(0)] public Vector4 Diffuse;
			[FieldOffset(16)] public Vector4 Specular;

			[FieldOffset(32)] public Vector3 Direction;
			
			[FieldOffset(44)] private float _padding;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct PointLight {
				
			[FieldOffset(0)] public Vector4 Diffuse;
			[FieldOffset(16)] public Vector4 Specular;
			
			[FieldOffset(32)] public Vector3 Position;
				
			[FieldOffset(48)] public float Constant;
			[FieldOffset(52)] public float Linear;
			[FieldOffset(56)] public float Quadratic;

			[FieldOffset(60)] private float _padding;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct SpotLight {
				
			[FieldOffset(0)] public Vector4 Diffuse;
			[FieldOffset(16)] public Vector4 Specular;
			
			[FieldOffset(32)] public Vector3 Direction;
			[FieldOffset(48)] public Vector3 Position;

			[FieldOffset(64-4)] public float Constant;
			[FieldOffset(68-4)] public float Linear;
			[FieldOffset(72-4)] public float Quadratic;

			[FieldOffset(76-4)] public float Cutoff;
			[FieldOffset(80-4)] public float OuterCutoff;
		}
	}
}