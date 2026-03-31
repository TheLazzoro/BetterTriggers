using BetterTriggers;
using BetterTriggers.WorldEdit.GameDataReader;
using System;
using System.Windows;

namespace Tests
{
    /// <summary>
    /// Every test class should inherit this class.
    /// Initializes all necessary data to run the tests.
    /// </summary>
    public abstract class TestBase
    {
        private static object _lock = new object();
        private static Application _app;

        public TestBase()
        {
            lock (_lock)
            {
                if (Application.Current == null)
                {
                    _app = new Application(); // Hack. Instantiates a new WPF application.
                    _app.DispatcherUnhandledException += app_DispatcherUnhandledException;
                }
                if (!Init.HasLoaded)
                {
                    WarcraftStorageReader.GameVersion = new Version(1, 36, 1);
                    Init.Initialize(true);
                }
            }
        }

        private void app_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            
        }
    }
}
