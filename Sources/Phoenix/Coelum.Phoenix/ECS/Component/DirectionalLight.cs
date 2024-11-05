using System.Drawing;
using System.Numerics;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.OpenGL;
using Coelum.Phoenix.OpenGL.UBO;

namespace Coelum.Phoenix.ECS.Component {
	
	public class DirectionalLight : Light {

		public Node? Owner { get; set; }

		public Color Diffuse { get; set; } = Color.FromArgb(64, 64, 64);
		public Color Specular { get; set; } = Color.White;

		public Vector3 Direction {
			get {
				if(Owner == null) return Vector3.Zero;
		
				if(Owner.TryGetComponent<Transform, Transform3D>(out var t3d)) {
					return Vector3.Normalize(
						new(
							MathF.Cos(t3d.GlobalPitch) * MathF.Sin(-t3d.GlobalYaw),
							MathF.Sin(t3d.GlobalPitch),
							MathF.Cos(t3d.GlobalPitch) * MathF.Cos(-t3d.GlobalYaw)
						)
					);
				} else {
					return Vector3.Zero;
				}
			}
		}
		
		public void Load(Lights ubo, int index) {
			ubo.DirectionalLights[index] = new() {
				Diffuse = Diffuse.ToVector4(),
				Specular = Specular.ToVector4(),
				Direction = Direction
			};
		}
	}
}