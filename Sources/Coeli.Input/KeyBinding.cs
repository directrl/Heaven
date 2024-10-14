using System.Diagnostics;
using Silk.NET.Input;

namespace Coeli.Input {
	
	public class KeyBinding {
		
		public string Name { get; }
		public Key[] Keys { get; set; }

		public bool Pressed { get; internal set; }
		/*public bool Released { get; internal set; }*/

		private bool _down;
		public bool Down {
			get {
				Debug.Assert(Keys.Length <= 1, "KeyBinding.Down supported only for single-key inputs");
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