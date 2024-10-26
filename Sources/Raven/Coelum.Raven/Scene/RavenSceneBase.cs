using System.Drawing;
using Coelum.Common.Graphics;
using Coelum.Debug;
using Coelum.Raven.Node.Component;
using Coelum.Raven.Window;

namespace Coelum.Raven.Scene {
	
	public class RavenSceneBase : SceneBase {

		public Cell ClearCell { get; protected set; } = RenderContext.DEFAULT_CELL;

		public RenderContext Context { get; private set; }

		public RavenSceneBase(string id) : base(id) { }

		public virtual void OnLoad(RenderWindow window) {
			Context = window.Context;
			Context.Reset();
		}
		
		public override void OnLoad(WindowBase window) {
			base.OnLoad(window);
			Tests.Assert(window is RenderWindow);
			
			OnLoad((RenderWindow) window);
		}

		public override void OnRender(float delta) {
			base.OnRender(delta);
			
			FindChildrenByComponent((IRenderable renderable) => {
				renderable.Render(Context);
			});
		}
	}
}