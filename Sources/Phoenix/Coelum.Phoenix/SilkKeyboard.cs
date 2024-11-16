using Silk.NET.Input;

namespace Coelum.Phoenix {
	
	public class SilkKeyboard : Common.Input.IKeyboard {
		
		public IKeyboard SilkImpl { get; }

		public SilkKeyboard(IKeyboard impl) {
			SilkImpl = impl;
		}

		public bool IsKeyPressed(int key) {
			return SilkImpl.IsKeyPressed((Key) key);
		}
	}
}