using System.Collections.Generic;
using API.Objects;

namespace API.Interfaces
{
    public interface IContactCRUD
    {
        int PostContact(Contact contact);
        IEnumerable<Contact> GetAllContacts();
        Contact GetContact(int id);
        void UpdateContact(int id, Contact contact);
        void DeleteContact(int id);
    }
}
