namespace Coelum.ECS {
	
	public partial class Node {
		
		public TComponent AddComponent<TComponent>(TComponent component) where TComponent : INodeComponent {
			Components[typeof(TComponent)] = component;
			component.Owner = this;
			return component;
		}

		public TComponent GetComponent<TComponent>() where TComponent : INodeComponent {
			return (TComponent) Components[typeof(TComponent)];
		}
		
		public TRealComponent GetComponent<TBaseComponent, TRealComponent>()
			where TRealComponent : INodeComponent
			where TBaseComponent : INodeComponent {
			
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
			where TRealComponent : INodeComponent
			where TBaseComponent : INodeComponent {
			
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
			where TRealComponent : INodeComponent
			where TBaseComponent : INodeComponent {

			if(!Components.TryGetValue(typeof(TBaseComponent), out var component)) {
				return false;
			}
			
			return component is TRealComponent;
		}
	}
}