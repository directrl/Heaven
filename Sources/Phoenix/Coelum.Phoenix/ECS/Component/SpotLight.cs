using System.Numerics;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.Component {
	
	public class SpotLight : PointLight {

		/// <summary>
		/// The cutoff angle in radians
		/// </summary>
		public float Cutoff { get; set; } = 45.0f.ToRadians();
		
		public Vector3 Direction {
			get {
				if(Owner == null) return Vector3.Zero;
		
				if(Owner.TryGetComponent<Transform, Transform3D>(out var t3d)) {
					// TODO i have no idea if this is correct
					return new(
						Math.Clamp(t3d.GlobalRotation.X / MathF.PI, -1.0f, 1.0f),
						Math.Clamp(t3d.GlobalRotation.Y / MathF.PI, -1.0f, 1.0f),
						Math.Clamp(t3d.GlobalRotation.Z / MathF.PI, -1.0f, 1.0f)
					);
					//return t3d.GlobalRotation;
				} else {
					return Vector3.Zero;
				}
			}
		}

		public override void Load(ShaderProgram shader) {
			base.Load(shader);

			shader.SetUniform("light.type", Light.LIGHT_SPOT);
			shader.SetUniform("light.cutoff", Cutoff);
			shader.SetUniform("light.direction", Direction);
		}
	}
}