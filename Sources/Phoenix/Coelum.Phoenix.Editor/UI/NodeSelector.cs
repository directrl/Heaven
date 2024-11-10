using System.Numerics;
using System.Reflection;
using Coelum.Debug;
using Coelum.ECS;
using Coelum.Phoenix.UI;
using Coelum.Resources;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.Editor.UI {
	
	public class NodeSelector : ImGuiUI {

		private Type? _typeRestriction;
		private bool _open = false;
		
		public Node? Result { get; private set; }

		public NodeSelector(PhoenixScene scene) : base(scene) { }

		public override void Render(float delta) {
			var scene = EditorApplication.TargetScene;
			
			if(_open) {
				ImGui.OpenPopup("Node Selector");
				_open = false;
			}
			
			if(ImGui.BeginPopupModal("Node Selector", ImGuiWindowFlags.AlwaysAutoResize)) {
				if(_typeRestriction is not null) {
					ImGui.Text($"Of type: {_typeRestriction.Name}");
				}

				var items = new List<string>();

				int i = 0;
				int selectedIndex = -1;

				if(ImGui.BeginListBox("Nodes")) {
					scene.QueryChildren()
		                 .Each(node => {
			                 if(Result is not null) return;
			                 
			                 if(_typeRestriction is null ||
			                    (_typeRestriction is not null && node.GetType().IsAssignableTo(_typeRestriction))) {

				                 bool selected = (selectedIndex == i);

				                 if(ImGui.Selectable(node.Path, ref selected)) {
					                 selectedIndex = i;
				                 }

				                 if(selected) {
					                 Result = node;
					                 ImGui.CloseCurrentPopup();
				                 }
			                 }
			                 
			                 i++;
		                 })
		                 .Execute();
					
					ImGui.EndListBox();
				}

				if(ImGui.Button("Cancel")) {
					Result = null;
					ImGui.CloseCurrentPopup();
				}
				
				ImGui.EndPopup();
			}
		}

		public void Prompt(Type? restrictType = null) {
			Result = null;
			_typeRestriction = restrictType;
			_open = true;
		}

		public void Reset() {
			Result = null;
			_typeRestriction = null;
		}
	}
}