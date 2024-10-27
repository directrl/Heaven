namespace Coelum.Physics.Collision {
	
	public interface ICollidable<T> {
		
		public bool Collides(in T other);
	}
}