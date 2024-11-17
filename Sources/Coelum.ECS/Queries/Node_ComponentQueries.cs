namespace Coelum.ECS {
	
	public partial class Node {
		
		public TComponent AddComponent<TComponent>(TComponent component) where TComponent : INodeComponent {
			Components[typeof(TComponent)] = component;
			component.Owner = this;

			if(component.GetType() != typeof(TComponent)) {
				Components[component.GetType()] = component;
			}
			
			return component;
		}

		public void RemoveComponent<TComponent>() where TComponent : INodeComponent {
			Components.Remove(typeof(TComponent));

			Components = Components
			             .Where(kv => kv.Value.GetType() != typeof(TComponent))
			             .ToDictionary(kv => kv.Key, kv => kv.Value);
		}

		public TComponent GetComponent<TComponent>() where TComponent : INodeComponent {
			return (TComponent) Components[typeof(TComponent)];
		}
		
		public TRealComponent GetComponent<TBaseComponent, TRealComponent>()
			where TBaseComponent : INodeComponent
			where TRealComponent : TBaseComponent {
			
			return (TRealComponent) Components[typeof(TBaseComponent)];
		}
		
		public bool TryGetComponent<TComponent>(out TComponent result) where TComponent : INodeComponent {
			if(HasComponent<TComponent>()) {
				result = (TComponent) Components[typeof(TComponent)];
				return true;
			}

			result = default;
			return false;
		}
		
		public bool TryGetComponent<TBaseComponent, TRealComponent>(out TRealComponent result)
			where TBaseComponent : INodeComponent
			where TRealComponent : TBaseComponent {
			
			if(Components.TryGetValue(typeof(TBaseComponent), out var component)) {
				if(component is TRealComponent real) {
					result = real;
					return true;
				}
			}

			result = default;
			return false;
		}

		public bool HasComponent<TComponent>() where TComponent : INodeComponent {
			return Components.ContainsKey(typeof(TComponent));
		}
		
		public bool HasComponent<TBaseComponent, TRealComponent>()
			where TBaseComponent : INodeComponent
			where TRealComponent : TBaseComponent {

			if(!Components.TryGetValue(typeof(TBaseComponent), out var component)) {
				return false;
			}
			
			return component is TRealComponent;
		}
	}
}