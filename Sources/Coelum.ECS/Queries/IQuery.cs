namespace Coelum.ECS.Queries {
	
	public interface IQuery<T> {

		public bool Parallel { get; set; }
		
		public SystemPhase Phase { get; set; }

		public bool Call(NodeRoot root, T param);
		public void Reset(NodeRoot root);
	}
}