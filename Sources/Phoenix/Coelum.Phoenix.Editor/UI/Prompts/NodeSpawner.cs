using System.Numerics;
using Coelum.ECS;
using Coelum.ECS.Prefab;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.Editor.UI.Prompts {
	
	public class NodeSpawner : PopupPrompt<Node> {

		private Node? _parentNode;
		private Node? _newNode;
		
		public PrefabManager PrefabManager { get; }
		public NodeManager NodeManager { get; }

		public NodeSpawner(PhoenixScene scene) : base(scene, "Node Spawner") {
			PrefabManager = new(EditorApplication.TargetScene, EditorApplication.TargetAssembly);
			PrefabManager.AddAssembly(GetType().Assembly);
			PrefabManager.AddAssembly(typeof(PhoenixScene).Assembly);
			
			NodeManager = new(EditorApplication.TargetScene, EditorApplication.TargetAssembly);
			NodeManager.AddAssembly(GetType().Assembly);
			NodeManager.AddAssembly(typeof(PhoenixScene).Assembly);
		}

		public override void Render(float delta) {
			base.Render(delta);
			
			var scene = EditorApplication.TargetScene;
			bool nodeNamer = false;

			if(ImGui.BeginPopupModal(Name, ImGuiWindowFlags.AlwaysAutoResize)) {
			#region Helper methods
				// nomination for worst snippet of code written in the 20th century
				void DrawPrefabManager(PrefabManager? pm, NodeManager? nm) {
					int i = 0;

					void ForeachInner(string name, Node node) {
						if(ImGui.Button(name)) {
							if(node is IPrefab prefab) {
								_newNode = prefab.Create(scene);
							} else {
								_newNode = NodeManager.Create(name);
							}

							nodeNamer = true;
							ImGui.CloseCurrentPopup();
							i++;
						}
			
						if(i > 9) {
							i = 0;
							return;
						}
						
						ImGui.SameLine();
					}

					if(nm != null) {
						foreach((var name, var node) in nm.Nodes) {
							ForeachInner(name, node);
						}
					}
					
					if(pm != null) {
						foreach((var name, var prefab) in pm.Prefabs) {
							ForeachInner(name, (Node) prefab);
						}
					}
				}
			#endregion

				ImGui.BeginTabBar("node list");
				{
					if(ImGui.BeginTabItem("Prefabs")) {
						DrawPrefabManager(PrefabManager, null);
						ImGui.EndTabItem();
					}
				
					if(ImGui.BeginTabItem("Nodes")) {
						DrawPrefabManager(null, NodeManager);
						ImGui.EndTabItem();
					}
				}
				ImGui.EndTabBar();
				
				ImGui.EndPopup();
			}

			if(nodeNamer) ImGui.OpenPopup("Node Namer");
			
			if(ImGui.BeginPopupModal("Node Namer", ImGuiWindowFlags.AlwaysAutoResize)) {
				if(_newNode is null) {
					ImGui.CloseCurrentPopup();
					return;
				}
				
				string name = "";
				
				ImGui.SetKeyboardFocusHere();
				if(ImGui.InputTextWithHint("Name", "Leave blank for random",
				                           ref name, 1024, ImGuiInputTextFlags.EnterReturnsTrue)) {

					if(!string.IsNullOrWhiteSpace(name)) {
						_newNode.Name = name;
					}

					if(EditorApplication.MainScene.Camera is Camera3D c3d
					   && _newNode.TryGetComponent<Transform, Transform3D>(out var t3d)) {

						var cameraPos = c3d.GetComponent<Transform, Transform3D>().Position;
						
						// set global position to camera's position
						// if(_parentNode != null
						//    && _parentNode.TryGetComponent<Transform, Transform3D>(out var pt3d)) {
						//
						// 	Matrix4x4.Invert(t3d.GlobalMatrix, out var igm);
						// 	t3d.Position = Vector3.Transform(cameraPos, igm);
						// } else {
						// 	t3d.Position = cameraPos;
						// }
					}

					if(_parentNode is null) {
						scene.Add(_newNode);
					} else {
						_parentNode.Add(_newNode);
					}
					
					_newNode = null;
					ImGui.CloseCurrentPopup();
				}
				
				ImGui.EndPopup();
			}
		}

		public override void Prompt() {
			base.Prompt();
			_parentNode = null;
		}

		public void Prompt(Node? parent) {
			base.Prompt();
			_parentNode = parent;
		}
	}
}