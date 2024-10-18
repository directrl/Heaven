using System.Runtime.CompilerServices;
using Serilog;
using Silk.NET.Core;
using Silk.NET.OpenGL;

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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CheckGLError(GL gl, bool aggressive = false) {
			var error = gl.GetError();

			switch(error) {
				case GLEnum.NoError:
					break;
				default:
					if(aggressive) {
						throw new PlatformException($"OpenGL Error: {error}");
					}
					
					Log.Error($"OpenGL Error: {error}");
					break;
			}
		}
	}
}