using System.Drawing;
using System.Numerics;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.OpenGL;

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
		
		public void Load(ShaderProgram shader, string target) {
			shader.SetUniform(target + ".diffuse", Diffuse.ToVector4());
			shader.SetUniform(target + ".specular", Specular.ToVector4());

			shader.SetUniform(target + ".direction", Direction);
		}
	}
}