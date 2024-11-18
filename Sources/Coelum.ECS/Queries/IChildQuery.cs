namespace Coelum.ECS.Queries {
	
	public interface IChildQuery : IQuery {

		void IQuery.Call(NodeRoot root) { }
		public void Call(NodeRoot root, Node child);
	}
}