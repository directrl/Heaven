using System.Numerics;
using System.Reflection;
using Coelum.Debug;
using Coelum.ECS;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;
using Serilog;

namespace Coelum.Phoenix.Editor.UI {
	
	// TODO node reparenting (dragging), tree view collapse
	public partial class NodeUI : ImGuiUI {

		private Node? _selectedNode;
		public Node? SelectedNode {
			get => _selectedNode;
			set {
				_selectedNode = value;
			}
		}

		private bool _openNodeChooser = false;
		
		public NodeUI(PhoenixScene scene) : base(scene) { }

		public unsafe override void Render(float delta) {
			var scene = EditorApplication.TargetScene;

			ImGui.Begin("Node Editor");
			{
				ImGui.BeginChild("node selection list", new Vector2(150, 0),
					ImGuiChildFlags.Borders | ImGuiChildFlags.ResizeX);
				{
					void DrawNodeTree(Node node) {
						if(node.Hidden) return;
						
						ImGui.PushID(node.Path);
						{
							if(ImGui.Selectable(node.Name, SelectedNode == node)) {
								SelectedNode = node;
							}
							
							if(ImGui.BeginDragDropSource()) {
								ImGui.SetDragDropPayload("NODE_DND", &node, (uint) sizeof(Node));
								ImGui.Text(node.Name);
								ImGui.EndDragDropSource();
							}

							if(ImGui.BeginDragDropTarget()) {
								ImGuiPayloadPtr payload;
								
								if((payload = ImGui.AcceptDragDropPayload("NODE_DND")).Handle is not null) {
									Tests.Assert(payload.DataSize == sizeof(Node));
									var nodePayload =  *((Node*) payload.Data);
									nodePayload.Parent = node;
								}
								
								ImGui.EndDragDropTarget();
							}
						}
						ImGui.PopID();
					
						scene.QueryChildren(node, depth: 1)
						     .Each(child => {
							     ImGui.Indent();
							     DrawNodeTree(child);
							     ImGui.Unindent();
						     })
						     .Execute();
					}
					
				#region Null (root) reparent
					ImGui.PushID("##root");
					{
						ImGui.BulletText("Root");
						
						if(ImGui.BeginDragDropTarget()) {
							ImGuiPayloadPtr payload;
								
							if((payload = ImGui.AcceptDragDropPayload("NODE_DND")).Handle is not null) {
								Tests.Assert(payload.DataSize == sizeof(Node));
								var nodePayload =  *((Node*) payload.Data);
								nodePayload.Parent = null;
							}
								
							ImGui.EndDragDropTarget();
						}
					}
					ImGui.PopID();
				#endregion
				
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
						ImGui.SeparatorText($"{SelectedNode.Name} ({SelectedNode.GetType().Name})");

					#region Helper methods
						void DrawPropertyEditor(Type type, string pName, object? pValue, Action<object> setValue) {
							ImGui.PushID(pName);
							{
								var pType = pValue.GetType();
								if(pType.IsAssignableTo(typeof(Node))) pType = typeof(Node);
								
								if(_PROPERTY_EDITORS.TryGetValue(pType, out var action)) {
									var newValue = action.Invoke(type, pName, pValue);
									if(newValue != null) setValue.Invoke(newValue);
								} else {
									ImGui.Text($"{pName}: {pValue}");
								}
							}
							ImGui.PopID();
						}

						bool IsFieldUsable(FieldInfo field) {
							if(field.DeclaringType == typeof(Node)) return false;
							if(field.IsInitOnly) return false;
							if(field.IsPrivate
							   || field.IsFamily
							   || field.IsAssembly) return false;

							return true;
						}

						bool IsPropertyUsable(PropertyInfo property) {
							if(property.DeclaringType == typeof(Node)) return false;
							if(property.Name == "Owner") return false;
							if(property.GetMethod == null
							   || property.GetMethod.IsPrivate
							   || property.GetMethod.IsFamily
							   || property.GetMethod.IsAssembly) return false;
							// TODO should unsettable properties still be displayed?
							if(property.SetMethod == null
							   || property.SetMethod.IsPrivate
							   || property.SetMethod.IsFamily
							   || property.SetMethod.IsAssembly) return false;

							return true;
						}
					#endregion

					#region Node editors
						DrawPropertyEditor(typeof(string), "Name", SelectedNode.Name,
						                   v => SelectedNode.Name = (string) v);
						
						foreach(var field in SelectedNode.GetType().GetFields()) {
							if(!IsFieldUsable(field)) continue;
							
							var fValue = field.GetValue(SelectedNode);
							DrawPropertyEditor(field.FieldType, field.Name, fValue,
							                   v => field.SetValue(SelectedNode, v));
						}

						foreach(var property in SelectedNode.GetType().GetProperties()) {
							if(!IsPropertyUsable(property)) continue;

							var pValue = property.GetValue(SelectedNode);
							DrawPropertyEditor(property.PropertyType, property.Name, pValue,
							                   v => property.SetValue(SelectedNode, v));
						}
					#endregion
						
						ImGui.SeparatorText("Components");

					#region Component editors
						foreach(var (_, component) in SelectedNode.Components) {
							var cType = component.GetType();
							
							ImGui.SetNextItemOpen(true, ImGuiCond.FirstUseEver);
							
							if(ImGui.TreeNode(cType.Name)) {
								foreach(var field in cType.GetFields()) {
									if(field.IsInitOnly) continue;
									
									var fValue = field.GetValue(component);
									DrawPropertyEditor(field.FieldType, field.Name, fValue,
									                   v => field.SetValue(component, v));
								}

								foreach(var property in cType.GetProperties()) {
									if(!IsPropertyUsable(property)) continue;

									var pValue = property.GetValue(component);
									DrawPropertyEditor(property.PropertyType, property.Name, pValue,
									                   v => property.SetValue(component, v));
								}
								
								ImGui.TreePop();
							}
						}
					#endregion
						
						ImGui.EndChild();
					}
				}
				ImGui.EndChild();
			}
			ImGui.End();

			if(_openNodeChooser) {
				ImGui.OpenPopup("Node chooser");
				_openNodeChooser = false;
			}

			if(ImGui.BeginPopupModal("Node chooser", ImGuiWindowFlags.AlwaysAutoResize)) {
				ImGui.EndPopup();
			}
		}
	}
}