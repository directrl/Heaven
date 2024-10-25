using Coelum.Graphics.OpenGL;
using Coelum.Node;

namespace Coelum.Graphics.Node.Component {

	public interface IShaderLoadable : INodeComponent {

		[Obsolete]
		public void Load(ShaderProgram shader);
	}
}