namespace Coelum.ECS.Queries {
	
	public interface IQuery<T> {

		internal List<T> Cache { get; }
		
		public bool Parallel { get; set; }
		public bool CacheResults { get; set; }
		
		public SystemPhase Phase { get; set; }

		public bool Call(NodeRoot root, T param);
		public void Reset(NodeRoot root);
	}
}