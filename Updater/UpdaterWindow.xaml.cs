using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Windows;

namespace Updater
{
    public partial class UpdaterWindow : Window
    {
        private Progress<long> _progress;
        private long _contentLength;
        private MemoryStream _downloadedFile;
        private BackgroundWorker _installThread;

        public UpdaterWindow()
        {
            InitializeComponent();

            Loaded += UpdaterWindow_Loaded;
        }

        private async void UpdaterWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _progress = new Progress<long>();
            _progress.ProgressChanged += _progress_ProgressChanged;
            await DownloadUpdate();

            txtDownloadInfo.Text = "Updating files...";
            progressBar.IsIndeterminate = true;

            _installThread = new BackgroundWorker();
            _installThread.RunWorkerCompleted += _installThread_RunWorkerCompleted;
            _installThread.DoWork += _installThread_DoWork;
            _installThread.RunWorkerAsync();
        }

        private void _progress_ProgressChanged(object? sender, long e)
        {
            var progress = (e / (double)_contentLength) * 100;
            progressBar.Value = progress;
            txtDownloadInfo.Text = $"Downloading:   {e / 1024 / 1024}/{_contentLength / 1024 / 1024} MB   {progress.ToString("0.")}%";
            Debug.WriteLine(progress);
        }

        private async Task DownloadUpdate()
        {
            // download from web
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60);
#if DEBUG
            string url = "https://localhost:7030/Api/download-latest";
#else
            string url = "https://bettertriggers.com/Api/download-latest";
#endif
            var cancellationToken = new CancellationToken();
            _downloadedFile = new MemoryStream();
            using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                if(!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Could not download update from {url}");
                }

                _contentLength = (long)response.Content.Headers.ContentLength;

                using (var download = await response.Content.ReadAsStreamAsync(cancellationToken))
                {
                    await CopyToAndReportProgressAsync(download, _downloadedFile, 81920, _progress, cancellationToken);
                }
            }

            await Task.Delay(100);
        }

        private static async Task CopyToAndReportProgressAsync(Stream source, Stream destination, int bufferSize, IProgress<long> progress = null, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!source.CanRead)
                throw new ArgumentException("Has to be readable", nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite)
                throw new ArgumentException("Has to be writable", nameof(destination));
            if (bufferSize < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
        }

        private void _installThread_DoWork(object? sender, DoWorkEventArgs e)
        {
            try
            {
                var tempDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Better Triggers\\temp");
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                // delete any temporary leftover files
                string[] filesystemEntries = Directory.GetFileSystemEntries(tempDir, "*", SearchOption.AllDirectories);
                foreach (var entry in filesystemEntries)
                {
                    if (File.Exists(entry))
                        File.Delete(entry);
                    else if (Directory.Exists(entry))
                        Directory.Delete(entry, true);
                }

                // write zip to disk
                var filename = Guid.NewGuid().ToString() + ".zip";
                var fullPath = Path.Combine(tempDir, filename);
                using (var fs = new FileStream(fullPath, FileMode.OpenOrCreate))
                {
                    _downloadedFile.Position = 0;
                    _downloadedFile.CopyTo(fs);
                }
                ZipFile.ExtractToDirectory(fullPath, tempDir);

                // remove files from the BT installation dir
                var btDir = Directory.GetCurrentDirectory();
                filesystemEntries = Directory.GetFileSystemEntries(btDir, "*", SearchOption.AllDirectories);
#if !DEBUG
            foreach (var filesystemEntry in filesystemEntries)
            {
                if (filesystemEntry.EndsWith("Updater.exe") ||
                   filesystemEntry.EndsWith("Updater.pdb"))
                {
                    continue;
                }

                if (File.Exists(filesystemEntry))
                    File.Delete(filesystemEntry);
                else if (Directory.Exists(filesystemEntry))
                    Directory.Delete(filesystemEntry, true);
            }
#endif

               // copy extracted files to the BT installation dir
               var pathToExtracted = Path.Combine(tempDir, "BetterTriggers");
                filesystemEntries = Directory.GetFileSystemEntries(pathToExtracted, "*", SearchOption.AllDirectories);
                foreach (var entry in filesystemEntries)
                {
                    var relativePath = Path.GetRelativePath(pathToExtracted, entry);
                    var destinationPath = Path.Combine(btDir, relativePath);
                    if (File.Exists(entry))
                    {
                        var parentFolder = Path.GetDirectoryName(destinationPath);
                        if (!Directory.Exists(parentFolder))
                        {
                            Directory.CreateDirectory(parentFolder);
                        }
                        File.Move(entry, destinationPath);
                    }
                }

                // cleanup
                Directory.Delete(pathToExtracted, true);
                File.Delete(fullPath);

                // launch BT
                var pathToBT = Path.Combine(btDir, "Better Triggers.exe");
                if(!File.Exists(pathToBT))
                {
                    pathToBT = Path.Combine(btDir, "BetterTriggers.exe");
                }

                if (File.Exists(pathToBT))
                {
                    Process.Start(pathToBT);
                }
            }
            catch (Exception ex)
            {
                installError = ex;
            }
        }

        private Exception installError;
        private void _installThread_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if(installError != null)
            {
                var service = new LoggingService();
                service.SubmitUpdateError_Async(installError);
                throw installError;
            }
            Application.Current.Shutdown();
        }
    }
}