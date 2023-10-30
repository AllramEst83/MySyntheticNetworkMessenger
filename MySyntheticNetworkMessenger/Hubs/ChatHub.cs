using Microsoft.AspNetCore.SignalR;
using MySyntheticNetworkMessenger.Models;
using MySyntheticNetworkMessenger.Services;
using System.ComponentModel;

namespace MySyntheticNetworkMessenger.Hubs
{
    public enum ContactType
    {
        None = 0,
        [Description("Pappa")]
        Dada = 1,
        [Description("Lisa")]
        Lisa = 2,
        [Description("Olle")]
        Olle = 3
    }
    public class ChatHub : Hub
    {
        private readonly IChatGptService chatGptService;
        private readonly IChatHistoryService chatHistoryService;

        public ChatHub(
            IChatGptService chatGptService,
            IChatHistoryService chatHistoryService)
        {
            this.chatGptService = chatGptService;
            this.chatHistoryService = chatHistoryService;
        }

        public async Task SendMessage(string user, string message, string tabId)
        {
            string chatMessage = string.Empty;
            if (!string.IsNullOrEmpty(user))
            {
                if (int.TryParse(user, out int userInt) && Enum.IsDefined(typeof(ContactType), userInt))
                {
                    chatHistoryService.AddChatMessage(message, userInt, MessageType.Sent);

                    ContactType contactType = (ContactType)userInt;
                    chatMessage =  await chatGptService.GetResponseAsync(message, contactType, userInt);

                    chatHistoryService.AddChatMessage(chatMessage, userInt, MessageType.Received);
                }
            }

            await Clients.All.SendAsync("ReceiveMessage", user, chatMessage, tabId);
        }
    }
}
