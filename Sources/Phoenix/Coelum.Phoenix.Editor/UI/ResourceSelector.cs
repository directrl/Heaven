using System.Numerics;
using System.Reflection;
using Coelum.Phoenix.UI;
using Coelum.Resources;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.Editor.UI {
	
	public class ResourceSelector : ImGuiUI {

		private Assembly _assembly;
		private ResourceType? _typeRestriction;
		
		public IResource? Result { get; private set; }

		public ResourceSelector(PhoenixScene scene, Assembly assembly) : base(scene) {
			_assembly = assembly;
		}

		public override void Render(float delta) {
			ImGui.SetNextWindowPos(
				new(Scene.Window.Framebuffer.Size.X / 2, Scene.Window.Framebuffer.Size.Y / 2),
				ImGuiCond.Always,
				new(0.5f, 0.5f)
			);
			
			ImGui.SetNextWindowSize(
				new(Scene.Window.Framebuffer.Size.X / 2, Scene.Window.Framebuffer.Size.Y / 2),
				ImGuiCond.Always
			);
			
			// TODO popup (modal)
			if(ImGui.Begin("Resource Selection",
				   ImGuiWindowFlags.NoCollapse
				   | ImGuiWindowFlags.NoMove
				   | ImGuiWindowFlags.NoResize
				   | ImGuiWindowFlags.HorizontalScrollbar
				   | ImGuiWindowFlags.NoDocking)) {

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
					ImGui.BeginChild(
						$"{type.Path} selection list",
						new Vector2(Scene.Window.Framebuffer.Size.X / 2 / resources.Count, 0),
						ImGuiChildFlags.Borders | ImGuiChildFlags.ResizeX);
					{
						ImGui.SeparatorText($"{type.Path} ({type.Extension})");
						foreach(var resource in resourceList) {
							if(_typeRestriction != null && type != _typeRestriction) {
								ImGui.TextDisabled(resource.Name);
								continue;
							}
							
							if(ImGui.Selectable(resource.Name)) {
								Result = resource;
								Visible = false;
							}
						}
					}
					ImGui.EndChild();
					
					ImGui.SameLine();
				}
				
				ImGui.End();
			}
		}

		public void Prompt(ResourceType? restrictType = null) {
			Result = null;
			_typeRestriction = restrictType;
			Visible = true;
		}

		public void Reset() {
			Result = null;
			_typeRestriction = null;
			Visible = false;
		}
	}
}