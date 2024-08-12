using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Logging
{
    public class LoggingService
    {
#if DEBUG
        private string _url = "https://localhost:7030/Api";
#else
        private string url = "https://bettertriggers.com/Api";
#endif


        public void SubmitError(Exception ex)
        {
            Task.Run(() => Async_SubmitError(ex));
        }

        public void SubmitSession()
        {
            Task.Run(() => Async_SubmitSession());
        }


        private async void Async_SubmitError(Exception ex)
        {
            try
            {
                var dto = new ErrorDTO(ex);
                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync("https://localhost:7030/Api/SubmitError", content);
            }
            catch (Exception)
            {

            }
        }

        private async void Async_SubmitSession()
        {
            try
            {
                CultureInfo ci = CultureInfo.InstalledUICulture;
                var dto = new SessionDTO
                {
                    MachineName = Environment.MachineName,
                    SystemLanguage = ci.Name,
                };
                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync("https://localhost:7030/Api/RegisterSession", content);
            }
            catch (Exception)
            {

            }
        }
    }
}
