namespace Coelum.ECS {
	
	public partial class NodeRoot {
		
		[Obsolete("Use v2 queries")]
		public Query<Node, TComponent> Query<TComponent>() where TComponent : INodeComponent {
			return new(
				each => {
					if(!_componentNodeMap.ContainsKey(typeof(TComponent))) return;
					
					foreach(var node in _componentNodeMap[typeof(TComponent)]) {
						each?.Invoke(node, (TComponent) node.Components[typeof(TComponent)]);
					}
				},
				each => {
					if(!_componentNodeMap.ContainsKey(typeof(TComponent))) return;
					
					Parallel.ForEach(_componentNodeMap[typeof(TComponent)], node => {
						each?.Invoke(node, (TComponent) node.Components[typeof(TComponent)]);
					});
				}
			);
		}
		
		[Obsolete("Use v2 queries")]
		public Query<Node, TComponent1, TComponent2> Query<TComponent1, TComponent2>()
			where TComponent1 : INodeComponent
			where TComponent2 : INodeComponent {
			
			return new(
				each => {
					if(!_componentNodeMap.ContainsKey(typeof(TComponent1))
					   || !_componentNodeMap.ContainsKey(typeof(TComponent2))) return;
					
					foreach(var node in _componentNodeMap[typeof(TComponent1)]) {
						bool r1 = node.TryGetComponent<TComponent1>(out var c1);
						bool r2 = node.TryGetComponent<TComponent2>(out var c2);
						
						if(r1 && r2) each?.Invoke(node, c1, c2);
					}
				},
				each => {
					if(!_componentNodeMap.ContainsKey(typeof(TComponent1))
					   || !_componentNodeMap.ContainsKey(typeof(TComponent2))) return;
					
					Parallel.ForEach(_componentNodeMap[typeof(TComponent1)], node => {
						bool r1 = node.TryGetComponent<TComponent1>(out var c1);
						bool r2 = node.TryGetComponent<TComponent2>(out var c2);
						
						if(r1 && r2) each?.Invoke(node, c1, c2);
					});
				}
			);
		}
	}
}