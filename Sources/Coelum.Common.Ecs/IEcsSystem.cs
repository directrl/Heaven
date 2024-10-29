using System.Runtime.Intrinsics.Arm;
using Flecs.NET.Core;

namespace Coelum.Common.Ecs {
	
	public interface IEcsSystem {

		public static virtual System<S1> Create<S1>(World world) => default;
		public static virtual System<S1, S2> Create<S1, S2>(World world) => default;
		public static virtual System<S1, S2, S3> Create<S1, S2, S3>(World world) => default;
	}
	
	public interface IEcsSystem<T1> {

		public static virtual System<S1> Create<S1>(World world, T1 arg1) => default;
		public static virtual System<S1, S2> Create<S1, S2>(World world, T1 arg1) => default;
		public static virtual System<S1, S2, S3> Create<S1, S2, S3>(World world, T1 arg1) => default;
	}
	
	public interface IEcsSystem<T1, T2> {
		public static virtual System<S1> Create<S1>(World world, T1 arg1, T2 arg2) => default;
		public static virtual System<S1, S2> Create<S1, S2>(World world, T1 arg1, T2 arg2) => default;
		public static virtual System<S1, S2, S3> Create<S1, S2, S3>(World world, T1 arg1, T2 arg2) => default;
	}
	
	public interface IEcsSystem<T1, T2, T3> {
		
		public static virtual System<S1> Create<S1>(World world, T1 arg1, T2 arg2, T3 arg3) => default;
		public static virtual System<S1, S2> Create<S1, S2>(World world, T1 arg1, T2 arg2, T3 arg3) => default;
		public static virtual System<S1, S2, S3> Create<S1, S2, S3>(World world, T1 arg1, T2 arg2, T3 arg3) => default;
	}
}