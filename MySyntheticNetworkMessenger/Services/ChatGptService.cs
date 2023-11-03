using Microsoft.Extensions.Options;
using MySyntheticNetworkMessenger.Models;
using Newtonsoft.Json.Linq;
using System.Text;

namespace MySyntheticNetworkMessenger.Services
{
    public interface IChatGptService
    {
        Task<string> GetResponseAsync(int chatId);
    }
    public class ChatGptService : IChatGptService
    {
        private readonly AppSettings appSettings;
        private readonly IChatHistoryService chatHistoryService;
        private readonly IContactService contactService;
        private readonly ITemplateBuilderService templateBuilderService;
        private readonly HttpClient _httpClient;

        public ChatGptService(
            IOptions<AppSettings> appSettings,
             IChatHistoryService chatHistoryService,
             IContactService contactService,
             ITemplateBuilderService templateBuilderService)
        {
            this.appSettings = appSettings.Value;
            this.chatHistoryService = chatHistoryService;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.appSettings.ChatGptSettings.ApiKey}");
            this.contactService = contactService;
            this.templateBuilderService = templateBuilderService;
        }

        public async Task<string> GetResponseAsync(int chatId)
        {
            var contact = contactService.GetContact(chatId);
            string instructionTemplate = templateBuilderService.BuildInstructionTemplate(contact);           

            var messagesList = new List<object>
            {
                new { role = "system", content = instructionTemplate }
            };

            var chatHistory = chatHistoryService.GetChatMessages(chatId);
            foreach (var historyMessage in chatHistory)
            {
                var role = historyMessage.MessageType == MessageType.Sent ? "user" : "assistant";
                messagesList.Add(new { role = role, content = historyMessage.Message });
            }

            var requestPayload = new
            {
                messages = messagesList.ToArray(),
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
