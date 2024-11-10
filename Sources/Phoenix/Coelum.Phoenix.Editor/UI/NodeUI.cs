using System.Numerics;
using System.Reflection;
using Coelum.ECS;
using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;
using Serilog;

namespace Coelum.Phoenix.Editor.UI {
	
	public partial class NodeUI : ImGuiUI {

		private Node? _selectedNode;
		public Node? SelectedNode {
			get => _selectedNode;
			set {
				_selectedNode = value;
			}
		}
		
		public NodeUI(PhoenixScene scene) : base(scene) { }

		public override void Render(float delta) {
			var scene = EditorApplication.TargetScene;
			
			ImGui.ShowDemoWindow();

			ImGui.Begin("Node Editor");
			{
				ImGui.BeginChild("node selection list", new Vector2(150, 0),
					ImGuiChildFlags.Borders | ImGuiChildFlags.ResizeX);
				{
					void DrawNodeTree(Node node) {
						if(ImGui.Selectable(node.Name, SelectedNode == node)) {
							SelectedNode = node;
						}
					
						scene.QueryChildren(node, depth: 1)
						     .Each(child => {
							     ImGui.Indent();
							     DrawNodeTree(child);
							     ImGui.Unindent();
						     })
						     .Execute();
					}
				
					scene.QueryChildren(depth: 1)
					     .Each(DrawNodeTree)
					     .Execute();
				}
				ImGui.EndChild();

				ImGui.SameLine();

				ImGui.BeginChild("node edit");
				{
					if(SelectedNode is null) {
						ImGui.Text("No node selected");
					} else {
						ImGui.BeginChild(SelectedNode.Name, ImGuiWindowFlags.HorizontalScrollbar);
						ImGui.SeparatorText($"{SelectedNode.Name} ({SelectedNode.GetType()})");
						
						foreach(var (type, component) in SelectedNode.Components) {
							var cType = component.GetType();
							
							ImGui.SetNextItemOpen(true, ImGuiCond.FirstUseEver);
							if(ImGui.TreeNode("Component: " + cType.Name)) {
								foreach(var field in cType.GetFields()) {
									if(field.IsInitOnly) continue;
									
									var fValue = field.GetValue(component);

									ImGui.PushID(field.Name);
									{
										if(fValue is null) {
											ImGui.Text($"{field.Name}: null");
										} else {
											if(_PROPERTY_EDITORS.TryGetValue(
												   fValue.GetType(), out var action)) {
												var newValue = action.Invoke(field.Name, fValue);
												field.SetValue(component, newValue);
											} else {
												ImGui.Text($"{field.Name}: {fValue}");
											}
										}
									}
									ImGui.PopID();
								}

								foreach(var property in cType.GetProperties()) {
									if(property.Name == "Owner") continue;
									// TODO should unsettable properties still be displayed?
									if(property.SetMethod == null
									   || property.SetMethod.IsPrivate
									   || property.SetMethod.IsAssembly) continue;

									var pValue = property.GetValue(component);

									ImGui.PushID(property.Name);
									{
										if(pValue is null) {
											ImGui.Text($"{property.Name}: null");
										} else {
											if(_PROPERTY_EDITORS.TryGetValue(
												   pValue.GetType(), out var action)) {
												var newValue = action.Invoke(property.Name, pValue);
												property.SetValue(component, newValue);
											} else {
												ImGui.Text($"{property.Name}: {pValue}");
											}
										}
									}
									ImGui.PopID();
								}

								ImGui.TreePop();
							}
						}
						
						ImGui.EndChild();
					}
				}
				ImGui.EndChild();
			}
			ImGui.End();
		}
	}
}