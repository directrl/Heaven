using Coelum.Common.Graphics;
using Coelum.Raven.Node.Component;
using Coelum.Raven.Terminal;

namespace Coelum.Raven.Scene {
	
	public class RavenSceneBase : SceneBase {
		
		public TerminalBase Terminal { get; }

		public RavenSceneBase(TerminalBase terminal, string id) : base(id) {
			Terminal = terminal;
		}

		public override void OnRender(float delta) {
			Terminal.Clear();
			base.OnRender(delta);
			
			FindChildrenByComponent((ITerminalRenderable renderable) => {
				renderable.Render(Terminal);
			});
			
			Terminal.Refresh();
		}
	}
}