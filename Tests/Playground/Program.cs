using System.Drawing;
using Silk.NET.GLFW;
using Silk.NET.Windowing;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;

namespace Playground {
	
	public class Program {

		public unsafe static void Main(string[] args) {
			var heaven = new Playground();
			heaven.Start(args);
		}
	}
}