using System.Drawing;
using System.Numerics;
using Coelum.Common.Input;
using Coelum.ECS.Prefab;
using Coelum.Phoenix.Input;
using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;
using Silk.NET.GLFW;
using Silk.NET.Windowing.Glfw;
using static Coelum.Phoenix.GLFW.GlobalGLFW;

namespace Coelum.Phoenix.Editor.UI {
	
	public class PrefabUI : ImGuiScene {
		
		public PrefabManager Prefabs { get; }

		public PrefabUI() : base("editor-ui_prefabs") {
			Prefabs = new(EditorApplication.TargetScene, EditorApplication.TargetAssembly);
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);

			window.SilkImpl.Title = "Editor: Prefabs";
		}

		public override void RenderUI(float delta) {
			base.RenderUI(delta);
			
			if(ImGui.Begin("Prefabs")) {
				int i = 0;
				
				foreach((var name, var prefab) in Prefabs.Prefabs) {
					if(ImGui.Button(name)) {
						EditorApplication.TargetScene.Add(prefab.Create(this));
						i++;
					}
			
					if(i > 9) {
						i = 0;
						continue;
					}
						
					ImGui.SameLine();
				}
					
				ImGui.End();
			}
		}
	}
}