using System.Drawing;
using Coelum.Physics.Collision;
using Coelum.Raven;
using Coelum.Raven.Node;
using Coelum.Raven.Node.Component;
using Silk.NET.Maths;

namespace RavenPlayground.Node {
	
	public class HouseNode : ObjectNode, ICollidable {

		// TODO loading from file obviously
		public static readonly Cell[,] MODEL = new Cell[6, 6] {
			{ new(' ', backgroundColor: Color.White), new('█', Color.White), new('█', Color.White), new('█', Color.White), new('█', Color.White), new('█', Color.White) },
			{ new(' ', backgroundColor: Color.White), new('░', Color.White), new('░', Color.White), new('░', Color.White), new('░', Color.White), new('█', Color.White) },
			{ new(' ', backgroundColor: Color.White), new(' ', Color.White), new(' ', Color.White), new(' ', Color.White), new(' ', Color.White), new('█', Color.White) },
			{ new(' ', backgroundColor: Color.White), new(' ', Color.White), new(' ', Color.White), new(' ', Color.White), new(' ', Color.White), new('█', Color.White) },
			{ new(' ', backgroundColor: Color.White), new('█', Color.White), new(' ', Color.White), new(' ', Color.White), new('█', Color.White), new('█', Color.White) },
			{ new('░', Color.White), new('░', Color.White), new(' ', Color.White), new(' ', Color.White), new('░', Color.White), new('░', Color.White) }
		};

		public BoundingBox2D<int> AABB => new(new(0, 0), new(5, 5));

		public HouseNode() : base(MODEL) { }
	}
}