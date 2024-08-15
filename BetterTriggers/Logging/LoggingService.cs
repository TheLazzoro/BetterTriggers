﻿using Newtonsoft.Json;
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
        private string _url = "https://bettertriggers.com/Api";
#endif


        public void SubmitSession()
        {
            Task.Run(() => Async_SubmitSession());
        }

        public void SubmitError(Exception ex, string comment = null)
        {
            Task.Run(() => Async_SubmitError(ex, comment));
        }

        public void SubmitReport(string comment)
        {
            Task.Run(() => Async_SubmitReport(comment));
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
                await client.PostAsync(_url + "/RegisterSession", content);
            }
            catch (Exception)
            {

            }
        }

        private async void Async_SubmitError(Exception ex, string comment = null)
        {
            try
            {
                var dto = new ErrorDTO(ex);
                dto.Comment = comment;
                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync(_url + "/SubmitError", content);
            }
            catch (Exception)
            {

            }
        }

        private async void Async_SubmitReport(string comment)
        {
            try
            {
                var dto = new IssueDTO(comment);
                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync(_url + "/SubmitIssue", content);
            }
            catch (Exception)
            {

            }
        }
    }
}