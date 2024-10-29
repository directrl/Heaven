using Flecs.NET.Core;

namespace Coelum.Common.Ecs {
	
	public static class WorldExtensions {

		public static void Import<T>(this World world)
			where T : IEcsModule, new() {
			
			world.Import<T>();
			new T().Setup(world);
		}
		
		public static void Import<T, T1>(this World world, T1 arg1)
			where T : IEcsModule<T1>, new() {
			
			world.Import<T>();
			new T().Setup(world, arg1);
		}
		
		public static void Import<T, T1, T2>(this World world, T1 arg1, T2 arg2)
			where T : IEcsModule<T1, T2>, new() {
			
			world.Import<T>();
			new T().Setup(world, arg1, arg2);
		}
		
		public static void Import<T, T1, T2, T3>(this World world, T1 arg1, T2 arg2, T3 arg3)
			where T : IEcsModule<T1, T2, T3>, new() {
			
			world.Import<T>();
			new T().Setup(world, arg1, arg2, arg3);
		}
	}
}