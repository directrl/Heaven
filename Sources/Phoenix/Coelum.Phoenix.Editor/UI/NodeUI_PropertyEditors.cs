using System.Drawing;
using System.Numerics;
using System.Reflection;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.ModelLoading;
using Coelum.Resources;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.Editor.UI {
	
	public unsafe partial class NodeUI {

		public static float DragWidth { get; set; } = 100;
		public static float DragSpeed { get; set; } = 0.25f;
		
		// TODO why do cameras reset when selecting them for the first time?
		private static readonly Dictionary<Type, Func<Type, string, object?, object?>> _PROPERTY_EDITORS = new() {
			{ typeof(Vector3), (type, property, o) => {
				var value = o is null ? new() : (Vector3) o;

				if(property == "Rotation") {
					ImGui.Text(property);
					ImGui.SameLine();
					ImGui.SetNextItemWidth(DragWidth);
					ImGui.SliderAngle("X", ref value.X, -180, 180);
					ImGui.SameLine();
					ImGui.SetNextItemWidth(DragWidth);
					ImGui.SliderAngle("Y", ref value.Y, -180, 180);
					ImGui.SameLine();
					ImGui.SetNextItemWidth(DragWidth);
					ImGui.SliderAngle("Z", ref value.Z, -180, 180);
				} else {
					ImGui.Text(property);
					ImGui.SameLine();
					ImGui.SetNextItemWidth(DragWidth);
					ImGui.DragFloat("X", ref value.X, DragSpeed);
					ImGui.SameLine();
					ImGui.SetNextItemWidth(DragWidth);
					ImGui.DragFloat("Y", ref value.Y, DragSpeed);
					ImGui.SameLine();
					ImGui.SetNextItemWidth(DragWidth);
					ImGui.DragFloat("Z", ref value.Z, DragSpeed);
				}

				return value;
			} },
			{ typeof(Vector2), (type, property, o) => {
				var value = o is null ? new() : (Vector2) o;
				
				ImGui.Text(property);
				ImGui.SameLine();
				ImGui.SetNextItemWidth(DragWidth * 1.5f);
				ImGui.DragFloat("X", ref value.X, DragSpeed);
				ImGui.SameLine();
				ImGui.SetNextItemWidth(DragWidth * 1.5f);
				ImGui.DragFloat("Y", ref value.Y, DragSpeed);

				return value;
			} },
			{ typeof(float), (type, property, o) => {
				var value = o is null ? 0.0f : (float) o;

				ImGui.SetNextItemWidth(DragWidth * 2);
				
				if(property == "Roll" || property == "Yaw" || property == "Pitch") {
					value = value.ToRadians();
					ImGui.SliderAngle(property, ref value, -180, 180);
					value = value.ToDegrees();
				} else {
					ImGui.DragFloat(property, ref value, DragSpeed);
				}
				
				return value;
			} },
			{ typeof(int), (type, property, o) => {
				var value = o is null ? 0 : (int) o;
				ImGui.SetNextItemWidth(DragWidth * 2);
				ImGui.DragInt(property, ref value);
				return value;
			} },
			{ typeof(bool), (type, property, o) => {
				var value = o is null ? false : (bool) o;
				ImGui.Checkbox(property, ref value);
				return value;
			} },
			{ typeof(string), (type, property, o) => {
				var value = o is null ? "" : (string) o;
				
				// TODO does bufSize matter?
				ImGui.SetNextItemWidth(DragWidth * 4);
				if(ImGui.InputText(property, ref value, 1024, ImGuiInputTextFlags.EnterReturnsTrue)) {
					return value;
				}
				
				return null;
			} },
			{ typeof(Model), (type, property, o) => {
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
			} },
			{ typeof(Color), (type, property, o) => {
				var value = o is null ? Color.Black : (Color) o;
				var valueVec4 = value.ToVector4();

				float[] color = new float[] { valueVec4.X, valueVec4.Y, valueVec4.Z, valueVec4.W };

				fixed(float* ptr = &color[0]) {
					ImGui.SetNextItemWidth(DragWidth * 2);
					ImGui.ColorPicker4(property, ptr);
				}
				
				value = Color.FromArgb(
					(int) Math.Floor(color[3] * 255),
					(int) Math.Floor(color[0] * 255),
					(int) Math.Floor(color[1] * 255),
					(int) Math.Floor(color[2] * 255)
				);
				return value;
			} },
			{ typeof(Node), (type, property, o) => {
				var value = (Node?) o;
				string name = value is null ? "null" : value.Name;

				var ns = EditorApplication.MainScene.NodeSelector;

				if(ImGui.Button(property + ": " + name)) {
					ns.Prompt(restrictType: type);
				}
				
				if(ns.Result is not null) {
					value = ns.Result;
					ns.Reset();
				}

				return value;
			} }
		};
	}
}