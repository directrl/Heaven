using System.Runtime.CompilerServices;

namespace Coelum.Debug {
	
	public static class Tests {
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assert(bool condition) {
		#if DEBUG
			System.Diagnostics.Debug.Assert(condition);
		#endif
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assert(bool condition, string message) {
		#if DEBUG
			System.Diagnostics.Debug.Assert(condition, message);
		#endif
		}
	}
}