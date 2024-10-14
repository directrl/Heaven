using System.Drawing;
using Coeli.Configuration;
using Silk.NET.OpenGL;

namespace Coeli.Graphics.Scene {
	
	public abstract class SceneBase {

	#region Delegates
		public delegate void LoadEventHandler(Window window);
		public delegate void UnloadEventHandler();
	
		public delegate void FixedUpdateEventHandler(float delta);
		public delegate void UpdateEventHandler(float delta);
		public delegate void RenderEventHandler(GL gl, float delta);
	#endregion

	#region Events
		public event LoadEventHandler? Load;
		public event UnloadEventHandler? Unload;
		
		public event FixedUpdateEventHandler? FixedUpdate;
		public event UpdateEventHandler? Update;
		public event RenderEventHandler? Render;
	#endregion

		public Color ClearColor { get; protected set; } = Color.Black;
		
		public Window? Window { get; private set; }
		public GameOptions Options { get; }
		
		public string Id { get; }

		public int Width => Window?.SilkImpl.FramebufferSize.X ?? 0;
		public int Height => Window?.SilkImpl.FramebufferSize.Y ?? 0;

		protected SceneBase(string id) {
			Id = id;

			Directories.Create(Directories.ConfigurationRoot, "scenes");
			Options = new(Path.Combine(Directories.ConfigurationRoot, "scenes", $"{id}.json"));
		}

		public virtual void OnLoad(Window window) {
			Window = window;
			Load?.Invoke(window);
		}

		public virtual void OnUnload() {
			Window = null;
			Unload?.Invoke();
		}

		public virtual void OnUpdate(float delta) {
			Update?.Invoke(delta);
		}

		public virtual void OnFixedUpdate(float delta) {
			FixedUpdate?.Invoke(delta);
		}
		
		public virtual void OnRender(GL gl, float delta) {
			gl.ClearColor(ClearColor);
			Render?.Invoke(gl, delta);
		}
	}
}