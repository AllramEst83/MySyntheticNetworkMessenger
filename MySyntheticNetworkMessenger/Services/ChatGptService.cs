using Microsoft.Extensions.Options;
using MySyntheticNetworkMessenger.Hubs;
using MySyntheticNetworkMessenger.Models;
using Newtonsoft.Json.Linq;
using System.Text;

namespace MySyntheticNetworkMessenger.Services
{
    public interface IChatGptService
    {
        Task<string> GetResponseAsync(string transcription, ContactType contactType);
    }
    public class ChatGptService : IChatGptService
    {
        private readonly AppSettings appSettings;
        private readonly HttpClient _httpClient;

        public ChatGptService(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.appSettings.ChatGptSettings.ApiKey}");
        }

        public async Task<string> GetResponseAsync(string message, ContactType contactType)
        {
            string instruction = string.Empty;
            switch (contactType)
            {
                case ContactType.Dada:
                    instruction = "Alla svar ska vara på språket Svenska (SE-sv). Svara som att du är 87 år och pratar med din son. Håll det kort och enkelt.";
                    break;
                case ContactType.Lisa:
                    instruction = "Alla svar ska vara på språket Svenska (SE-sv). Svara som att du är 15 årig tjej från 1990 talet. Du pratar med din bästis. Undvika för långa svar.";
                    break;
                case ContactType.Olle:
                    break;
                default:
                    break;
            }

            var requestPayload = new
            {
                messages = new[]
                {
                    new { role = "system", content = instruction },
                    new { role = "user", content = message }
                },
                max_tokens = appSettings.ChatGptSettings.MaxTokens,
                temperature = appSettings.ChatGptSettings.Temperature,
                top_p = appSettings.ChatGptSettings.TopP,
                model = appSettings.ChatGptSettings.Model
            };

            var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload);
            var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(appSettings.ChatGptSettings.EndPoint, httpContent);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject parsedResponse = JObject.Parse(jsonResponse);
                return parsedResponse["choices"][0]["message"]["content"].ToString();
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve response: {response.ReasonPhrase}. Response content: {errorContent}");
            }
        }
    }
}
