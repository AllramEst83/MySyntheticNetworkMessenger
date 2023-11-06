using Microsoft.Extensions.Options;
using MySyntheticNetworkMessenger.Models;
using System.Collections.Concurrent;

namespace MySyntheticNetworkMessenger.Services
{
    public interface IChatHistoryService
    {
        bool AddChatMessage(string message, int chatId, MessageType messageType);
        List<ChatMessage> GetChatMessages(int chatId);
        bool DeleteChatHistory(int chatId);
        void ReCalculateHistoryLength(int chatId);
    }

    public class ChatHistoryService : IChatHistoryService
    {
        private static readonly ConcurrentDictionary<int, List<ChatMessage>> ChatHistories = new ConcurrentDictionary<int, List<ChatMessage>>();
        private readonly AppSettings appSettings;
        public ChatHistoryService(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }
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
                UserId = chatId,
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

        private int EstimateTokenCount(string message)
        {
            // Estimate the token count based on the length of the message
            // assuming that on average, one token is 2.85 characters long in your specific use case
            return (int)Math.Ceiling(message.Length / 2.85);
        }

        public void ReCalculateHistoryLength(int chatId)
        {
            // Check if the specified chat history exists
            if (ChatHistories.TryGetValue(chatId, out List<ChatMessage> chatHistory))
            {
                // Get the current total token count
                int totalTokenCount = chatHistory.Sum(message => EstimateTokenCount(message.Message));

                // Calculate how many tokens we need to remove
                int tokensToRemove = totalTokenCount - appSettings.ChatGptSettings.MaxTokens;

                // Ensure that the chatHistory is sorted from oldest to newest
                chatHistory = chatHistory.OrderBy(x => x.Timestamp).ToList();

                while (tokensToRemove > 0 && chatHistory.Count > 0)
                {
                    // The first message is the oldest due to the ordering above
                    var messageToRemove = chatHistory[0];
                    int messageTokenCount = EstimateTokenCount(messageToRemove.Message);

                    // Only remove the message if doing so won't make tokensToRemove negative
                    if (tokensToRemove - messageTokenCount >= 0)
                    {
                        tokensToRemove -= messageTokenCount;
                        chatHistory.RemoveAt(0); // Removes the oldest message
                    }
                    else
                    {
                        // If removing the message would make tokensToRemove negative,
                        // break out of the loop as we can't remove any more messages without going below zero.
                        break;
                    }
                }
            }
        }
    }
}
