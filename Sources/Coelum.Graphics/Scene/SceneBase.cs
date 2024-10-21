using System.Collections.Concurrent;
using System.Drawing;
using Coelum.Configuration;
using Coelum.Debug;
using Coelum.Graphics.Node;
using Coelum.Graphics.Node.Component;
using Coelum.Graphics.OpenGL;
using Coelum.Graphics.Texture;
using Coelum.Node;
using Silk.NET.OpenGL;

namespace Coelum.Graphics.Scene {
	
	public abstract class SceneBase : RootNode {

	#region Delegates
		public delegate void LoadEventHandler(Window window);
		public delegate void UnloadEventHandler();
	
		public delegate void FixedUpdateEventHandler(float delta);
		public delegate void UpdateEventHandler(float delta);
		public delegate void RenderEventHandler(float delta);

		public delegate void ShaderSetupEventHandler(ShaderProgram shader);
	#endregion

	#region Events
		public event LoadEventHandler? Load;
		public event UnloadEventHandler? Unload;
		
		public event FixedUpdateEventHandler? FixedUpdate;
		public event UpdateEventHandler? Update;
		public event RenderEventHandler? Render;

		public event ShaderSetupEventHandler? PrimaryShaderSetup;
		public event ShaderSetupEventHandler? SecondaryShaderSetup;
	#endregion

		public Color ClearColor { get; protected set; } = Color.Black;
		
		public Window? Window { get; private set; }
		public GameOptions Options { get; }

		public ShaderProgram PrimaryShader { get; protected set; }
		public List<IShaderOverlay> ShaderOverlays { get; } = new();
		
		public string Id { get; }

		public int Width => Window?.SilkImpl.FramebufferSize.X ?? 0;
		public int Height => Window?.SilkImpl.FramebufferSize.Y ?? 0;

		protected SceneBase(string id) {
			Id = id;

			Directories.Create(Directories.ConfigurationRoot, "scenes");
			Options = new(Path.Combine(Directories.ConfigurationRoot, "scenes", $"{id}.json"));
		}

		public virtual void OnLoad(Window window) {
			Tests.Assert(PrimaryShader != null);
			PrimaryShader.Validate();
			PrimaryShader.AddOverlays(ShaderOverlays);
			
			Window = window;

			// TODO should this really be set each time a scene is changed?
			window.SilkImpl.Size = new(
				Options.GetOrDefault("window_width", window.SilkImpl.Size.X),
				Options.GetOrDefault("window_height", window.SilkImpl.Size.Y)
			);

			window.SilkImpl.Position = new(
				Options.GetOrDefault("window_x", window.SilkImpl.Position.X),
				Options.GetOrDefault("window_y", window.SilkImpl.Position.Y)
			);

			window.SilkImpl.Resize += newSize => {
				Options.Set("window_width", newSize.X);
				Options.Set("window_height", newSize.Y);
			};

			window.SilkImpl.Move += newPosition => {
				Options.Set("window_x", newPosition.X);
				Options.Set("window_y", newPosition.Y);
			};
			
			Load?.Invoke(window);
		}

		public virtual void OnUnload() {
			Window = null;
			Options.Save();
			Unload?.Invoke();
		}

		public virtual void OnUpdate(float delta) {
			Update?.Invoke(delta);
		}

		public virtual void OnFixedUpdate(float delta) {
			FixedUpdate?.Invoke(delta);
		}
		
		public virtual void OnRender(float delta) {
			Gl.ClearColor(ClearColor);
			
			PrimaryShader.Bind();
			PrimaryShaderSetup?.Invoke(PrimaryShader);
			
			PrimaryShader.EnableOverlays(ShaderOverlays);

			int i = 0;

			FindChildrenByComponent((IShaderRenderable renderable) => {
				renderable.Render(PrimaryShader);
			});
			
			Render?.Invoke(delta);
		}
	}
}