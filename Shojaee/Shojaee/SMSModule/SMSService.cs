using System.Text;

namespace Shojaee.SMSModule
{
    public class SMSService
    {
        public async Task SendOneTimePassword(string code, string mobilenumber)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("x-api-key", "l2vucBE7MXeoXnfrparJbPdSwEWCzHhuf4vvm7Ox4zk0887ydOnJmQ7mT8kcts0o");
            var payload = $@"{{
                                ""mobile"": ""{mobilenumber}"",
                                ""templateId"": 100000,
                                ""parameters"": [
                                  {{
                                    ""name"": ""Code"",
                                    ""value"": ""{code}""
                                  }}
                                ]
                            }}";
            HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("https://api.sms.ir/v1/send/verify", content);
        }
    }
}
