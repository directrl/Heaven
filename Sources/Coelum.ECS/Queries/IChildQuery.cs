namespace Coelum.ECS.Queries {
	
	public interface IChildQuery : IQuery {

		bool IQuery.Call(NodeRoot root) => false;
		public bool Call(NodeRoot root, Node child);
	}
}