using System.Drawing;
using Coelum.Common.Graphics;
using Coelum.Debug;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.ECS;
using Coelum.Phoenix.ECS.Nodes;
using Coelum.Phoenix.ECS.System;
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
		
		public ShaderOverlay[][]? ShaderOverlays { get; protected set; }
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

		protected PhoenixScene(string id) : base(id) {
			// initialize additional property importers/exporters
			typeof(PropertyExporters).TypeInitializer?.Invoke(null, null);
			typeof(PropertyImporters).TypeInitializer?.Invoke(null, null);
		}

		public virtual void OnLoad(SilkWindow window) {
			base.Window = window;
		}
		
		public override void OnLoad(WindowBase window) {
			PrimaryShader = new(
				Module.RESOURCES,
				new(ShaderType.FragmentShader,
				    Module.RESOURCES[ResourceType.SHADER, "scene.frag"]),
				new(ShaderType.VertexShader,
				    Module.RESOURCES[ResourceType.SHADER, "scene.vert"])
			);

			if(ShaderOverlays != null) {
				foreach(var o in ShaderOverlays) {
					PrimaryShader.AddOverlays(o);
				}
			}
			
			ClearSystems();
			ClearNodes(unexportable: true);
			base.OnLoad(window);
			
			if(!PrimaryShader._ready) {
				PrimaryShader.Validate();
				PrimaryShader.Build();
			}
			
			PrimaryShader.Bind();
			
			Tests.Assert(window is SilkWindow, "Phoenix renderer scenes work only with" +
			             "Phoenix renderer windows!");
			OnLoad((SilkWindow) window);
			
			AddSystem("RenderPre", new TransformSystem());
			AddSystem("RenderPost", new ObjectRenderSystem(PrimaryShader));
			AddSystem("RenderPost", new UISystem());
			AddSystem("Render", new ViewportRenderSystem(PrimaryShader, DoRender));
			
			if(PrimaryShader.HasOverlays(PhongShading.OVERLAYS)
			   || PrimaryShader.HasOverlays(GouraudShading.OVERLAYS)) {
				AddSystem("RenderPre", new LightingSystem(PrimaryShader));
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
			this.Process("UpdatePre", delta);
			base.OnUpdate(delta);
			this.Process("UpdatePost", delta);
		}

		public override void OnRender(float delta) {
			this.Process("Render", delta);
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

			this.Process("RenderPre", delta);
			base.OnRender(delta);
			this.Process("RenderPost", delta);
		}
	}
}