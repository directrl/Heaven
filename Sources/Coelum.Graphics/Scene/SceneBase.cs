using System.Drawing;
using Coelum.Configuration;
using Coelum.Debug;
using Coelum.Graphics.OpenGL;
using Silk.NET.OpenGL;

namespace Coelum.Graphics.Scene {
	
	public abstract class SceneBase {

	#region Delegates
		public delegate void LoadEventHandler(Window window);
		public delegate void UnloadEventHandler();
	
		public delegate void FixedUpdateEventHandler(float delta);
		public delegate void UpdateEventHandler(float delta);
		public delegate void RenderEventHandler(GL gl, float delta);

		public delegate void ShaderSetupEventHandler(GL gl, string id, ShaderProgram shader);
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
			
			PrimaryShader.Bind();
			PrimaryShaderSetup?.Invoke(gl, "", PrimaryShader);
			
			PrimaryShader.EnableOverlays(ShaderOverlays);

			if(Debugging.WireframeRendering) {
				gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
			}
			
			Render?.Invoke(gl, delta);
		}
	}
}