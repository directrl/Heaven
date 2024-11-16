using System.Drawing;
using System.Numerics;
using Coelum.Common.Input;
using Coelum.ECS;
using Coelum.ECS.Prefab;
using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;
using Silk.NET.GLFW;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;
using static Coelum.Phoenix.GLFW.GlobalGLFW;

namespace Coelum.Phoenix.Editor.UI {
	
	public class PrefabUI : ImGuiUI {

		private Node? _newNode;
		private bool _openNodeSpawner = false;
		
		public PrefabManager PrefabManager { get; }
		public NodeManager NodeManager { get; }

		public PrefabUI(PhoenixScene scene) : base(scene) {
			PrefabManager = new(EditorApplication.TargetScene, EditorApplication.TargetAssembly);
			PrefabManager.AddAssembly(typeof(PrefabUI).Assembly);

			NodeManager = new(EditorApplication.TargetScene, EditorApplication.TargetAssembly);
			NodeManager.AddAssembly(typeof(PrefabUI).Assembly);
			NodeManager.AddAssembly(typeof(PhoenixScene).Assembly);
		}

		public override void Render(float delta) {
			ImGui.Begin("Prefabs");
			{
				ImGui.BeginTabBar("prefabs");

			#region Helper methods
				// nomination for worst snippet of code written in the 20th century
				void DrawPrefabManager(PrefabManager? pm, NodeManager? nm) {
					int i = 0;

					void ForeachInner(string name, Node node) {
						if(ImGui.Button(name)) {
							if(node is IPrefab prefab) {
								_newNode = prefab.Create(EditorApplication.TargetScene);
							} else {
								_newNode = NodeManager.Create(name);
							}
							
							_openNodeSpawner = true;
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
				
				if(ImGui.BeginTabItem("Prefabs")) {
					DrawPrefabManager(PrefabManager, null);
					ImGui.EndTabItem();
				}
				
				if(ImGui.BeginTabItem("Nodes")) {
					DrawPrefabManager(null, NodeManager);
					ImGui.EndTabItem();
				}
				
				ImGui.EndTabBar();
			}
			ImGui.End();

			if(_openNodeSpawner) {
				ImGui.OpenPopup("Node spawner");
				_openNodeSpawner = false;
			}
			
			if(ImGui.BeginPopupModal("Node spawner", ImGuiWindowFlags.AlwaysAutoResize)) {
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
					
					EditorApplication.TargetScene.Add(_newNode);
					_newNode = null;
					ImGui.CloseCurrentPopup();
				}
				
				ImGui.EndPopup();
			}
		}
	}
}