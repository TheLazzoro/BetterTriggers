using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Updater
{
    public class LoggingService
    {
#if DEBUG
        private string _url = "https://localhost:7030/Api";
#else
        private string _url = "https://bettertriggers.com/Api";
#endif

        public async Task SubmitUpdateError_Async(Exception ex)
        {
            try
            {
                var dto = new UpdateErrorDTO(ex);
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(30);
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync(_url + "/SubmitUpdateError", content);
            }
            catch (Exception)
            {

            }
        }
    }
}
