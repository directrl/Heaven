using System.Numerics;
using Coelum.Debug;
using Coelum.ECS;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.UI {
	
	public class DebugOverlay : ImGuiUI {

	#region Delegates
		public delegate void AdditionalInfoEventHandler(float delta);
	#endregion

	#region Events
		public event AdditionalInfoEventHandler? AdditionalInfo;
	#endregion
		
		private const int ECS_SYSTEM_TIME_RESOLUTION = 512;

		private readonly Dictionary<EcsSystem, (float MaxTime, float[] Times)> _ecsSystems = new();
		
		public PhoenixScene TargetScene { get; init; }

		public DebugOverlay(PhoenixScene scene, PhoenixScene? targetScene = null) : base(scene) {
			targetScene ??= scene;
			TargetScene = targetScene;

			if(!Debugging.Enabled) Visible = false;
		}

		public override void Render(float delta) {
			ImGui.Begin("Standard Debug UI", ImGuiWindowFlags.AlwaysAutoResize);
			{
				ImGui.BeginTabBar("std");

				if(ImGui.BeginTabItem("Scene")) {
					ImGui.Text($"Children count: {TargetScene.ChildCount}");

					if(ImGui.Button("Reload scene")) {
						var window = TargetScene.Window;

						if(window != null) {
							window.Scene = null;
							TargetScene.PrimaryShader.Rebuild();
							window.Scene = Scene;
						}
					}
					
					ImGui.EndTabItem();
				}

				if(ImGui.BeginTabItem("Deltas")) {
					float rndMs = (TargetScene.Window?.RenderDelta ?? 0) * 1000;
					float updMs = (TargetScene.Window?.UpdateDelta ?? 0) * 1000;
					float fxUpdMs = (TargetScene.Window?.FixedUpdateDelta ?? 0) * 1000;
				
					ImGui.Text($"Render delta: {rndMs:F2}ms ({(1000 / rndMs):F2} FPS)");
					ImGui.Text($"Update delta: {updMs:F2}ms ({(1000 / updMs):F2} FPS)");
					ImGui.Text($"FixedUpdate delta: {fxUpdMs:F2}ms ({(1000 / fxUpdMs):F2} FPS)");
					ImGui.EndTabItem();
				}

				if(ImGui.BeginTabItem("Shaders")) {
					foreach((var overlay, bool enabled) in TargetScene.PrimaryShader.Overlays) {
						if(!overlay.HasCall) continue;
						
						// TODO why does this stop working after reloading the scene?
						if(ImGui.Button($"{overlay.Name} ({overlay.Type})")) {
							if(enabled) {
								TargetScene.PrimaryShader.DisableOverlays(overlay);
							} else {
								TargetScene.PrimaryShader.EnableOverlays(overlay);
							}
						}
						
						ImGui.SameLine();
						ImGui.Text(enabled ? "Enabled" : "Disabled");
					}
					
					ImGui.EndTabItem();
				}

				if(ImGui.BeginTabItem("Additional")) {
					AdditionalInfo?.Invoke(delta);
					ImGui.EndTabItem();
				}
				
				ImGui.EndTabBar();
			}
			ImGui.End();

			ImGui.Begin("ECS Debug", ImGuiWindowFlags.AlwaysAutoResize);
			{
				ImGui.BeginTabBar("ecs");

				if(ImGui.BeginTabItem("General")) {
					ImGui.Text($"Children count: {TargetScene.ChildCount}");

					ImGui.BeginChild("children", new Vector2(400, 300),
					                 ImGuiWindowFlags.AlwaysVerticalScrollbar
					                 | ImGuiWindowFlags.HorizontalScrollbar);
					{
						TargetScene.QueryChildren()
						      .Each(node => {
							      ImGui.Text($"{node.Id}: {node.Path} ({node})");
						      })
						      .Execute();
					}
					ImGui.EndChild();
					
					ImGui.EndTabItem();
				}

				foreach(var (phase, systems) in TargetScene.GetSystems()) {
					if(!ImGui.BeginTabItem(phase.ToString())) continue;
					      
					foreach(var system in systems) {
						ImGui.PushID(system.Name);
						      
						if(!_ecsSystems.ContainsKey(system)) {
							_ecsSystems[system] = (0, new float[ECS_SYSTEM_TIME_RESOLUTION]);
						}

						var debugData = _ecsSystems[system];
						debugData.MaxTime = 0;
						      
						for(int i = 1; i < ECS_SYSTEM_TIME_RESOLUTION; i++) {
							debugData.Times[i - 1] = debugData.Times[i];

							if(debugData.Times[i] > debugData.MaxTime) {
								debugData.MaxTime = debugData.Times[i];
							}
						}

						float timeUs = (float) system.ExecutionTime.TotalMilliseconds * 1_000;
						debugData.Times[ECS_SYSTEM_TIME_RESOLUTION - 1] = timeUs;
						
						_ecsSystems[system] = debugData;
						
						ImGui.SeparatorText($"{system.Name}: {(system.Enabled ? "On" : "Off")}");
						ImGui.SameLine();
						ImGui.Checkbox($"{phase}/{system.Name}", ref system.Enabled);

						string label
							= system is QuerySystem qs1
								? $"{timeUs:F2}us ({(qs1.SingleExecutionTime.TotalMilliseconds * 1_000):F2}us/Q - {qs1.QueryMatches}Q)"
								: $"{timeUs:F2}us";
						
						ImGui.PlotHistogram(label,
						                    ref _ecsSystems[system].Times[0], ECS_SYSTEM_TIME_RESOLUTION, 0, 
						                    "", 0, debugData.MaxTime, new Vector2(500, 50));
						
						if(!system.Enabled) {
							for(int i = 0; i <= 2; i++) {
								int step = (int) Math.Pow(10, i);
								      
								if(ImGui.Button($"Step {step}x")) {
									system.Step(step);
								}
								if(i != 2) ImGui.SameLine();
							}
						}
						      
						ImGui.PopID();
					}
					      
					ImGui.EndTabItem();
				}
				
				ImGui.EndTabBar();
			}
			ImGui.End();
		}
	}
}