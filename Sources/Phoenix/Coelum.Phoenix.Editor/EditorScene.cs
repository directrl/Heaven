using Coelum.Common.Graphics;
using Coelum.Common.Input;
using Coelum.Debug;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.ECS.System;
using Coelum.Phoenix.Editor.Camera;
using Coelum.Phoenix.Editor.UI;
using Coelum.Phoenix.Input;
using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.Editor {
	
	public class EditorScene : PhoenixScene {

		private bool _initialSceneUpdate = true;
		
	#region UI
		public MainUI MainUI { get; private set; }
		public PrefabUI PrefabUI { get; private set; }
		public OutputUI EditorViewUI { get; private set; }
		public OutputUI OutputViewUI { get; private set; }
		public NodeUI NodeUI { get; private set; }
		
		public ResourceSelector ResourceSelector { get; private set; }
		
		public OutputScene EditorView { get; private set; }
		public OutputScene OutputView { get; private set; }
	#endregion
		
		public KeyBindings KeyBindings { get; }
		public FreeCamera3D FreeCamera { get; private set; }

		// TODO imgui window settings dont save
		public EditorScene() : base("editor_main") {
			KeyBindings = new(Id);
			this.SetupKeyBindings(KeyBindings);

			/*var scene = EditorApplication.TargetScene;
			Load += scene.OnLoad;
			Unload += scene.OnUnload;
			Update += scene.OnUpdate;
			FixedUpdate += scene.OnFixedUpdate;*/

			EditorView = new("editor_view");
			OutputView = new("editor_output", false);
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);

		#region UI
			MainUI = new(this);
			PrefabUI = new(this);
			EditorViewUI = new(this, EditorView);
			OutputViewUI = new(this, OutputView);
			NodeUI = new(this);

			ResourceSelector = new(this, EditorApplication.TargetAssembly) {
				Visible = false
			};

			UIOverlays.AddRange(new OverlayUI[] {
				MainUI,
				PrefabUI,
				EditorViewUI,
				OutputViewUI,
				NodeUI,
				ResourceSelector
			});

			var debugOverlay = new DebugOverlay(this, EditorApplication.TargetScene);
			UIOverlays.Add(debugOverlay);
		#endregion
			
			// TODO this is quite messy with two different OnLoad methods
			EditorApplication.TargetScene.OnLoad((WindowBase) window);
			
			// disable any viewports that render to the main window
			EditorApplication.TargetScene.QueryChildren<Viewport>()
			                 .Each(viewport => {
				                 if(viewport.Framebuffer == window.Framebuffer) {
					                 viewport.Enabled = false;
				                 }
			                 })
			                 .Execute();
			
			// disable any UI on the target scene
			var uiSystem = EditorApplication.TargetScene.QuerySystem<UISystem>();
			if(uiSystem != null) uiSystem.Enabled = false;

			EditorView.OnLoad((WindowBase) window);
			OutputView.OnLoad((WindowBase) window);
			
			Add(new Viewport(new Camera2D(), window.Framebuffer));
		}

		public override void OnUnload() {
			base.OnUnload();
			EditorView.OnUnload();
			OutputView.OnUnload();
			EditorApplication.TargetScene.OnUnload();
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			// need to update for 1-frame at the beginning for stuff like lighting TODO why?
			if(MainUI.TargetSceneUpdate || _initialSceneUpdate) {
				EditorApplication.TargetScene.OnUpdate(delta);
				_initialSceneUpdate = false;
			}
			
			EditorView.OnUpdate(delta);
			OutputView.OnUpdate(delta);
			
			this.UpdateKeyBindings(KeyBindings);
		}

		public override void OnRender(float delta) {
			base.OnRender(delta);
			EditorView.OnRender(delta);
			OutputView.OnRender(delta);
			
			Window.Framebuffer.Bind();
		}
	}
}