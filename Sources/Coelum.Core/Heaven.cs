using System.Diagnostics;
using System.Reflection;
using Coelum.Configuration;
using Coelum.Debug;
using Coelum.Core.Logging;
using Coelum.Common.Graphics;
using Coelum.Resources;
using Serilog;
using Serilog.Core;

namespace Coelum.Core {
	
	public abstract class Heaven : IDisposable {
		
		public static Heaven CurrentApplication { get; private set; }
		
		internal static Logger EngineLogger { get; private set; }
		public static Logger? AppLogger { get; protected set; }
		
		public static ExternalResourceManager ExternalResources { get; private set; }
		internal static ResourceManager EngineResources { get; private set; }
		public static ResourceManager AppResources { get; protected set; }
		
		public static List<WindowBase> Windows { get; private set; }
		
		private List<Action> _futureActions;
		
		public string Id { get; private set; }
		public bool Running { get; protected set; }

		protected Heaven(string id) {
			_futureActions = new();

			CurrentApplication = this;
			
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

			// TODO ProcessExit doesn't get called for console applications :D
			AppDomain.CurrentDomain.ProcessExit += (_, _) => Stop();
		}

		public abstract void Setup(string[] args);

		public void Start(string[] args) {
			Debugging.FromArgs(args);
			ExperimentalFlags.FromArgs(args);
			EngineOptions.FromArgs(args);
			
		#region Engine logger
		#if DEBUG
			string? path = "";
		#else
			string? path = null;
		#endif
			
			var loggerConfig = LoggerFactory
				.CreateDefaultConfiugration(LoggerPurpose.Engine, path);

			if(path == "") {
				Console.WriteLine("Running in debug configuration; disabled logging to file");
			}
			
			if(Debugging.Enabled) loggerConfig.MinimumLevel.Debug();
			if(Debugging.Verbose) loggerConfig.MinimumLevel.Verbose();
			Log.Logger = EngineLogger = loggerConfig.CreateLogger();
		#endregion

		#region Application logger
			loggerConfig = LoggerFactory
				.CreateDefaultConfiugration(LoggerPurpose.Application, path);
			
			if(Debugging.Enabled) loggerConfig.MinimumLevel.Debug();
			if(Debugging.Verbose) loggerConfig.MinimumLevel.Verbose();
			AppLogger = loggerConfig.CreateLogger();
		#endregion
			
			Running = true;
			
			Setup(args);
			Tests.Assert(Windows.Count > 0);
			
			WindowBase? toRemove = null;
			
			while(Windows.Count > 0 && Windows[0].DoUpdates && Running) {
				foreach(var window in Windows) {
					if(!window.DoUpdates) continue;
					if(!window.Update()) toRemove = window;
				}

				if(toRemove != null) {
					Windows.Remove(toRemove);
					toRemove = null;
				}

				var a = new List<Action>(_futureActions);
				foreach(var action in a) {
					action.Invoke();
					_futureActions.Remove(action);
				}
			}

			Dispose();
		}

		public void Stop() {
			Running = false;
		}

		/// <summary>
		/// Schedules an action to run in the main application thread
		/// </summary>
		/// <param name="action">The action to run</param>
		public void RunLater(Action action) {
			_futureActions.Add(action);
		}

		public void Dispose() {
			GC.SuppressFinalize(this);

			Running = false;
			
			EngineLogger.Information("Shutting down");
			AppLogger?.Information("Shutting down");
			
			foreach(var window in Windows) {
				window.Close();
			}
		}
	}
}