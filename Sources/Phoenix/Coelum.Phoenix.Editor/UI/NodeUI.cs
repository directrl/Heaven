using System.Numerics;
using System.Reflection;
using Coelum.Debug;
using Coelum.ECS;
using Coelum.ECS.Extensions;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.Physics.ECS.Components;
using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;
using Serilog;

namespace Coelum.Phoenix.Editor.UI {
	
	// TODO tree view collapse
	public partial class NodeUI : ImGuiUI {
		
		private bool _openNodeChooser = false;

		public Node? SelectedNode { get; set; }
		
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

						#region Node reparenting
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
						#endregion
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
								var pType = type.GetCommonTypeForNodeUse(useDecimal: false);
								
								if(_PROPERTY_EDITORS.TryGetValue(pType, out var action)) {
									var newValue = action.Invoke(type, pName, pValue);
									if(newValue is not null) setValue.Invoke(newValue);
								} else {
									ImGui.Text($"{pName}: {pValue ?? "null"}");
								}
							}
							ImGui.PopID();
						}
					#endregion

					#region Node editors
						var nodeProperties = new List<string>();
						
						DrawPropertyEditor(typeof(string), "Name", SelectedNode.Name,
						                   v => SelectedNode.Name = (string) v);
						
						foreach(var field in SelectedNode.GetType().GetFields()) {
							if(!field.IsForPublicNodeUse()) continue;
							
							var fValue = field.GetValue(SelectedNode);
							DrawPropertyEditor(field.FieldType, field.Name, fValue,
							                   v => field.SetValue(SelectedNode, v));
							
							nodeProperties.Add(field.Name);
						}

						foreach(var property in SelectedNode.GetType().GetProperties()) {
							if(!property.IsForPublicNodeUse()) continue;

							var pValue = property.GetValue(SelectedNode);
							DrawPropertyEditor(property.PropertyType, property.Name, pValue,
							                   v => property.SetValue(SelectedNode, v));
							
							nodeProperties.Add(property.Name);
						}
					#endregion
						
						ImGui.SeparatorText("Components");

					#region Component editors
						var drawnComponents = new List<INodeComponent>();
						
						foreach(var (_, component) in SelectedNode.Components) {
							if(drawnComponents.Contains(component)) continue;
							
							var cType = component.GetType();
							
							ImGui.SetNextItemOpen(true, ImGuiCond.FirstUseEver);
							
							if(ImGui.TreeNode(cType.Name)) {
								foreach(var field in cType.GetFields()) {
									if(!field.IsForPublicNodeUse()) continue;
									if(nodeProperties.Contains(field.Name)) continue;
									
									var fValue = field.GetValue(component);
									DrawPropertyEditor(field.FieldType, field.Name, fValue,
									                   v => field.SetValue(component, v));
								}

								foreach(var property in cType.GetProperties()) {
									if(!property.IsForPublicNodeUse()) continue;
									if(nodeProperties.Contains(property.Name)) continue;

									var pValue = property.GetValue(component);
									DrawPropertyEditor(property.PropertyType, property.Name, pValue,
									                   v => property.SetValue(component, v));
								}
								
								drawnComponents.Add(component);
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