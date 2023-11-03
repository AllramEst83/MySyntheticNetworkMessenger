using MySyntheticNetworkMessenger.Models;
using System.Collections.Concurrent;

namespace MySyntheticNetworkMessenger.Services
{
    public interface IContactService
    {
        void AddOrUpdateContact(ContactData contactData);
        ContactData? GetContact(int chatId);
    }
    public class ContactService : IContactService
    {
        private static readonly ConcurrentDictionary<int, ContactData> Contacts = new ConcurrentDictionary<int, ContactData>();
        public void AddOrUpdateContact(ContactData contactData)
        {
            Contacts.AddOrUpdate(contactData.ChatId, contactData, (key, oldValue) => contactData);
        }

        public ContactData? GetContact(int chatId)
        {
            if (Contacts.TryGetValue(chatId, out ContactData contact))
            {
                return contact;
            }

            return null;
        }

    }
}
