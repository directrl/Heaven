using System.Diagnostics;
using System.Reflection;
using Coeli.Configuration;
using Coeli.Core.Logging;
using Coeli.Debug;
using Coeli.Graphics;
using Coeli.Graphics.OpenGL;
using Coeli.Resources;
using Serilog;
using Serilog.Core;
using Silk.NET.Core;
using Silk.NET.GLFW;

namespace Coeli.Core {
	
	public abstract class Heaven : IDisposable {
		
		public static string Id { get; private set; }
		public static bool Running { get; protected set; }
		
		internal static Logger EngineLogger { get; private set; }
		public static Logger? AppLogger { get; protected set; }
		
		public static ExternalResourceManager ExternalResources { get; private set; }
		internal static ResourceManager EngineResources { get; private set; }
		public static ResourceManager AppResources { get; protected set; }
		
		public static List<Window> Windows { get; private set; }

		protected Heaven(string id) {
			Id = id;
			Windows = new();
			
			Directories.Resolve(id);

			ExternalResources = new();

			var eNamespace = Assembly.GetExecutingAssembly().GetName().Name + ".Resources";
			var aNamespace = Assembly.GetCallingAssembly().GetName().Name + ".Resources";
			
			if(string.IsNullOrEmpty(eNamespace)) Console.WriteLine("Could not get the engine namespace; " +
				"resources might not load properly");
			if(string.IsNullOrEmpty(aNamespace)) Console.WriteLine("Could not get the application namespace; " +
				"resources might not load properly");
			
			EngineResources = new(eNamespace ?? "");
			AppResources = new(aNamespace ?? "", Assembly.GetCallingAssembly());
		}

		public abstract void Setup();

		public void Start(string[] args) {
			Debugging.FromArgs(args);
			ExperimentalFlags.FromArgs(args);
			EngineOptions.FromArgs(args);
			
		#region Engine logger
			var loggerConfig = LoggerFactory
				.CreateDefaultConfiugration(LoggerPurpose.Engine,
					Debugging.Enabled ? "" : null);

			if(Debugging.Enabled) {
				Console.WriteLine("Debugging enabled; disabled logging to file");
				loggerConfig.MinimumLevel.Debug();
			}
			
			if(Debugging.Verbose) loggerConfig.MinimumLevel.Verbose();
			Log.Logger = EngineLogger = loggerConfig.CreateLogger();
		#endregion

		#region Application logger
			loggerConfig = LoggerFactory
				.CreateDefaultConfiugration(LoggerPurpose.Application,
					Debugging.Enabled ? "" : null);
			
			if(Debugging.Enabled) loggerConfig.MinimumLevel.Debug();
			if(Debugging.Verbose) loggerConfig.MinimumLevel.Verbose();
			AppLogger = loggerConfig.CreateLogger();
		#endregion

		#region Windowing platform setup
			if(ExperimentalFlags.ForceSDL) {
				Silk.NET.Windowing.Window.PrioritizeSdl();
			} else {
				Silk.NET.Windowing.Window.PrioritizeGlfw();
			}
			
			if(OperatingSystem.IsLinux() && !ExperimentalFlags.ForceWayland) {
				var glfw = Glfw.GetApi();
				
				//			  GLFW_PLATFORM			 GLFW_PLATFORM_X11
				glfw.InitHint((InitHint) 0x00050003, 0x00060004);

				if(!glfw.Init()) {
					throw new PlatformException("Could not initialize GLFW");
				}
			}
		#endregion
			
			Running = true;
			
			Setup();
			Tests.Assert(Windows.Count > 0);
			
			var primaryWindow = Windows[0];
			while(!primaryWindow.SilkImpl.IsClosing && Running) {
				foreach(var window in Windows) {
					window.SilkImpl.DoEvents();
					
					if(!window.SilkImpl.IsClosing) window.SilkImpl.DoUpdate();
					if(window.SilkImpl.IsClosing) continue;
					
					window.SilkImpl.DoRender();
				}
			}
			
			Stop();
		}

		public void Stop() {
			EngineLogger.Information("Shutting down");
			AppLogger?.Information("Shutting down");
			
			Running = false;
			
			foreach(var window in Windows) {
				window.Dispose();
			}
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
			Stop();
		}
	}
}