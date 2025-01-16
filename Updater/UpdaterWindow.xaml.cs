using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Windows;

namespace Updater
{
    public partial class UpdaterWindow : Window
    {
        private BackgroundWorker _worker;

        public UpdaterWindow()
        {
            InitializeComponent();

            Loaded += UpdaterWindow_Loaded;
        }

        private void UpdaterWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
            _worker.RunWorkerAsync();
        }

        private async void _worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            // download from web
            using var client = new HttpClient();
            //using var s = await client.GetStreamAsync("https://via.placeholder.com/150");

            var tempDir = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
            var filename = Guid.NewGuid().ToString() + ".zip";
            var fullPath = Path.Combine(tempDir, filename);
            using var fs = new FileStream(fullPath, FileMode.OpenOrCreate);
            //s.CopyTo(fs);

            var btDir = Directory.GetCurrentDirectory();
            string[] filesystemEntries = Directory.GetFileSystemEntries(btDir, "*", SearchOption.AllDirectories);
            foreach (var filesystemEntry in filesystemEntries)
            {
                if(filesystemEntry.EndsWith("Updater.exe") ||
                   filesystemEntry.EndsWith("Updater.pdb"))
                {
                    continue;
                }

                if(File.Exists(filesystemEntry))
                    File.Delete(filesystemEntry);
                else if(Directory.Exists(filesystemEntry))
                    Directory.Delete(filesystemEntry, true);
            }
        }

        private void _worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {

        }
    }
}