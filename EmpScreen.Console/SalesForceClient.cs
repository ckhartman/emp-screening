using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace EmpScreen.Console
{
    class SalesForceClient
    {
        private const string LOGIN_ENDPOINT = "https://cityofhope--ctracker.my.salesforce.com/services/oauth2/token";
        private const string API_ENDPOINT = "/services/data/v48.0/sobjects/Temperature_Check__c";

        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public static string AuthToken { get; set; }
        public string InstanceUrl { get; set; }
        public string SecurityToken { get; set; }

        public void Login()
        {
            String jsonResponse;
            using (var client = new HttpClient())
            {
                var request = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"client_id", ClientId},
                {"client_secret", ClientSecret},
                {"username", Username},
                {"password", Password},
                {"security_token", SecurityToken}
            }
                );
                //request.Headers.Add("X-PrettyPrint", "1");
                var response = client.PostAsync(LOGIN_ENDPOINT, request).Result;
                jsonResponse = response.Content.ReadAsStringAsync().Result;
            }
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);
            AuthToken = values["access_token"];
            InstanceUrl = values["instance_url"];
        }

        public string Query(string content)
        {
            using (var client = new HttpClient())
            {
                string restRequest = InstanceUrl + API_ENDPOINT;
                var request = new HttpRequestMessage(HttpMethod.Post, restRequest);
                request.Headers.Add("Authorization", "Bearer " + AuthToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //request.Headers.Add("X-PrettyPrint", "1");
                System.Net.Http.StringContent stringContent = new StringContent(content, UnicodeEncoding.UTF8,"application/json");
                request.Content = stringContent;
                var response = client.SendAsync(request).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
