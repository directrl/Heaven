namespace Coelum.ECS.Prefab {
	
	public interface IPrefab {

		public Node Create(NodeRoot root);
	}

	public class PrefabAttribute : Attribute {

		public string Name { get; set; }
		
		public PrefabAttribute(string name) {
			Name = name;
		}
	}
}