using Coelum.Common.Graphics;
using Coelum.Common.Input;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.Editor.Camera;
using Coelum.Phoenix.Editor.UI;
using Coelum.Phoenix.Input;
using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.Editor {
	
	public class EditorScene : PhoenixScene {
		
	#region UI
		public MainUI MainUI { get; private set; }
		public PrefabUI PrefabUI { get; private set; }
		public OutputUI EditorViewUI { get; private set; }
		public OutputUI OutputViewUI { get; private set; }
		
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
			OutputViewUI = new(this, EditorView);
			OutputViewUI = new(this, OutputView, false);

			var scene = EditorApplication.TargetScene;

			var debugUi = new DebugUI(this) { Scene = EditorView };
			debugUi.AdditionalInfo += (_, _) => {
				if(EditorView.CurrentCamera is Camera3D c3d) {
					var t3d = c3d.GetComponent<Transform, Transform3D>();
					
					ImGui.Text($"Camera position: {t3d.Position}");
					ImGui.Text($"Camera rotation: {t3d.Rotation}");
				}
			};
		#endregion

			var camera = new PerspectiveCamera(window) {
				Current = true
			};
			FreeCamera = new(new PerspectiveCamera(window), this, KeyBindings);
			
			// TODO this is quite messy with two different OnLoad methods
			EditorApplication.TargetScene.OnLoad((WindowBase) window);
			EditorView.OnLoad((WindowBase) window);
			OutputView.OnLoad((WindowBase) window);
		}

		public override void OnUnload() {
			base.OnUnload();
			EditorView.OnUnload();
			OutputView.OnUnload();
			EditorApplication.TargetScene.OnUnload();
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);
			EditorApplication.TargetScene.OnUpdate(delta);
			
			this.UpdateKeyBindings(KeyBindings);
		}

		public override void OnRender(float delta) {
			ImGuiManager.Begin(MainUI.Context);
			
			ImGui.PushID("editor");
			base.OnRender(delta);
			ImGui.PopID();
			
			ImGui.PushID("editor-view");
			EditorView.OnRender(delta);
			ImGui.PopID();
			
			ImGui.PushID("output-view");
			OutputView.OnRender(delta);
			ImGui.PopID();
			
			Window.Framebuffer.Bind();
			
			ImGuiManager.End(MainUI.Context);
		}
	}
}