using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = SFML.Graphics.Color;
using Image = SixLabors.ImageSharp.Image;

namespace Playground {

	public class Application {

		private static int _cMw = 0;
		private static int _cMh = 0;
		
		private static int _hMw = 0;
		private static int _hMh = 0;

		private static Vector3[,] _colorMap;
		private static Vector3[,] _heightMap;

		private static Vector2 p = new(400, 450);
		private static float phi = 0;
		private static float height = 150;
		private static float horizon = 300;
		
		private static RectangleShape b = new RectangleShape(new Vector2f(1, 1));
		private static RectangleShape c = new RectangleShape(new Vector2f(1, 1));

		public static void Lain() {
			DoParse("C10W.png", out _colorMap, ref _cMw, ref _cMh);
			DoParse("D10.png", out _heightMap, ref _hMw, ref _hMh);
			
			var mode = new VideoMode(800, 800);
			var window = new RenderWindow(mode, "Playground");

			var view = window.DefaultView;
			//view.Size = new Vector2f(window.Size.X, -window.Size.Y);
			
			window.SetView(view);

			window.Closed += (o, e) => {
				window.Close();
			};

			float cm = 3.0f;
			window.KeyPressed += (o, e) => {
				if(e.Code == Keyboard.Key.W) p.Y -= cm;
				if(e.Code == Keyboard.Key.S) p.Y += cm;
				if(e.Code == Keyboard.Key.A) p.X -= cm;
				if(e.Code == Keyboard.Key.D) p.X += cm;
				if(e.Code == Keyboard.Key.Q) phi += cm / MathF.PI / 10;
				if(e.Code == Keyboard.Key.E) phi -= cm / MathF.PI / 10;
				if(e.Code == Keyboard.Key.R) height += 5;
				if(e.Code == Keyboard.Key.F) height -= 5;
				if(e.Code == Keyboard.Key.T) horizon += 20;
				if(e.Code == Keyboard.Key.G) horizon -= 20;
			};

			Stopwatch sw = Stopwatch.StartNew();
			
			//window.SetFramerateLimit(60);

			var a = new RectangleShape(new Vector2f(_cMw / 2, _cMh / 2));
			//a.Position = new(0, window.Size.Y - a.Size.Y);
			a.FillColor = new Color(0, 0, 0, 0);
			a.OutlineColor = Color.Red;
			a.OutlineThickness = 2;

			b.FillColor = Color.Blue;

			int distance = 256;
			int z = distance;

			Font font = new(GetResourceStream("font.ttf"));
			Text fpsText = new("FPS", font);
			
			while(window.IsOpen) {
				fpsText.DisplayedString = $"{sw.ElapsedMilliseconds}ms frame time ({1000/sw.ElapsedMilliseconds} FPS)";
				sw.Restart();
				
				if(z > 1) z--;
				else z = distance;
				
				window.Clear();
				window.DispatchEvents();

				LoopRender(ref window, p, phi, height, horizon, 100, distance, z);
				
				window.Draw(a);
				window.Draw(b);
				window.Draw(fpsText);
				
				window.Display();
			}
		}

		private static Stream GetResourceStream(string name) {
			return Assembly.GetExecutingAssembly().GetManifestResourceStream($"Playground.{name}");
		}

		private static void DoParse(string file, out Vector3[,] map, ref int mw, ref int mh) {
			using(var image = Image.Load<Rgba32>(GetResourceStream(file))) {
				var m = new Vector3[image.Width, image.Height];
				mw = image.Width;
				mh = image.Height;
				
				image.ProcessPixelRows(accessor => {
					for(int y = 0; y < accessor.Height; y++) {
						var row = accessor.GetRowSpan(y);

						for(int x = 0; x < row.Length; x++) {
							var pixel = row[x];

							m[x, y] = new(pixel.R, pixel.G, pixel.B);
						}
					}
				});

				map = m;
			}
		}

