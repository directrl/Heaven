using Coelum.Graphics.OpenGL;

namespace Coelum.Graphics {
	
	public interface _IRenderable {

		public void Render();
	}

	public interface _IShaderLoadable {
		
		public void Load(ShaderProgram shader);
	}
	
	public interface _IShaderRenderable : _IShaderLoadable, _IRenderable {

		public void Render(ShaderProgram shader) {
			Load(shader);
			Render();
		}
	}
}