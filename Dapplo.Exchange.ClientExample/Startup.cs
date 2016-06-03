using Dapplo.CaliburnMicro;
using Dapplo.LogFacade;
using Dapplo.LogFacade.Loggers;
using System;
using System.Diagnostics;
using System.Windows;

namespace Dapplo.Exchange.ClientExample
{
	/// <summary>
	/// This takes care or starting the Application
	/// </summary>
	public class Startup
	{
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
			var application = new Dapplication("Dapplo.Exchange.ExampleClient", "05ffc82c-f7cd-45d3-831d-867660a231ff");
			application.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			application.Add(@".", "Dapplo.CaliburnMicro*.dll");
			application.Add(typeof(Startup).Assembly);
			application.Run();
		}
	}
}
