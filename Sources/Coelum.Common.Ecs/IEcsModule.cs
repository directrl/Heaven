using Flecs.NET.Core;

namespace Coelum.Common.Ecs {
	
	public interface IEcsModule : IFlecsModule {

		public void Setup(World world);
	}
	
	public interface IEcsModule<T1> : IFlecsModule {
		
		public void Setup(World world, T1 arg1);
	}
	
	public interface IEcsModule<T1, T2> : IFlecsModule {

		public void Setup(World world, T1 arg1, T2 arg2);
	}
	
	public interface IEcsModule<T1, T2, T3> : IFlecsModule {
		
		public void Setup(World world, T1 arg1, T2 arg2, T3 arg3);
	}
}