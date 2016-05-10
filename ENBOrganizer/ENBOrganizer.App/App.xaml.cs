﻿using System;
using System.Windows;

namespace ENBOrganizer.App
{
    // TODO: indeterminate progress bar on footer
    // TODO: tooltips for everything
    // TODO: double-check binding modes
    // TODO: title bar toggle buttons
    // TODO: ContextMenu on TreeViewItems only works when right-clicking on text
    // TODO: remove unused icons
    // TODO: split PresetItems into its own view model?
    // TODO: double-check reponsibilties of services
    // TODO: optimize UI speed
    // TODO: settings
    // TODO: add support for binaries
    // TODO: Remove exception handling when it's just catching Exception and displaying a message. This could be done globally.
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
        }
    }
}
