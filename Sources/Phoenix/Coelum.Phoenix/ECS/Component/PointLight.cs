using System.Drawing;
using System.Numerics;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.Component {
	
	public class PointLight : Light {

		private static readonly Dictionary<int, (float constant, float linear, float quadratic)> ATTENUATION = new() {
			{ 0, (1.0f, 1.0f, 1.0f) },
			{ 7, (1.0f, 0.7f, 1.8f) },
			{ 13, (1.0f, 0.35f, 0.44f) },
			{ 20, (1.0f, 0.22f, 0.20f) },
			{ 32, (1.0f, 0.14f, 0.07f) },
			{ 50, (1.0f, 0.09f, 0.032f) },
			{ 65, (1.0f, 0.07f, 0.017f) },
			{ 100, (1.0f, 0.045f, 0.0075f) },
			{ 160, (1.0f, 0.027f, 0.0028f) },
			{ 200, (1.0f, 0.022f, 0.0019f) },
			{ 325, (1.0f, 0.014f, 0.0007f) },
			{ 600, (1.0f, 0.007f, 0.0002f) },
			{ 3250, (1.0f, 0.0014f, 0.000007f) },
		};

		public Node? Owner { get; set; }

		public Color Diffuse { get; set; } = Color.White;
		public Color Specular { get; set; } = Color.White;

		public Vector3 Position {
			get {
				if(Owner == null) return Vector3.Zero;
		
				if(Owner.TryGetComponent<Transform, Transform3D>(out var t3d)) {
					return t3d.GlobalPosition;
				} else {
					return Vector3.Zero;
				}
			}
		}

		public int Distance { get; set; } = 64;
		
		public virtual void Load(ShaderProgram shader, string target) {
			shader.SetUniform(target + ".diffuse", Diffuse.ToVector4());
			shader.SetUniform(target + ".specular", Specular.ToVector4());
			
			shader.SetUniform(target + ".position", Position);

			// TODO could also precalculate this on startup
			if(!ATTENUATION.TryGetValue(Distance, out var light)) {
				var distances = ATTENUATION.Keys;

				int closestLower = distances.LastOrDefault(d => d <= Distance);
				int closestUpper = distances.FirstOrDefault(d => d >= Distance);

				if(closestLower == 0) {
					light = ATTENUATION.Values.First();
					goto apply;
				}

				if(closestUpper == 0) {
					light = ATTENUATION.Values.Last();
					goto apply;
				}

				if(closestLower == closestUpper) {
					light = ATTENUATION[closestLower];
					goto apply;
				}
				
				// interpolate
				var lower = ATTENUATION[closestLower];
				var upper = ATTENUATION[closestUpper];

				float ratio = (float) (Distance - closestLower) / (closestUpper - closestLower);
				light.constant = lower.constant;
				light.linear = lower.linear + ratio * (upper.linear - lower.linear);
				light.quadratic = lower.quadratic + ratio * (upper.quadratic - lower.quadratic);
			}
			
		apply:
			shader.SetUniform(target + ".constant", light.constant);
			shader.SetUniform(target + ".linear", light.linear);
			shader.SetUniform(target + ".quadratic", light.quadratic);
		}
	}
}