using Coelum.Configuration;
using Coelum.Debug;
using Coelum.Common.Graphics;
using Coelum.Phoenix.Scene;
using Coelum.Phoenix.OpenGL;
using Silk.NET.Core;
using Silk.NET.Core.Contexts;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Coelum.Phoenix {
	
	public sealed class SilkWindow : WindowBase {

		private static IGLContext? _sharedContext;
		
		public IWindow SilkImpl { get; }
		public IInputContext? Input { get; private set; }

		static SilkWindow() {
			if(ExperimentalFlags.ForceSDL) {
				Window.PrioritizeSdl();
			} else {
				Window.PrioritizeGlfw();
			}
			
			if(OperatingSystem.IsLinux() && !ExperimentalFlags.ForceWayland) {
				var glfw = Glfw.GetApi();
				
				//			  GLFW_PLATFORM			 GLFW_PLATFORM_X11
				glfw.InitHint((InitHint) 0x00050003, 0x00060004);

				if(!glfw.Init()) {
					throw new PlatformException("Could not initialize GLFW");
				}
			}
		}

		public SilkWindow(IWindow impl) {
			SilkImpl = impl;
			
			SilkImpl.Load += () => {
				if(_sharedContext == null) {
					new GlobalOpenGL(SilkImpl.CreateOpenGL());
					_sharedContext = SilkImpl.GLContext;
				}
				
				SilkImpl.MakeCurrent();
				Gl.Viewport(SilkImpl.FramebufferSize);
				
				if(Debugging.Enabled) {
					GLManager.EnableDebugOutput();
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

				GLManager.SetDefaults();
				
				Gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
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
			SilkImpl.Close();
			SilkImpl.Dispose();
		}

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

				if(_sharedContext != null) {
					defaults.SharedContext = _sharedContext;
				}
			}

			if(debug) api.Flags |= ContextFlags.Debug;
			
			defaults.API = api;
			
			return new(Window.Create(defaults));
		}
	}
}