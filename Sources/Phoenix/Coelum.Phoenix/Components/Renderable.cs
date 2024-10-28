using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.Components {

	public record Renderable(Action<float, ShaderProgram> Render);
}