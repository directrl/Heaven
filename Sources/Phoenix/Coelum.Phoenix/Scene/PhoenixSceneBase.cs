using System.Collections.Concurrent;
using System.Drawing;
using Coelum.Configuration;
using Coelum.Debug;
using Coelum.Common.Graphics;
using Coelum.Phoenix.Node;
using Coelum.Phoenix.Texture;
using Coelum.Node;
using Coelum.Phoenix.Node.Component;
using Coelum.Phoenix.OpenGL;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix.Scene {
	
	public abstract class PhoenixSceneBase : SceneBase {

	#region Delegates
		public delegate void ShaderSetupEventHandler(ShaderProgram shader);
	#endregion

	#region Events
		public event ShaderSetupEventHandler? PrimaryShaderSetup;
		public event ShaderSetupEventHandler? SecondaryShaderSetup;
	#endregion

		public Color ClearColor { get; protected set; } = Color.Black;

		public ShaderProgram PrimaryShader { get; protected set; }
		public List<IShaderOverlay> ShaderOverlays { get; } = new();

		public new SilkWindow? Window
			=> base.Window == null ? null : (SilkWindow) base.Window;

		protected PhoenixSceneBase(string id) : base(id) { }

		public virtual void OnLoad(SilkWindow window) { }
		public override void OnLoad(WindowBase window) {
			base.OnLoad(window);
			
			Tests.Assert(window is SilkWindow, "Phoenix renderer scenes work only with" +
			             "Pheonix renderer windows!");
			OnLoad((SilkWindow) window);
			
			Tests.Assert(PrimaryShader != null, "The primary shader cannot be null");
			PrimaryShader.Validate();
			PrimaryShader.AddOverlays(ShaderOverlays);

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
		
		public override void OnRender(float delta) {
			Gl.ClearColor(ClearColor);
			
			PrimaryShader.Bind();
			PrimaryShaderSetup?.Invoke(PrimaryShader);
			
			PrimaryShader.EnableOverlays(ShaderOverlays);

			base.OnRender(delta);
			
			// TODO
			FindChildrenByComponent((IShaderRenderable renderable) => {
				renderable.Render(PrimaryShader);
			});
		}
	}
}