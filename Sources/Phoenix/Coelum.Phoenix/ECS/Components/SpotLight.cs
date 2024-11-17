using System.Numerics;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.OpenGL.UBO;

namespace Coelum.Phoenix.ECS.Components {
	
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

		public override void Load(Lights ubo, int index) {
			var light = CalculateLightValues();
			
			ubo.SpotLights[index] = new() {
				Diffuse = Diffuse.ToVector4(),
				Specular = Specular.ToVector4(),
				Position = Position,
				Constant = light.constant,
				Linear = light.linear,
				Quadratic = light.quadratic,
				Cutoff = Cutoff,
				OuterCutoff = Fade,
				Direction = Direction
			};
		}
	}
}