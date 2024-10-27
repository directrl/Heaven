using Silk.NET.Input;
using IKeyboard = Coelum.Common.Input.IKeyboard;

namespace Coelum.Phoenix.Input {
	
	public class SilkKeyboard : IKeyboard {
		
		public Silk.NET.Input.IKeyboard SilkImpl { get; }

		public SilkKeyboard(Silk.NET.Input.IKeyboard impl) {
			SilkImpl = impl;
		}

		public bool IsKeyPressed(int key) {
			return SilkImpl.IsKeyPressed((Key) key);
		}
	}
}