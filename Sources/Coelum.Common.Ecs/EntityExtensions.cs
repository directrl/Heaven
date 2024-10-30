using Flecs.NET.Core;

namespace Coelum.Common.Ecs {
	
	public static class EntityExtensions {

		// TODO this is SUPER slow even with a flat tree (loses about 40% performance)
		public static T GetGlobal<T, TComponent>(this Entity e,
		                                         Func<TComponent, T> propertyGetter,
		                                         Func<T, T, T> action) {
			
			if(!e.Has<TComponent>()) return default;

			var eLocal = propertyGetter.Invoke(e.Get<TComponent>());
			var parent = e.Parent();

			if(parent.Id != 0 && parent.Has<TComponent>()) {
				var pGlobal = GetGlobal(e, propertyGetter, action);
				return action.Invoke(eLocal, pGlobal);
			}

			return eLocal;
		}
	}
}