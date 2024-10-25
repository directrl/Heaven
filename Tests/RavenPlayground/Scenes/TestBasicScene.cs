using Coelum.Common.Input;
using Coelum.Raven.Node;
using Coelum.Raven.Scene;
using Coelum.Raven.Terminal;

namespace RavenPlayground.Scenes {
	
	public class TestBasicScene : RavenSceneBase {

		private CharNode _player;

		private KeyBindings _keyBindings;
		
		private KeyBinding _up;
		private KeyBinding _down;
		private KeyBinding _left;
		private KeyBinding _right;

		public TestBasicScene(TerminalBase terminal) : base(terminal, "test-basic") {
			_keyBindings = new(Id);
			
			_player = new('@');
			AddChild(_player);
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			if(Console.KeyAvailable) {
				var key = Console.ReadKey(true);

				switch(key.Key) {
					case ConsoleKey.W:
						_player.Position.Y--;
						break;
					case ConsoleKey.S:
						_player.Position.Y++;
						break;
					case ConsoleKey.A:
						_player.Position.X--;
						break;
					case ConsoleKey.D:
						_player.Position.X++;
						break;
					default:
						break;
				}
			}
		}
	}
}