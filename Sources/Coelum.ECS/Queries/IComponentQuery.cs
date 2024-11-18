namespace Coelum.ECS.Queries {
	
	public interface IComponentQuery : IQuery {

		void IQuery.Call(NodeRoot root) { }
		public void Call(NodeRoot root, INodeComponent component);
	}
}