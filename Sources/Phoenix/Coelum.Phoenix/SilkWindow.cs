using Coelum.Configuration;
using Coelum.Debug;
using Coelum.Common.Graphics;
using Coelum.Phoenix.GLFW;
using Coelum.Phoenix.OpenGL;
using Serilog;
using Silk.NET.Core;
using Silk.NET.Core.Contexts;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Monitor = Silk.NET.Windowing.Monitor;

namespace Coelum.Phoenix {
	
	public sealed class SilkWindow : WindowBase {

		private static IGLContext? _sharedContext;
		
		public IWindow SilkImpl { get; }
		public IInputContext Input { get; private set; }

		public Framebuffer Framebuffer { get; private set; }

		static SilkWindow() {
			if(ExperimentalFlags.ForceSDL) {
				Window.PrioritizeSdl();
			} else {
				Window.PrioritizeGlfw();
			}
			
			if(OperatingSystem.IsLinux() && !ExperimentalFlags.ForceWayland) {
				//			  GLFW_PLATFORM			 GLFW_PLATFORM_X11
				GlFw.InitHint((InitHint) 0x00050003, 0x00060004);

				if(!GlFw.Init()) {
					throw new PlatformException("Could not initialize GLFW");
				}
			}
		}

		public SilkWindow(IWindow impl) {
			SilkImpl = impl;
			
			SilkImpl.Load += () => {
				if(_sharedContext == null) {
					_ = new GlobalOpenGL(SilkImpl.CreateOpenGL());
					_sharedContext = SilkImpl.GLContext;
					
					if(Debugging.Enabled) {
						GLManager.EnableDebugOutput();
					}
				}
				
				SilkImpl.MakeCurrent();
				Gl.Viewport(SilkImpl.FramebufferSize);

				Framebuffer = new(this);

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

				Scene?.OnRender((float) delta);
			};
			
			SilkImpl.FramebufferResize += size => {
				SilkImpl.MakeCurrent();
				Gl.Viewport(size);
			};
			
			SilkImpl.Initialize();
		}

		public override bool Update() {
			SilkImpl.DoEvents();

			if(!SilkImpl.IsClosing) SilkImpl.DoUpdate();
			if(SilkImpl.IsClosing) {
				DoUpdates = false;
				Close();
				return false;
			}

			SilkImpl.DoRender();
			return true;
		}

		public override void Show() {
			SilkImpl.IsVisible = true;
		}

		public override void Hide() {
			SilkImpl.IsVisible = false;
		}

		public override void Close() {
			base.Close();

			if(DoUpdates) {
				SilkImpl.IsClosing = true;
			} else {
				SilkImpl.Close();
				SilkImpl.Dispose();
			}
		}

	#region Input additions
		public IReadOnlyList<IKeyboard>? GetKeyboards() => Input.Keyboards;
		public IReadOnlyList<IMouse>? GetMice() => Input.Mice;
		public IReadOnlyList<IGamepad>? GetGamepads() => Input.Gamepads;
		public IReadOnlyList<IJoystick>? GetJoysticks() => Input.Joysticks;
	#endregion

		public static SilkWindow Create(WindowOptions? options = null,
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

				if(defaults.Position == new Vector2D<int>(-1, -1)) {
					var monitorCenter = Monitor.GetMainMonitor(null).Bounds.Center;
					
					defaults.Position = new(
						monitorCenter.X - defaults.Size.X / 2,
						monitorCenter.Y - defaults.Size.Y / 2
					);
				}

				// if(defaults.Size == Vector2D<int>.Zero) {
				// 	defaults.Size = new(1280, 720);
				// }
			}
			
			if(_sharedContext != null) defaults.SharedContext = _sharedContext;
			if(debug) api.Flags |= ContextFlags.Debug;
			
			defaults.API = api;
			
			return new(Window.Create(defaults));
		}
	}
}