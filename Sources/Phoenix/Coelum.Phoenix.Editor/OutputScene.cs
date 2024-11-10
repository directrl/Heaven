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

namespace Coelum.Phoenix.Editor {
	
	public class OutputScene : PhoenixScene {

		private PhoenixScene _scene;
		private bool _editorCamera;

		private FreeCamera3D? _freeCamera;
		
		public KeyBindings KeyBindings { get; }
		public Framebuffer OutputFramebuffer { get; internal set; }
		
		// TODO how would you resize target scene cameras to imgui window size? (i guess you could just not)
		public OutputScene(string name, bool setupEditorCamera = true) : base(name) {
			_scene = EditorApplication.TargetScene;
			_editorCamera = setupEditorCamera;
			
			KeyBindings = new(name);
			this.SetupKeyBindings(KeyBindings);

			OutputFramebuffer = new(new(512, 512));
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);

			var camera = _scene.CurrentCamera;

			if(_editorCamera) {
				camera = new PerspectiveCamera() {
					FOV = 60,
					Name = "EditorCamera"
				};
				//Add(camera);
				camera.GetComponent<Transform, Transform3D>()
				       .Position = new(4, 1, -4);
				camera.Current = true;
				
				
				
				_freeCamera = new((Camera3D) camera, this, KeyBindings);
			
				//TODO WHY DOES THIS NOT WORK
				//_scene.AddSystem("RenderPre", new FunnyCameraSystem(PrimaryShader, _scene.PrimaryShader));
			}

			if(camera != null) {
				_scene.Add(new Viewport(camera, OutputFramebuffer) {
					Hidden = true
				});
			} else {
				Heaven.AppLogger.Warning("Scene does not contain an active camera");
			}
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);
			this.UpdateKeyBindings(KeyBindings);

			// TODO WHY IS CURRENT CAMERA APPARENTLY NULL
			// if(CurrentCamera is Camera3D c3d) {
			// 	c3d.GetComponent<Transform, Transform3D>().Yaw += delta * 2;
			// }
		}

		public override void OnRender(float delta) {
			//this.Process("RenderPre", delta);
			
			//OutputFramebuffer.Bind();

			// var funny = _scene.QuerySystem<FunnyCameraSystem>("RenderPre");
			// if(funny != null) {
			// 	if(_editorCamera) funny.Enabled = true;
			// 	else funny.Enabled = false;
			// }
			
			_scene.OnRender(delta);
		}
	}
}