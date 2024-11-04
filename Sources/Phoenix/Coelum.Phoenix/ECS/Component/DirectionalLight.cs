using System.Drawing;
using System.Numerics;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.Component {
	
	public class DirectionalLight : Light {

		public Node? Owner { get; set; }

		public Color Ambient { get; set; } = Color.DarkSlateGray;
		public Color Diffuse { get; set; } = Color.White;
		public Color Specular { get; set; } = Color.White;

		public float SpecularStrength { get; set; } = 1; // TODO is this really needed?

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
				} else {
					return Vector3.Zero;
				}
			}
		}
		
		public void Load(ShaderProgram shader) {
			shader.SetUniform("light.type", Light.LIGHT_DIRECTIONAL);
			
			shader.SetUniform("light.ambient", Ambient.ToVector3());
			shader.SetUniform("light.diffuse", Diffuse.ToVector3());
			shader.SetUniform("light.specular", Specular.ToVector3());
			shader.SetUniform("light.specular_strength", SpecularStrength);
			
			shader.SetUniform("light.direction", Direction);
		}
	}
}