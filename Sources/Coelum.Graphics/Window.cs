using Coelum.Configuration;
using Coelum.Debug;
using Coelum.Graphics.OpenGL;
using Coelum.Graphics.Scene;
using Silk.NET.Core.Contexts;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Coelum.Graphics {
	
	public sealed class Window : IDisposable {

		private static IGLContext? _sharedContext;
		
		public IWindow SilkImpl { get; }
		
		public IInputContext? Input { get; private set; }
		
		public float UpdateDelta { get; private set; }
		public float FixedUpdateDelta { get; private set; }
		public float RenderDelta { get; private set; }

		public bool DoUpdates { get; set; } = true;

		private SceneBase? _scene;
		public SceneBase? Scene {
			get => _scene;
			set {
				_scene?.OnUnload();
				_scene = value;

				if(value != null) {
					value?.OnLoad(this);
				}
			}
		}

		public Window(IWindow impl) {
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
				Gl.Viewport(size);
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
			DoUpdates = false;
			SilkImpl.Close();
		}

		public void Dispose() {
			Close();
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

				if(_sharedContext != null) {
					defaults.SharedContext = _sharedContext;
				}
			}

			if(debug) api.Flags |= ContextFlags.Debug;
			
			defaults.API = api;
			
			return new(Silk.NET.Windowing.Window.Create(defaults));
		}
	}
}