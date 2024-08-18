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
        public TestBase()
        {
            if(Application.Current == null)
            {
                new Application(); // Hack. Instantiates a new WPF application.
            }
            if(WarcraftStorageReader.GameVersion == null)
            {
                WarcraftStorageReader.GameVersion = new Version(1, 36, 1);
            }
            if(!Init.HasLoaded)
            {
                Init.Initialize(true);
            }
        }
    }
}
