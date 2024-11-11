using Coelum.Debug;
using Newtonsoft.Json;
using Serilog;
using Silk.NET.Input;

namespace Coelum.Common.Input {
	
	public class KeyBinding {
		
		public string Name { get; }
		public int[] Keys { get; set; }

		public bool Pressed { get; internal set; }
		/*public bool Released { get; internal set; }*/

		private bool _down;
		public bool Down {
			get {
				Log.Warning($"{Name}.Down is supported only for single-key inputs");
				return _down;
			}
			internal set => _down = value;
		}

		[JsonConstructor]
		public KeyBinding(string name, params int[] keys) {
			Name = name;
			Keys = keys;
		}

		public KeyBinding(string name, params Key[] keys) {
			Name = name;
			Keys = new int[keys.Length];

			for(int i = 0; i < keys.Length; i++) {
				Keys[i] = (int) keys[i];
			}
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