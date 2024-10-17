using Coeli.Graphics.OpenGL;

namespace Coeli.Graphics {
	
	public interface IRenderable {

		public void Render();
	}

	public interface IShaderLoadable {
		
		public void Load(ShaderProgram shader);
	}
	
	public interface IShaderRenderable : IShaderLoadable, IRenderable {

		public void Render(ShaderProgram shader) {
			Load(shader);
			Render();
		}
	}
}