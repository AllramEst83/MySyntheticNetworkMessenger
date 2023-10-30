using MySyntheticNetworkMessenger.Models;
using System.Collections.Concurrent;

namespace MySyntheticNetworkMessenger.Services
{
    public interface IChatHistoryService
    {
        bool AddChatMessage(string message, int chatId, MessageType messageType);
        List<ChatMessage> GetChatMessages(int chatId);
        bool DeleteChatHistory(int chatId);
    }

    public class ChatHistoryService : IChatHistoryService
    {
        private static readonly ConcurrentDictionary<int, List<ChatMessage>> ChatHistories = new ConcurrentDictionary<int, List<ChatMessage>>();

        public bool AddChatMessage(string message, int chatId, MessageType messageType)
        {
            List<ChatMessage> chatHistory;
            if (ChatHistories.ContainsKey(chatId))
            {
                chatHistory = ChatHistories[chatId];
            }
            else
            {
                chatHistory = new List<ChatMessage>();
            }

            chatHistory.Add(new ChatMessage()
            {
                userId = chatId,
                MessageType = messageType,
                Message = message
            });

            ChatHistories.AddOrUpdate(chatId, chatHistory, (key, oldValue) => chatHistory);

            return true;
        }

        public List<ChatMessage> GetChatMessages(int chatId)
        {
            if (ChatHistories.TryGetValue(chatId, out var chatHistory))
            {
                return chatHistory;
            }
            else
            {
                return new List<ChatMessage>();
            }
        }

        public bool DeleteChatHistory(int chatId)
        {
            bool isRemoved = ChatHistories.TryRemove(chatId, out var removedChatHistory);
            return isRemoved;
        }

    }
}
