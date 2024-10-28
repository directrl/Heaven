using System.Drawing;
using System.Numerics;
using Coelum.Debug;
using Coelum.Resources;
using Coelum.Phoenix;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.Node;
using Coelum.Phoenix.OpenGL;
using Coelum.Phoenix.Scene;
using Coelum.Phoenix.Texture;
using Coelum.Common.Input;
using Coelum.Phoenix.Input;
using Coelum.Phoenix.ModelLoading;
using Coelum.Phoenix.UI;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;

namespace PhoenixPlayground.Scenes {
	
	public class ModelTest : Scene3D {

		private Model? _model;
		private Mesh? _mesh;
		private Model? _model2;
		private Node3D? _node;

		private DebugUI? _debugOverlay;

		private KeyBindings _keyBindings;
		private FreeCamera _freeCamera;

		public ModelTest() : base("model-test") {
			_keyBindings = new(Id);
			_freeCamera = new(_keyBindings);
			
			this.SetupKeyBindings(_keyBindings);
			
			ShaderOverlays.AddRange(Material.OVERLAYS);
			// ShaderOverlays.AddRange(TextureArray.OVERLAYS);
			// ShaderOverlays.AddRange(InstancedNode<Node3D>.OVERLAYS);
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);

			if(Camera == null) {
				Camera = new PerspectiveCamera(window) {
					FOV = 60,
					Position = new(0, 0, 0)
				};
			}

			if(_model == null) {
				_model = ModelLoader.Load(Playground.AppResources[ResourceType.MODEL, "untitled.glb"]);
				_node = new() {
					Model = _model
				};
				AddChild(_node);
			}

			_debugOverlay = new(this);
			_debugOverlay.AdditionalInfo += (_, _) => {
				if(ImGui.Begin("Camera info")) {
					ImGui.Text($"Camera position: {Camera?.Position.ToString() ?? "Unknown"}");
					ImGui.Text($"Camera pitch: {Camera?.Pitch.ToString() ?? "Unknown"}");
					ImGui.Text($"Camera yaw: {Camera?.Yaw.ToString() ?? "Unknown"}");
					ImGui.End();
				}
			};

			window.GetMice()[0].MouseMove += (mouse, pos) => {
				_freeCamera.CameraMove(Camera, pos);
			};
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			var mouse = Window.Input.Mice[0];
			_freeCamera.Update(Camera, ref mouse, delta);
			
			this.UpdateKeyBindings(_keyBindings);
		}

		public override void OnRender(float delta) {
			base.OnRender(delta);
		}
	}
}