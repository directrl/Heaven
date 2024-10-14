using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Playground.Scenes.VoxelSpace {
	
	public static class MapLoader {
		
		public static void DoParse(Stream stream, out Vector3[,] map, ref int mw, ref int mh) {
			using(var image = Image.Load<Rgba32>(stream)) {
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
	}
}