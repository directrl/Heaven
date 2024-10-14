using System.Numerics;
using Coeli.Graphics;
using Coeli.Graphics.Object;
using Coeli.Graphics.OpenGL;

namespace Playground.Scenes.VoxelSpace {
	
	public static class Renderer {

		public static int ObjectCount { get; private set; } = 0;
		public static Dictionary<Object2D, Vector4> Objects { get; } = new();
		
		/*
		 * possible optimizations:
		 * instancing/batch rendering/indirect rendering
		 * pre-processing map for Vector4 normalized colors
		 * utilize 3D capabilities to move every position by something small like +0.01Z to get depth testing
		 *
		 * most of the performance seems to be lost at the rendering loop itself, rather than the actual rendering
		 */
		public static void Render(Window window,
		                          Vector2 p,
		                          float phi,
		                          float height,
		                          float horizon,
		                          float scaleHeight,
		                          int distance,
		                          ref Vector3[,] colorMap,
		                          ref Vector3[,] heightMap,
		                          ref int cMw, ref int cMh,
		                          ref int hMw, ref int hMh,
		                          ShaderProgram shader,
		                          ref Mesh pixel) {
			int screenWidth = window.SilkImpl.FramebufferSize.X;
			int screenHeight = window.SilkImpl.FramebufferSize.Y;

			ObjectCount = 0;
			Objects.Clear();

			var sinPhi = MathF.Sin(phi);
			var cosPhi = MathF.Cos(phi);

			for(int z = distance; z > 1; z--) {
				var left = new Vector2(
					(-cosPhi * z - sinPhi * z) + p.X,
					(sinPhi * z - cosPhi * z) + p.Y);
				var right = new Vector2(
					(cosPhi * z - sinPhi * z) + p.X,
					(-sinPhi * z - cosPhi * z) + p.Y);

				float dX = (right.X - left.X) / screenWidth;
				float dY = (right.Y - left.Y) / screenHeight;

				for(int i = 0; i < screenWidth; i++) {
					int iX = (int) left.X;
					int iY = (int) left.Y;
					
					if(iX < 0 || iY < 0 || iX >= hMw || iY >= hMh) continue;

					var pHeight = heightMap[iX, iY];
					var heightOnScreen = (height - pHeight.X) / z * scaleHeight + horizon;

				#region Drawing
					var c = colorMap[iX, iY];
					
					var o = new Object2D {
						Meshes = new[] { pixel },
						Position = new(
							i - screenWidth / 2,
							(heightOnScreen * 2) - (screenHeight / 2)
						),
						Scale = new(1, heightOnScreen - screenHeight)
					};
					
					/*shader.SetUniform("color",
						new Vector4(
							c.X / 255,
							c.Y / 255,
							c.Z / 255,
							1
						));
					o.Render(shader);*/

					Objects.Add(o, new Vector4(
						c.X / 255,
						c.Y / 255,
						c.Z / 255,
						1
					));
					ObjectCount++;
				#endregion

					left.X += dX;
					left.Y += dY;
				}
			}
		}
	}
}