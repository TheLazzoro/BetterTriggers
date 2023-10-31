using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace GUI.Components.VersionCheck
{
    internal class VersionCheck
    {
        private const string url = "https://api.github.com/repos/thelazzoro/BetterTriggers/releases/latest";
        private const string accept = "application/vnd.github+json";
        private string userAgent;
        private string version;

        public VersionCheck()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            version = fvi.FileVersion;

            while (version.EndsWith(".0"))
            {
                version = version.Substring(0, version.Length - 2);
            }

            userAgent = "BetterTriggers/" + version;

            Task.Run(GetNewestVersionAsync);
        }

        private async Task GetNewestVersionAsync()
        {
            HttpRequestMessage message = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
            };
            message.Headers.Add("accept", accept);
            message.Headers.Add("User-Agent", userAgent);
            HttpClient client = new HttpClient();
            var response = await client.SendAsync(message);
            string content = await response.Content.ReadAsStringAsync();
            VersionDTO versionDTO = JsonConvert.DeserializeObject<VersionDTO>(content);
            
            if(!response.IsSuccessStatusCode || versionDTO == null || string.IsNullOrEmpty(versionDTO.name))
            {
                return;
            }

            if(!versionDTO.html_url.EndsWith(this.version))
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    var window = new NewVersionWindow(versionDTO);
                    window.ShowDialog();
                });
            }
        }

    }
}
