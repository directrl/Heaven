using Coelum.Common.Graphics;
using Coelum.Common.Input;
using Coelum.Core;
using Coelum.ECS;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.ECS.System;
using Coelum.Phoenix.Editor.Camera;
using Coelum.Phoenix.Input;
using Coelum.Phoenix.OpenGL;
using Coelum.Phoenix.OpenGL.UBO;
using Coelum.Phoenix.UI;
using Hexa.NET.ImGuizmo;

namespace Coelum.Phoenix.Editor {
	
	public class OutputScene : PhoenixScene {

		private PhoenixScene _scene;
		internal bool _editor;

		// TODO support 2D camera
		public FreeCamera3D? FreeCamera { get; private set; }
		
		public KeyBindings KeyBindings { get; }
		public Framebuffer OutputFramebuffer { get; }
		public Viewport OutputViewport { get; private set; }
		
		public OutputScene(string name, bool setupEditorCamera = true) : base(name) {
			_scene = EditorApplication.TargetScene;
			_editor = setupEditorCamera;
			
			KeyBindings = new(name);
			this.SetupKeyBindings(KeyBindings);

			OutputFramebuffer = new(new(512, 512));
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);

			var camera = _scene.PrimaryCamera;

			if(_editor) {
				camera = new PerspectiveCamera() {
					FOV = 60,
					Name = "EditorCamera"
				};
				
				FreeCamera = new((Camera3D) camera, this, KeyBindings);
			}

			if(camera != null) {
				OutputViewport = new(camera, OutputFramebuffer) {
					Hidden = _editor,
					Name = _editor ? "Editor Viewport" : "Editor Output Viewport"
				};
				_scene.Add(OutputViewport);
			} else {
				Heaven.AppLogger.Warning("Scene does not contain an active camera");
			}
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);
			this.UpdateKeyBindings(KeyBindings);
		}

		public override void OnRender(float delta) {
			_scene.OnRender(delta);
		}
	}
}