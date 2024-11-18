namespace Coelum.ECS.Queries {
	
	public interface IQuery {
		
		public bool Parallel { get; set; }
		public SystemPhase Phase { get; set; }

		public void Call(NodeRoot root);
		public void Reset(NodeRoot root);
	}
}