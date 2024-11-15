using Coelum.ECS;
using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.Editor.UI.Prompts {
	
	public class NodeSelector : PopupPrompt<Node> {

		private Type? _typeRestriction;

		public NodeSelector(PhoenixScene scene) : base(scene, "Node Selector") { }

		public override void Render(float delta) {
			base.Render(delta);
			
			var scene = EditorApplication.TargetScene;
			
			if(ImGui.BeginPopupModal(Name, ImGuiWindowFlags.AlwaysAutoResize)) {
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

		public override void Prompt() {
			base.Prompt();
			_typeRestriction = null;
		}

		public void Prompt(Type? restrictType) {
			base.Prompt();
			_typeRestriction = restrictType;
		}
	}
}