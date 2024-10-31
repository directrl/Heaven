using Coelum.Configuration;
using Coelum.ECS;

namespace Coelum.Common.Graphics {
	
	public abstract class SceneBase : NodeRoot {
		
	#region Delegates
		public delegate void LoadEventHandler(WindowBase window);
		public delegate void UnloadEventHandler();
	
		public delegate void FixedUpdateEventHandler(float delta);
		public delegate void UpdateEventHandler(float delta);
		public delegate void RenderEventHandler(float delta);
	#endregion

	#region Events
		public event LoadEventHandler? Load;
		public event UnloadEventHandler? Unload;
		
		public event FixedUpdateEventHandler? FixedUpdate;
		public event UpdateEventHandler? Update;
		public event RenderEventHandler? Render;
	#endregion

		public WindowBase? Window { get; private set; }
		public GameOptions? Options { get; }
		
		public string Id { get; }

		protected SceneBase(string? id) {
			if(id == null) {
				Id = "";
				return;
			}
			
			Id = id;

			Directories.Create(Directories.ConfigurationRoot, "scenes");
			Options = new(Path.Combine(Directories.ConfigurationRoot, "scenes", $"{id}.json"));
		}

		public virtual void OnLoad(WindowBase window) {
			Window = window;
			Load?.Invoke(window);
		}

		public virtual void OnUnload() {
			Window = null;
			Options?.Save();
			Unload?.Invoke();
		}

		public virtual void OnUpdate(float delta) {
			Update?.Invoke(delta);
		}

		public virtual void OnFixedUpdate(float delta) {
			FixedUpdate?.Invoke(delta);
		}
		
		public virtual void OnRender(float delta) {
			Render?.Invoke(delta);
		}
	}
}