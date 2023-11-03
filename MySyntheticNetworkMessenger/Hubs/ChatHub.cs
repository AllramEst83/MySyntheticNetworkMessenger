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
        private readonly IContactService contactService;

        public ChatHub(
            IChatGptService chatGptService,
            IChatHistoryService chatHistoryService,
            IContactService contactService)
        {
            this.chatGptService = chatGptService;
            this.chatHistoryService = chatHistoryService;
            this.contactService = contactService;
        }

        public async Task SendMessage(string user, string message, string tabId)
        {
            string chatMessage = string.Empty;
            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(message))
            {
                if (int.TryParse(user, out int userInt))
                {
                    chatHistoryService.AddChatMessage(message, userInt, MessageType.Sent);

                    chatMessage = await chatGptService.GetResponseAsync(userInt);

                    chatHistoryService.AddChatMessage(chatMessage, userInt, MessageType.Received);
                }
             
                await Clients.All.SendAsync("ReceiveMessage", user, chatMessage, tabId);
            }
        }

        public async Task AddContact(ContactData contactData)
        {
            contactService.AddOrUpdateContact(contactData);

            await Clients.All.SendAsync("ContactAdded", contactData);
        }
    }
}
