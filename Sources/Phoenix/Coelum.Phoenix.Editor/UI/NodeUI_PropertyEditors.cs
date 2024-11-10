using System.Numerics;
using System.Reflection;
using Coelum.Phoenix.ModelLoading;
using Coelum.Resources;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.Editor.UI {
	
	public partial class NodeUI {

		public static float DragWidth { get; set; } = 100;
		public static float DragSpeed { get; set; } = 0.25f;
		
		private static readonly Dictionary<Type, Func<string, object?, object?>> _PROPERTY_EDITORS = new() {
			{ typeof(Vector3), (property, o) => {
				var value = o is null ? new() : (Vector3) o;

				if(property == "Rotation") {
					ImGui.Text(property);
					ImGui.SameLine();
					ImGui.PushItemWidth(DragWidth);
					ImGui.SliderAngle("X", ref value.X, -180, 180);
					ImGui.SameLine();
					ImGui.PushItemWidth(DragWidth);
					ImGui.SliderAngle("Y", ref value.Y, -180, 180);
					ImGui.SameLine();
					ImGui.PushItemWidth(DragWidth);
					ImGui.SliderAngle("Z", ref value.Z, -180, 180);
				} else {
					ImGui.Text(property);
					ImGui.SameLine();
					ImGui.PushItemWidth(DragWidth);
					ImGui.DragFloat("X", ref value.X, DragSpeed);
					ImGui.SameLine();
					ImGui.PushItemWidth(DragWidth);
					ImGui.DragFloat("Y", ref value.Y, DragSpeed);
					ImGui.SameLine();
					ImGui.PushItemWidth(DragWidth);
					ImGui.DragFloat("Z", ref value.Z, DragSpeed);
				}

				return value;
			} },
			{ typeof(Vector2), (property, o) => {
				var value = o is null ? new() : (Vector2) o;
				
				ImGui.Text(property);
				ImGui.SameLine();
				ImGui.PushItemWidth(DragWidth * 2);
				ImGui.DragFloat("X", ref value.X, DragSpeed);
				ImGui.SameLine();
				ImGui.PushItemWidth(DragWidth * 2);
				ImGui.DragFloat("Y", ref value.Y, DragSpeed);

				return value;
			} },
			{ typeof(float), (property, o) => {
				var value = o is null ? 0 : (float) o;

				if(property == "Roll" || property == "Yaw" || property == "Pitch") {
					ImGui.SliderAngle(property, ref value, -180, 180);
				} else {
					ImGui.DragFloat(property, ref value, DragSpeed);
				}
				
				return value;
			} },
			{ typeof(Model), (property, o) => {
				var value = (Model?) o;
				string name = value is null ? "None" : value.Name;
				if(string.IsNullOrWhiteSpace(name)) name = "Unknown";

				var rs = EditorApplication.MainScene.ResourceSelector;

				if(ImGui.Button(property + ": " + name)) {
					rs.Prompt(restrictType: ResourceType.MODEL);
				}

				if(rs.Result is not null) {
					value = ModelLoader.Load(rs.Result);
					rs.Reset();
				}

				return value;
			} }
		};
	}
}