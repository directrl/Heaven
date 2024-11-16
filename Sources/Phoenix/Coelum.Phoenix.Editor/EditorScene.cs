using Coelum.Common.Graphics;
using Coelum.Common.Input;
using Coelum.Debug;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.ECS.System;
using Coelum.Phoenix.Editor.Camera;
using Coelum.Phoenix.Editor.UI;
using Coelum.Phoenix.Editor.UI.Prompts;
using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.Editor {
	
	// TODO node picking with raycasting (bepuphysics?)
	public class EditorScene : PhoenixScene {

		private bool _initialSceneUpdate = true;
		
	#region UI
		public MainUI MainUI { get; private set; }
		public OutputUI EditorViewUI { get; private set; }
		public OutputUI OutputViewUI { get; private set; }
		public NodeUI NodeUI { get; private set; }
		
		public ResourceSelector ResourceSelector { get; private set; }
		public NodeSelector NodeSelector { get; private set; }
		public NodeSpawner NodeSpawner { get; private set; }
		
		public OutputScene EditorView { get; private set; }
		public OutputScene OutputView { get; private set; }
	#endregion
		
		public CameraBase? Camera => EditorView.FreeCamera?.Camera;

		public EditorScene() : base("editor_main") {
			EditorApplication.KeyBindings = new(KeyBindings);

			EditorView = new("editor_view");
			OutputView = new("editor_output", false);
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);

		#region UI
			MainUI = new(this);
			EditorViewUI = new(this, EditorView);
			OutputViewUI = new(this, OutputView);
			NodeUI = new(this);
			
			ResourceSelector = new(this, EditorApplication.TargetAssembly);
			NodeSelector = new(this);
			NodeSpawner = new(this);

			UIOverlays.AddRange(new OverlayUI[] {
				MainUI,
				EditorViewUI,
				OutputViewUI,
				NodeUI,
				ResourceSelector,
				NodeSelector,
				NodeSpawner
			});

			if(Debugging.Enabled) {
				var debugOverlay = new DebugOverlay(this, EditorApplication.TargetScene);
				debugOverlay.AdditionalInfo += delta => {
					var camera = EditorView.FreeCamera.Camera;
					
					ImGui.Text($"View camera position: {camera.GetComponent<Transform, Transform3D>().Position}");
					ImGui.Text($"View camera rotation: {camera.GetComponent<Transform, Transform3D>().Rotation}");
					ImGui.Separator();
					ImGui.Text($"View camera yaw: {camera.Yaw}");
					ImGui.Text($"View camera pitch: {camera.Pitch}");
					ImGui.Separator();
				};
				
				UIOverlays.Add(debugOverlay);
			}
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
			
			// disable keybindings on the target scene
			EditorApplication.TargetScene.KeyBindings.Enabled = false;
			
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
			
			EditorApplication.KeyBindings.Update(delta);

			// need to update for 1-frame at the beginning for stuff like lighting TODO why?
			if(MainUI.TargetSceneUpdate || _initialSceneUpdate) {
				EditorApplication.TargetScene.OnUpdate(delta);
				_initialSceneUpdate = false;
			}
			
			EditorView.OnUpdate(delta);
			OutputView.OnUpdate(delta);
			
			UpdateKeyBindings();
		}

		public override void OnRender(float delta) {
			if(EditorView.FreeCamera?.Active ?? false) {
				ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.NoMouse;
			} else {
				ImGui.GetIO().ConfigFlags &= ~ImGuiConfigFlags.NoMouse;
			}

			base.OnRender(delta);
			EditorView.OnRender(delta);
			OutputView.OnRender(delta);
			
			Window.Framebuffer.Bind();
		}
	}
}