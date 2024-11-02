using System.Drawing;
using Coelum.Common.Graphics;
using Coelum.Debug;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.ECS.Nodes;
using Coelum.Phoenix.ECS.System;
using Coelum.Phoenix.OpenGL;
using Coelum.Resources;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix {
	
	public abstract class PhoenixScene : SceneBase {

	#region Delegates
		public delegate void ShaderSetupEventHandler(ShaderProgram shader);
	#endregion

	#region Events
		public event ShaderSetupEventHandler? PrimaryShaderSetup;
	#endregion

		public Color ClearColor { get; protected set; } = Color.Black;

		public ShaderProgram PrimaryShader { get; protected set; }
		public List<IShaderOverlay> ShaderOverlays { get; } = new();

		public new SilkWindow? Window
			=> base.Window == null ? null : (SilkWindow) base.Window;

		protected CameraBase? CurrentCamera {
			get {
				CameraBase? currentCamera = null;
				
				QueryChildren()
					.Each(node => {
						if(node is CameraBase { Current: true } camera) {
							currentCamera = camera;
						}
					})
					.Execute();

				return currentCamera;
			}
		}

		protected PhoenixScene(string id) : base(id) { }

		public virtual void OnLoad(SilkWindow window) { }
		public override void OnLoad(WindowBase window) {
			PrimaryShader = new(
				Module.RESOURCES,
				new(ShaderType.FragmentShader,
				    Module.RESOURCES[ResourceType.SHADER, "scene.frag"]),
				new(ShaderType.VertexShader,
				    Module.RESOURCES[ResourceType.SHADER, "scene.vert"])
			);
			
			ClearSystems();
			base.OnLoad(window);
			
			Tests.Assert(window is SilkWindow, "Phoenix renderer scenes work only with" +
			             "Phoenix renderer windows!");
			OnLoad((SilkWindow) window);
			
			Tests.Assert(PrimaryShader != null, "The primary shader cannot be null");
			PrimaryShader.Validate();
			PrimaryShader.AddOverlays(ShaderOverlays);
			
			AddSystem("RenderPre", new CameraSystem(PrimaryShader));
			AddSystem("RenderPre", new RenderSystem(PrimaryShader));
			AddSystem("UpdatePost", new TransformSystem());

			var pWindow = (SilkWindow) window;

			// TODO should this really be set each time a scene is changed?
			pWindow.SilkImpl.Size = new(
				Options.GetOrDefault("window_width", pWindow.SilkImpl.Size.X),
				Options.GetOrDefault("window_height", pWindow.SilkImpl.Size.Y)
			);

			pWindow.SilkImpl.Position = new(
				Options.GetOrDefault("window_x", pWindow.SilkImpl.Position.X),
				Options.GetOrDefault("window_y", pWindow.SilkImpl.Position.Y)
			);

			pWindow.SilkImpl.Resize += newSize => {
				Options.Set("window_width", newSize.X);
				Options.Set("window_height", newSize.Y);
			};

			pWindow.SilkImpl.Move += newPosition => {
				Options.Set("window_x", newPosition.X);
				Options.Set("window_y", newPosition.Y);
			};
		}

		public override void OnUpdate(float delta) {
			this.Process("UpdatePre", delta);
			base.OnUpdate(delta);
			this.Process("UpdatePost", delta);
		}

		public override void OnRender(float delta) {
			Gl.ClearColor(ClearColor);
			
			PrimaryShader.Bind();
			PrimaryShaderSetup?.Invoke(PrimaryShader);

			var environment = QuerySingleton<SceneEnvironment>();
			if(environment != null) {
				environment.Load(PrimaryShader);
			}

			this.Process("RenderPre", delta);
			base.OnRender(delta);
			this.Process("RenderPost", delta);
		}
	}
}