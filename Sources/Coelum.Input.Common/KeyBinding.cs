using Coelum.Debug;
using Silk.NET.Input;

namespace Coelum.Input.Common {
	
	public class KeyBinding {
		
		public string Name { get; }
		public Key[] Keys { get; set; }

		public bool Pressed { get; internal set; }
		/*public bool Released { get; internal set; }*/

		private bool _down;
		public bool Down {
			get {
				Tests.Assert(Keys.Length <= 1, "KeyBinding.Down is supported only for single-key inputs");
				return _down;
			}
			internal set => _down = value;
		}

		public KeyBinding(string name, params Key[] keys) {
			Name = name;
			Keys = keys;
		}

		public override string ToString() {
			string s = "";

			foreach(var key in Keys) {
				s += $"{key} + ";
			}

			return s[..^3];
		}
	}
}