using Coelum.Common.Graphics;
using Coelum.Common.Input;
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

		public class FunnyCameraSystem : EcsSystem {

			public FunnyCameraSystem(ShaderProgram sh1, ShaderProgram sh2) : base("funny cmaera system") {
				Action = (root, delta) => {
					var cm1 = sh1.GetUBO<CameraMatrices>();
					var cm2 = sh2.GetUBO<CameraMatrices>();

					cm2.Projection = cm1.Projection;
					cm2.View = cm1.View;
					cm2.CameraPos = cm1.CameraPos;
					cm2.Upload();
				};
			}
		}

		private PhoenixScene _scene;
		private bool _editorCamera;
		
		public KeyBindings KeyBindings { get; }
		public Framebuffer OutputFramebuffer { get; internal set; }
		
		// TODO how would you resize target scene cameras to imgui window size? (i guess you could just not)
		public OutputScene(string name, bool setupEditorCamera = true) : base(name) {
			_scene = EditorApplication.TargetScene;
			_editorCamera = setupEditorCamera;
			
			KeyBindings = new(name);
			this.SetupKeyBindings(KeyBindings);

			OutputFramebuffer = new(512, 512);
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);
//WHYU DOESN OTNWQAOIRHREIWOUGHNWEOIRHNWROIGUHERIOULGTHNERIOUTB ONONTOTNHITGNOSJV;K'LBL,ZF
DX]BX
	
	/S//SA
			if(_editorCamera) {
				var camera = new PerspectiveCamera(window) {
					FOV = 60
				};
				Add(camera);
				camera.GetComponent<Transform, Transform3D>()
				      .Position = new(4, 0, -4);
				camera.Current = true;
				
				
				
				_ = new FreeCamera3D(camera, this, KeyBindings);
			
				//TODO WHY DOES THIS NOT WORK
				_scene.AddSystem("RenderPre", new FunnyCameraSystem(PrimaryShader, _scene.PrimaryShader));
			}
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);
			this.UpdateKeyBindings(KeyBindings);

			// TODO WHY IS CURRENT CAMERA APPARENTLY NULL
			if(CurrentCamera is Camera3D c3d) {
				c3d.GetComponent<Transform, Transform3D>().Yaw += delta * 2;
			}
		}

		public override void OnRender(float delta) {
			this.Process("RenderPre", delta);
			
			OutputFramebuffer.Bind();

			var funny = _scene.QuerySystem<FunnyCameraSystem>("RenderPre");
			if(funny != null) {
				if(_editorCamera) funny.Enabled = true;
				else funny.Enabled = false;
			}
			
			_scene.OnRender(delta);
		}
	}
}