using Coelum.ECS;
using Coelum.Phoenix.Scene;
using ImGuiNET;

namespace Coelum.Phoenix.UI {
	
	public class DebugUI : ImGuiOverlay {
		
		public delegate void AdditionalInfoEventHandler(float delta, params dynamic[] args);
		
		public event AdditionalInfoEventHandler? AdditionalInfo;

		private const int ECS_SYSTEM_TIME_RESOLUTION = 512;

		private readonly PhoenixSceneBase _scene;
		private readonly Dictionary<EcsSystem, (float Max, float[] Values)> _ecsSystemTimes = new();

		public DebugUI(PhoenixSceneBase scene) : base(scene) {
			_scene = scene;
		}

		public override void OnRender(float delta, params dynamic[] args) {
			Controller.Update(delta);

			if(ImGui.Begin("Standard Debug UI", ImGuiWindowFlags.AlwaysAutoResize)) {
				float updMs = (_scene.Window?.UpdateDelta ?? 0) * 1000;
				float fxUpdMs = (_scene.Window?.FixedUpdateDelta ?? 0) * 1000;
				float rndMs = (_scene.Window?.RenderDelta ?? 0) * 1000;
				
				ImGui.Text($"Update delta: {updMs:F2}ms ({(1000 / updMs):F2} FPS)");
				ImGui.Text($"FixedUpdate delta: {fxUpdMs:F2}ms ({(1000 / fxUpdMs):F2} FPS)");
				ImGui.Text($"Render delta: {rndMs:F2}ms ({(1000 / rndMs):F2})");
				
				AdditionalInfo?.Invoke(delta, args);
			}

			if(ImGui.Begin("ECS Debug", ImGuiWindowFlags.AlwaysAutoResize)) {
				_scene.QuerySystems()
				      .Each((phase, systems) => {
					      ImGui.Text($"Phase: {phase}");
					      foreach(var system in systems) {
						      if(!_ecsSystemTimes.ContainsKey(system)) {
							      _ecsSystemTimes[system] = (0, new float[ECS_SYSTEM_TIME_RESOLUTION]);
							      return;
						      }

						      var times = _ecsSystemTimes[system];
						      times.Max = 0;
						      
						      for(int i = 1; i < ECS_SYSTEM_TIME_RESOLUTION; i++) {
							      times.Values[i - 1] = times.Values[i];

							      if(times.Values[i] > times.Max) {
								      times.Max = times.Values[i];
							      }
						      }

						      float timeUs = (float) system.ExecutionTime.TotalSeconds * 1_000_000;
						      times.Values[ECS_SYSTEM_TIME_RESOLUTION - 1] = timeUs;

						      _ecsSystemTimes[system] = times;
						      
						      ImGui.PlotHistogram($"{system.Name}: {timeUs:F2}us",
						                          ref _ecsSystemTimes[system].Values[0], ECS_SYSTEM_TIME_RESOLUTION, 0, 
						                          "", 0, times.Max, new(500, 50));
					      }
					      ImGui.Separator();
				      })
				      .Execute();
			}
		
			Controller.Render();
		}
	}
}