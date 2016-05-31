using Dapplo.Addons;
using Dapplo.Addons.Bootstrapper;
using Dapplo.LogFacade;
using Dapplo.LogFacade.Loggers;
using Dapplo.Utils;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace Dapplo.Exchange.ClientExample
{
	/// <summary>
	/// This takes care or starting the Application
	/// </summary>
	public class Startup : Application
	{
		private readonly ApplicationBootstrapper _bootstrapper = new ApplicationBootstrapper("Dapplo.Exchange.ExampleClient", "05ffc82c-f7cd-45d3-831d-867660a231ff");

		/// <summary>
		/// Start the application
		/// </summary>
		[STAThread, DebuggerNonUserCode]
		public static void Main()
		{
#if DEBUG
			// Initialize a debug logger for Dapplo packages
			LogSettings.Logger = new DebugLogger { Level = LogLevel.Verbose };
#endif
			var application = new Startup();
			application.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			application.Run();
		}

		/// <summary>
		/// Make sure we startup everything after WPF instanciated
		/// </summary>
		/// <param name="startupEventArgs">StartupEventArgs</param>
		protected override async void OnStartup(StartupEventArgs startupEventArgs)
		{
			base.OnStartup(startupEventArgs);

			UiContext.Initialize();

			_bootstrapper.Add(@".", "Dapplo.CaliburnMicro*.dll");
			_bootstrapper.Add(GetType().Assembly);

			// TODO: Remove "hack" for the shutdown/exit
			await _bootstrapper.InitializeAsync();
			_bootstrapper.Export<IBootstrapper>(_bootstrapper);


			await _bootstrapper.RunAsync();

		}
	}
}
