using System.Drawing;
using Coelum.Raven.Node;
using Coelum.Raven.Window;

namespace Coelum.Raven.Scene {
	
	public class ExceptionScene : RavenSceneBase {

		private Exception _exception;

		public ExceptionScene(Exception e) : base(null) {
			_exception = e;

			ClearCell = new() {
				Character = ' ',
				BackgroundColor = Color.DarkRed
			};
		}

		public override void OnLoad(RenderWindow window) {
			base.OnLoad(window);
			
			Context.Clear(ref Context.BackBuffer, new() {
				Character = '!',
				BackgroundColor = Color.IndianRed,
				ForegroundColor = Color.DarkRed
			});
			Context.Render();
			
			var title = new LabelNode($"UNHANDLED EXCEPTION: {_exception.Source ?? "No source available"}", true) {
				Anchor = new(0.5f, 0),
				Position = new(Context.Display.Width / 2,  1),
			};

			var message = new LabelNode(_exception.Message, true) {
				Anchor = new(0.5f, 0),
				Position = new(title.Position.X, title.Position.Y + 1),
			};

			var stackTrace = new LabelNode("Stack trace:\n" + (_exception.StackTrace ?? "No stack trace available"), true) {
				Anchor = new(0, 0),
				Position = new(0, message.Position.Y + 2),
			};
			
			AddChild(title);
			AddChild(message);
			AddChild(stackTrace);
		}

		public override void OnRender(float delta) {
			base.OnRender(delta);
		}
	}
}