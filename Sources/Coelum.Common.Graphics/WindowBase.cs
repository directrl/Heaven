namespace Coelum.Common.Graphics {
	
	public abstract class WindowBase {
		
		public float UpdateDelta { get; protected set; }
		public float FixedUpdateDelta { get; protected set; }
		public float RenderDelta { get; protected set; }

		public bool DoUpdates { get; set; } = true;
		
		private SceneBase? _scene;
		public SceneBase? Scene {
			get => _scene;
			set {
				_scene?.OnUnload();
				_scene = value;

				value?.OnLoad(this);
			}
		}

		public abstract bool Update();

		public abstract void Show();
		public abstract void Hide();

		public virtual void Close() {
			Scene?.OnUnload();
			DoUpdates = false;
		}
	}
}