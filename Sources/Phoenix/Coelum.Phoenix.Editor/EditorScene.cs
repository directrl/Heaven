using System.Drawing;
using System.Numerics;
using System.Reflection;
using Coelum.Common.Input;
using Coelum.Phoenix.Editor.Scanning;
using Coelum.Phoenix.Input;
using Coelum.Phoenix.OpenGL;
using Hexa.NET.ImGui;
using Silk.NET.GLFW;
using Silk.NET.Windowing.Glfw;

using static Coelum.Phoenix.OpenGL.GlobalOpenGL;
using static Coelum.Phoenix.GLFW.GlobalGLFW;

namespace Coelum.Phoenix.Editor {
	
	public class EditorScene : PhoenixScene {
		
		private Vector2 _lastFramebufferSize = Vector2.Zero;
		
		public KeyBindings KeyBindings { get; }
		
		public OutputScene OutputScene { get; }
		public Framebuffer OutputFramebuffer { get; private set; }

		public unsafe EditorScene() : base("editor-main") {
			OutputScene = new($"editor-{EditorApplication.TargetAssembly.GetName().FullName}");
			OutputFramebuffer = new(512, 512);
			
			ClearColor = Color.FromArgb(0, 0, 0, 0);
			
			var prefabs = PrefabScanner.Scan(EditorApplication.TargetAssembly);

		#region Output scene loop setup
			Update += OutputScene.OnUpdate;
			FixedUpdate += OutputScene.OnFixedUpdate;
			Load += OutputScene.OnLoad;
			Unload += OutputScene.OnUnload;
		#endregion
			
			Render += delta => {
				Gl.ClearColor(0, 0, 0, 0);
				
				HexaImGui.Begin();
				//ImGui.ShowDemoWindow();

				if(ImGui.Begin("prefabs")) {
					foreach(var t in prefabs) {
						ImGui.Text($"{t.Name}");
					}
					ImGui.End();
				}

				if(ImGui.Begin("Output")) {
					var size = ImGui.GetContentRegionAvail();

					if(size != _lastFramebufferSize) {
						OutputFramebuffer = new((uint) size.X, (uint) size.Y);
						_lastFramebufferSize = size;
					}
					
					ImGui.Image(
						new ImTextureID(OutputFramebuffer.Texture.Id),
						new Vector2(OutputFramebuffer.Width, OutputFramebuffer.Height),
						new Vector2(0, 1), new Vector2(1, 0)
					);

					ImGui.End();
				}
				
				HexaImGui.End();
			};

			Update += delta => {
				var handle = GlfwWindowing.GetHandle(Window.SilkImpl);
				
				GlFw.SetWindowAttrib(handle, WindowAttributeSetter.Floating, true);
				
				if(!ImGui.GetIO().WantCaptureMouse) {
					GlFw.SetWindowAttrib(handle, (WindowAttributeSetter) 0x0002000D, true);
				} else {
					GlFw.SetWindowAttrib(handle, (WindowAttributeSetter) 0x0002000D, false);
				}
			};

			// KeyBindings = new(Id);
			// this.SetupKeyBindings(KeyBindings);
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);
			
			HexaImGui.Setup(window);
		}

		public override void OnRender(float delta) {
			// OutputFramebuffer.Bind();
			// OutputScene.OnRender(delta);
			// Gl.BindFramebuffer(OutputFramebuffer.Target, 0);
			Window.Framebuffer.Bind();
			
			base.OnRender(delta);
		}
	}
}