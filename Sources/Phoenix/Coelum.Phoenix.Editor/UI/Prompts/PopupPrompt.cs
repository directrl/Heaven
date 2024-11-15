using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.Editor.UI.Prompts {
	
	public abstract class PopupPrompt<TResult> : ImGuiUI {
		
		protected bool Open { get; set; } = false;
		
		public string Name { get; }
		public TResult? Result { get; protected set; }

		public PopupPrompt(PhoenixScene scene, string name) : base(scene) {
			Name = name;
		}

		public override void Render(float delta) {
			if(Open) {
				ImGui.OpenPopup(Name);
				Open = false;
			}
		}

		public virtual void Prompt() {
			Open = true;
			Result = default;
		}
	}
}