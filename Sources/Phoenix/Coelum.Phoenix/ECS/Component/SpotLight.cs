using System.Numerics;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.Component {
	
	public class SpotLight : PointLight {

		/// <summary>
		/// The cutoff angle in radians
		/// </summary>
		public float Cutoff { get; set; } = 45.0f.ToRadians();

		/// <summary>
		/// The relative outer cutoff (fade) angle in radians 
		/// </summary>
		public float Fade { get; set; } = 2.0f.ToRadians();
		
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

		public override void Load(ShaderProgram shader) {
			base.Load(shader);

			shader.SetUniform("light.type", Light.LIGHT_SPOT);
			shader.SetUniform("light.cutoff", Cutoff);
			shader.SetUniform("light.outer_cutoff", Fade);
			shader.SetUniform("light.direction", Direction);
		}
	}
}