using System.Runtime.Versioning;
using Coeli.Graphics.OpenGL;
using Coeli.Resources;

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

	public interface IOverlayShaderLoadable : IShaderLoadable {

		public static abstract void SetupOverlays(ShaderProgram shader);
	}
	
	public interface IOverlayShaderRenderable : IOverlayShaderLoadable, IRenderable { }
}