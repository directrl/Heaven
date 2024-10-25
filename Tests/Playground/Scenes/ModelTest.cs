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

namespace Playground.Scenes {
	
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
				_mesh = new Mesh(PrimitiveType.Triangles,
				                 new float[] {
					                 // VO
					                 -0.5f, 0.5f, 0.5f,
					                 // V1
					                 -0.5f, -0.5f, 0.5f,
					                 // V2
					                 0.5f, -0.5f, 0.5f,
					                 // V3
					                 0.5f, 0.5f, 0.5f,
					                 // V4
					                 -0.5f, 0.5f, -0.5f,
					                 // V5
					                 0.5f, 0.5f, -0.5f,
					                 // V6
					                 -0.5f, -0.5f, -0.5f,
					                 // V7
					                 0.5f, -0.5f, -0.5f,
				                 },
				                 new uint[] {
					                 // Front face
					                 0, 1, 3, 3, 1, 2,
					                 // Top Face
					                 4, 0, 3, 5, 4, 3,
					                 // Right face
					                 3, 2, 7, 5, 3, 7,
					                 // Left face
					                 6, 1, 0, 6, 0, 4,
					                 // Bottom face
					                 2, 1, 6, 2, 6, 7,
					                 // Back face
					                 7, 6, 4, 7, 4, 5,
				                 },
				                 null,
				                 null
				) {
					Material = new() {
						// Textures = new() {
						// 	(Material.TextureType.Diffuse, Texture2D.DefaultTexture)
						// }
					}
				};
				_model2 = new("test", new[] { _mesh });
				
				_model = ModelLoader.Load(Playground.AppResources[ResourceType.MODEL, "backpack/backpack.obj"]);
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

		public override void OnUnload() {
			base.OnUnload();
			
			_keyBindings.Dispose();
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			var mouse = Window.Input.Mice[0];
			_freeCamera.Update(Camera, ref mouse, delta);
			
			this.UpdateKeyBindings(_keyBindings);
		}

		public override void OnRender(float delta) {
			base.OnRender(delta);
			GlobalOpenGL.Gl.Disable(EnableCap.CullFace);
			
			//_model?.Render(PrimaryShader);
			//_model2.Load(PrimaryShader);
			//_model2?.Render(PrimaryShader);
		}
	}
}