namespace Coelum.ECS {
	
	public partial class NodeRoot {
		
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
		
		public Query<Node, TComponent1, TComponent2> Query<TComponent1, TComponent2>()
			where TComponent1 : INodeComponent
			where TComponent2 : INodeComponent {
			
			return new(
				each => {
					if(!_componentNodeMap.ContainsKey(typeof(TComponent1))
					   || !_componentNodeMap.ContainsKey(typeof(TComponent2))) return;
					
					foreach(var node in _componentNodeMap[typeof(TComponent1)]) {
						var c1 = node.GetComponent<TComponent1>();
						var c2 = node.GetComponent<TComponent2>();
						
						if(c1 != null && c2 != null) each?.Invoke(node, c1, c2);
					}
				},
				each => {
					if(!_componentNodeMap.ContainsKey(typeof(TComponent1))
					   || !_componentNodeMap.ContainsKey(typeof(TComponent2))) return;
					
					Parallel.ForEach(_componentNodeMap[typeof(TComponent1)], node => {
						var c1 = node.GetComponent<TComponent1>();
						var c2 = node.GetComponent<TComponent2>();
						
						if(c1 != null && c2 != null) each?.Invoke(node, c1, c2);
					});
				}
			);
		}
	}
}