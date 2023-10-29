using Microsoft.AspNetCore.SignalR;
using MySyntheticNetworkMessenger.Services;

namespace MySyntheticNetworkMessenger.Hubs
{
    public enum ContactType
    {
        None = 0,
        Dada = 1,
        Lisa = 2,
        Olle = 3
    }
    public class ChatHub : Hub
    {
        IChatGptService chatGptService;
        public ChatHub(IChatGptService chatGptService)
        {
            this.chatGptService = chatGptService;
        }

        public async Task SendMessage(string user, string message, string tabId)
        {
            string chatMessage = string.Empty;
            ContactType contactType = ContactType.None;
            if (!string.IsNullOrEmpty(user))
            {
                if (int.TryParse(user, out int userInt) && Enum.IsDefined(typeof(ContactType), userInt))
                {
                    contactType = (ContactType)userInt;
                    chatMessage=  await chatGptService.GetResponseAsync(message, contactType);
                }
            }

            await Clients.All.SendAsync("ReceiveMessage", user, chatMessage, tabId);
        }
    }
}
