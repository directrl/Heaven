using Coelum.Graphics.OpenGL;

namespace Coelum.Graphics.Node.Component {
	
	public interface IShaderRenderable : IRenderable, IShaderLoadable {

		public void Render(ShaderProgram shader) {
			Load(shader);
			Render();
		}
	}
}