		private static RectangleShape line = new();

		private static void LoopRender(ref RenderWindow window, Vector2 p, float phi, float height,
		                               float horizon, float scale_height, int distance, int lada) {

			for(int z = distance; z > 1; z--) {
				Render(ref window, p, phi, height, horizon, scale_height, distance, z);
			}
		}

		private static void Render(ref RenderWindow window, Vector2 p, float phi, float height, float horizon, float scale_height, int distance, int z) {
			int screen_width = (int) window.Size.X;
			int screen_height = (int) window.Size.Y;

			var sinphi = MathF.Sin(phi);
			var cosphi = MathF.Cos(phi);

			// 256 -> 0
			//for(int z = distance; z > 1; z--) {
			
				//var left = new Vector2(-z + p.X, -z + p.Y);
				//var right = new Vector2(z + p.X, -z + p.Y);
				
				var left = new Vector2(
					(-cosphi * z - sinphi * z) + p.X,
					(sinphi * z - cosphi * z) + p.Y);
				var right = new Vector2(
					(cosphi * z - sinphi * z) + p.X,
					(-sinphi * z - cosphi * z) + p.Y);

				b.Size = new((right.X - left.X) / 2, 1);
				b.Position = new(left.X / 2, left.Y / 2);
				//b.Position = new(b.Position.X, window.Size.Y - b.Position.Y);
				
				//Console.WriteLine($"{z}: {left} {right}");

				var dx = (right.X - left.X) / screen_width;
				var dy = (right.Y - left.Y) / screen_width;

				for(int i = 0; i < screen_width; i++) {
					var ix = (int) left.X;
					var iy = (int) left.Y;
					if(ix < 0 || iy < 0 || ix >= _hMw || iy >= _hMh) continue;
					
					var pheight = _heightMap[ix, iy];
					var height_on_screen = (height - pheight.X) / z * scale_height + horizon;

					// draw vertical line
					line.Size = new(1, height_on_screen - screen_height);
					var c = _colorMap[ix ,iy];
					line.FillColor = new Color((byte) c.X, (byte) c.Y, (byte) c.Z);
					//Console.WriteLine(dx);
					line.Position = new Vector2f(i, height_on_screen * 2);

					window.Draw(line);

					left.X += dx;
					left.Y += dy;
				}
			//}

			/*int screen_width = (int) window.Size.X;
			int screen_height = (int) window.Size.Y;

			var sinphi = MathF.Sin(phi);
			var cosphi = MathF.Cos(phi);

			for(int z = distance; z > 1; z--) {
				var pleft = new Vector2(p.X, z + p.Y);
				var pright = new Vector2(z + p.X, z + p.Y);

				// var pleft = new Vector2(
				// 	(cosphi * z + sinphi * z) + p.X,
				// 	(sinphi * z + cosphi * z) + p.Y);
				// var pright = new Vector2(
				// 	(cosphi * z + sinphi * z) + p.X,
				// 	(sinphi * z + cosphi * z) + p.Y);

				var dx = (pright.X - pleft.X) / screen_width;
				var dy = (pright.Y - pleft.Y) / screen_width;

				/*if(pleft.X < 0 || pleft.Y < 0) {
					pleft.X += dx;
					continue;
				}#1#

				for(int i = 0; i < screen_width; i++) {
					var pheight = _heightMap[(int) pleft.X, (int) pleft.Y];
					var height_on_screen = (height - pheight.X) / z * scale_height + horizon;

					// draw vertical line
					line.Size = new(1, height_on_screen - screen_height);
					var c = _colorMap[(int) pleft.X, (int) pleft.Y];
					line.FillColor = new Color((byte) c.X, (byte) c.Y, (byte) c.Z);
					line.Position = new Vector2f(i, height_on_screen * 2);

					window.Draw(line);

					pleft.X += dx;
					pleft.Y += dy;
				}
			}*/
		}
	}
}