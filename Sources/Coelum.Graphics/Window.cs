using Coelum.Configuration;
using Coelum.Debug;
using Coelum.Graphics.OpenGL;
using Coelum.Graphics.Scene;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Coelum.Graphics {
	
	public sealed class Window : IDisposable {
		
		public IWindow SilkImpl { get; }
		public GL? GL { get; private set; }
		
		public IInputContext? Input { get; private set; }
		
		public float UpdateDelta { get; private set; }
		public float FixedUpdateDelta { get; private set; }
		public float RenderDelta { get; private set; }

		private SceneBase? _scene;
		public SceneBase? Scene {
			get => _scene;
			set {
				_scene?.OnUnload();
				_scene = value;

				if(value != null && GL != null) {
					var oldGl = GLManager.Current;
					GLManager.Current = GL;
					
					value?.OnLoad(this);

					GLManager.Current = oldGl;
				}
			}
		}

		public Window(IWindow impl) {
			SilkImpl = impl;
			
			SilkImpl.Load += () => {
				GL = SilkImpl.CreateOpenGL();
				GL.Viewport(SilkImpl.FramebufferSize);

				if(Debugging.Enabled) {
					GLManager.EnableDebugOutput(GL);
				}

				Input = SilkImpl.CreateInput();

				SilkImpl.IsVisible = true;
			};

			SilkImpl.Update += delta => {
				FixedUpdateDelta = (float) delta;
				Scene?.OnFixedUpdate((float) delta);
			};

			SilkImpl.Render += delta => {
				UpdateDelta = (float) delta;
				RenderDelta = (float) delta;
				
				Scene?.OnUpdate((float) delta);
				
				if(!SilkImpl.IsVisible) return;
				SilkImpl.MakeCurrent();

				if(GL != null) {
					GLManager.Current = GL;
					GLManager.SetDefaults();
					
					GL.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
					Scene?.OnRender(GL, (float) delta);
				}
			};

			SilkImpl.FramebufferResize += size => {
				GL?.Viewport(size);
			};
			
			SilkImpl.Initialize();
		}

		public void Show() {
			SilkImpl.IsVisible = true;
		}

		public void Hide() {
			SilkImpl.IsVisible = false;
		}

		public void Close() {
			Scene?.OnUnload();
			SilkImpl.Close();
		}

		public void Dispose() {
			Close();
			GL?.Dispose();
			SilkImpl.Dispose();
		}

		public static Window Create(WindowOptions? options = null,
		                            bool optimalDefaults = true,
		                            bool debug = false) {
			WindowOptions defaults;
			
			if(options == null) defaults = WindowOptions.Default;
			else defaults = options.Value;
			
			var api = defaults.API;

			if(optimalDefaults) {
				api.Flags |= ContextFlags.ForwardCompatible;

				defaults.VSync = EngineOptions.VerticalSync;
				defaults.UpdatesPerSecond = 60;
				defaults.IsVisible = false;
				defaults.TransparentFramebuffer = true;
				defaults.ShouldSwapAutomatically = true;
			}

			if(debug) api.Flags |= ContextFlags.Debug;
			
			defaults.API = api;
			
			return new(Silk.NET.Windowing.Window.Create(defaults));
		}
	}
}