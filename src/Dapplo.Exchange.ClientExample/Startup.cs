#region Dapplo 2016-2018 - GNU Lesser General Public License

// Dapplo - building blocks for .NET applications
// Copyright (C) 2016-2018 Dapplo
// 
// For more information see: http://dapplo.net/
// Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
// This file is part of Dapplo.Exchange
// 
// Dapplo.Exchange is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Dapplo.Exchange is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have a copy of the GNU Lesser General Public License
// along with Dapplo.Exchange. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

#endregion

#region Usings

using System;
using System.Diagnostics;
using System.Windows;
using Dapplo.Addons.Bootstrapper;
using Dapplo.Log;
using Dapplo.Log.Loggers;
using Dapplo.CaliburnMicro.Dapp;

#endregion

namespace Dapplo.Exchange.ClientExample
{
    /// <summary>
    ///     This takes care or starting the Application
    /// </summary>
    public static class Startup
    {
        /// <summary>
        ///     Start the application
        /// </summary>
        [STAThread, DebuggerNonUserCode]
        public static void Main()
        {
#if DEBUG
            // Initialize a debug logger for Dapplo packages
            LogSettings.RegisterDefaultLogger<DebugLogger>(LogLevels.Verbose);
#endif
            var applicationConfig = ApplicationConfigBuilder
                .Create()
                .WithApplicationName("Dapplo.Exchange.ExampleClient")
                .WithMutex("05ffc82c-f7cd-45d3-831d-867660a231ff")
                .WithConfigSupport()
                .WithCaliburnMicro()
                .BuildApplicationConfig();
            var application = new Dapplication(applicationConfig)
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown
            };
            if (application.WasAlreadyRunning)
            {

            }
            application.Run();
        }
    }
}