using System.Drawing;
using Coelum.Common.Graphics;
using Coelum.Common.Input;
using Coelum.Debug;
using Coelum.ECS;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.ECS;
using Coelum.Phoenix.ECS.Nodes;
using Coelum.Phoenix.ECS.Systems;
using Coelum.Phoenix.Lighting;
using Coelum.Phoenix.OpenGL;
using Coelum.Phoenix.OpenGL.UBO;
using Coelum.Phoenix.UI;
using Coelum.Resources;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix {
	
	public abstract class PhoenixScene : SceneBase {

		public Color ClearColor { get; protected set; } = Color.Black;
		public ShaderProgram PrimaryShader { get; protected set; }

		public List<ShaderOverlay[]> ShaderOverlays { get; protected set; } = new();
		public List<OverlayUI> UIOverlays { get; protected set; } = new();

		public new SilkWindow? Window
			=> base.Window == null ? null : (SilkWindow) base.Window;
		
		public Viewport? PrimaryViewport {
			get {
				Viewport? viewport = null;
				
				QueryChildren<Viewport>()
					.Each(node => {
						if(node.Framebuffer == Window?.Framebuffer) {
							viewport = node;
						}
					})
					.Execute();

				return viewport;
			}
		}
		
		public CameraBase? PrimaryCamera {
			get => PrimaryViewport?.Camera;
		}
		
		public KeyBindings KeyBindings { get; }

		protected PhoenixScene(string id) : base(id) {
			// initialize additional property importers/exporters
			typeof(PropertyExporters).TypeInitializer?.Invoke(null, null);
			typeof(PropertyImporters).TypeInitializer?.Invoke(null, null);
			
			KeyBindings = new(Id);
		}

		public virtual void OnLoad(SilkWindow window) {
			base.Window = window;
		}
		
		public override void OnLoad(WindowBase window) {
			if(window is not SilkWindow silkWindow) {
				throw new ArgumentException("Phoenix renderer scenes work only with" +
				                            " Phoenix renderer windows!");
			}
			
			PrimaryShader = new(
				Module.RESOURCES,
				new(ShaderType.FragmentShader,
				    Module.RESOURCES[ResourceType.SHADER, "scene.frag"]),
				new(ShaderType.VertexShader,
				    Module.RESOURCES[ResourceType.SHADER, "scene.vert"])
			);

			foreach(var o in ShaderOverlays) {
				PrimaryShader.AddOverlays(o);
			}

		#region Input
			foreach(var keyboard in silkWindow.Input.Keyboards) {
				keyboard.KeyUp += (_, key, _) => KeyBindings.Input(KeyAction.Release, (int) key);
				keyboard.KeyDown += (_, key, _) => KeyBindings.Input(KeyAction.Press, (int) key);
			}
		#endregion
			
			ClearSystems();
			ClearNodes(unexportable: true);
			base.OnLoad(window);
			
			if(!PrimaryShader._ready) {
				PrimaryShader.Validate();
				PrimaryShader.Build();
			}
			
			PrimaryShader.Bind();
			
			OnLoad((SilkWindow) window);
			
			AddSystem(new TransformSystem());
			AddSystem(new ObjectRenderSystem(PrimaryShader));
			AddSystem(new UISystem());
			AddSystem(new ViewportRenderSystem(PrimaryShader, DoRender));
			
			if(PrimaryShader.HasOverlays(PhongShading.OVERLAYS)
			   || PrimaryShader.HasOverlays(GouraudShading.OVERLAYS)) {
				AddSystem(new LightingSystem(PrimaryShader));
			}

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
			this.Process(SystemPhase.RENDER_PRE, delta);
			base.OnUpdate(delta);
			this.Process(SystemPhase.RENDER_POST, delta);
		}

		public override void OnRender(float delta) {
			this.Process(SystemPhase.RENDER, delta);
		}

		protected virtual void DoRender(float delta) {
			void Clear() {
				Gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
				Gl.ClearColor(ClearColor);
			}

			Clear();
			GLManager.SetDefaults();
			
			PrimaryShader.Bind();

			var environment = QuerySingleton<SceneEnvironment>();
			if(environment != null) {
				environment.Load(PrimaryShader);
			}

			this.Process(SystemPhase.RENDER_PRE, delta);
			base.OnRender(delta);
			this.Process(SystemPhase.RENDER_POST, delta);
		}

		protected void UpdateKeyBindings() {
			foreach(var keyboard in Window!.Input.Keyboards) {
				KeyBindings.Update(new SilkKeyboard(keyboard));
			}
		}
	}
}