using Coelum.Common.Graphics;
using Coelum.Node;
using Coelum.Raven.Node.Component;
using Coelum.Raven.Terminal;

namespace Coelum.Raven.Node {
	
	public class CharNode : SpatialNode {

		public char Character { get; set; }
		
		public CharNode(char value) {
			Character = value;
		}
		
		public override void Render(TerminalBase terminal) {
			terminal.Write(Character, Position);
		}
	}
}