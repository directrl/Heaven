using System.Numerics;
using System.Reflection;
using Coelum.Phoenix.UI;
using Coelum.Resources;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.Editor.UI.Prompts {
	
	public class ResourceSelector : PopupPrompt<IResource> {

		private readonly Assembly _assembly;
		private ResourceType? _typeRestriction;

		public ResourceSelector(PhoenixScene scene, Assembly assembly) : base(scene, "Resource Selector") {
			_assembly = assembly;
		}

		public override void Render(float delta) {
			base.Render(delta);
			
			ImGui.SetNextWindowSize(
				new(Scene.Window.Framebuffer.Size.X / 1.5f, Scene.Window.Framebuffer.Size.Y / 1.5f),
				ImGuiCond.FirstUseEver
			);
			
			ImGui.SetNextWindowPos(
				new(Scene.Window.Framebuffer.Size.X / 1.5f, Scene.Window.Framebuffer.Size.Y / 1.5f),
				ImGuiCond.FirstUseEver,
				new(0.5f, 0.5f) // TODO why does neither (0.5f, 0.5f) nor (0.75f, 0.75f) center properly?
			);
			
			if(ImGui.BeginPopupModal(Name)) {
				var wSize = ImGui.GetWindowSize();
				
				var resources = new Dictionary<ResourceType, List<IResource>>() {
					{ ResourceType.TEXTURE, new() },
					{ ResourceType.MODEL, new() },
					{ ResourceType.SHADER, new() },
					{ ResourceType.CUSTOM, new() },
				};
				
				foreach(var resourcePath in _assembly.GetManifestResourceNames()) {
					var resourceType = ResourceType.MatchPath(resourcePath);
					var resourceName = resourcePath.Replace(_assembly.GetName().Name + ".Resources.", "");
					resourceName = resourceName.Replace(resourceType.Path + ".", "");

					if(!string.IsNullOrWhiteSpace(resourceType.Extension)) {
						resourceName = resourceName.Replace(resourceType.Extension, "");
					}

					var resource = EditorApplication.AppResources[resourceType, resourceName];
					resources[resourceType].Add(resource);
				}

				foreach(var (type, resourceList) in resources) {
					ImGui.SameLine();
					
					ImGui.BeginChild(
						$"{type.Path} selection list",
						// TODO which style property is this?
						//             \/   \/
						new Vector2(
							(wSize.X - 32 - 16 - ImGui.GetStyle().WindowPadding.X) / resources.Count,
							wSize.Y - 32 - ImGui.GetStyle().WindowPadding.X - ImGui.GetFrameHeightWithSpacing()
						),
						ImGuiChildFlags.Borders
					);
					{
						ImGui.SeparatorText($"{type.Path} ({type.Extension})");
						foreach(var resource in resourceList) {
							if(_typeRestriction != null && type != _typeRestriction) {
								ImGui.TextDisabled(resource.Name);
								continue;
							}
							
							if(ImGui.Selectable(resource.Name)) {
								Result = resource;
								ImGui.CloseCurrentPopup();
							}
						}
					}
					ImGui.EndChild();
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

		public void Prompt(ResourceType? restrictType) {
			base.Prompt();
			_typeRestriction = restrictType;
		}
	}
}