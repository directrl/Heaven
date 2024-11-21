namespace Coelum.ECS {
	
	public partial class NodeRoot {
		
		public Node? GetChild(ulong id) => _nodes.GetValueOrDefault(id);
		public Node? GetChild(string path) => _pathNodeMap.GetValueOrDefault(path);

		public List<Node>? GetChildren<TNode>() where TNode : Node
			=> _typeNodeMap.GetValueOrDefault(typeof(TNode));
	}
}