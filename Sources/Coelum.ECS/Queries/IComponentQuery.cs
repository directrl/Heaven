namespace Coelum.ECS.Queries {
	
	public interface IComponentQuery : IQuery {

		bool IQuery.Call(NodeRoot root) => false;
		public bool Call(NodeRoot root, INodeComponent component);
	}
}