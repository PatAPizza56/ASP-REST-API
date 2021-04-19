using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using API.Interfaces;
using API.Objects;

namespace API.Controllers
{
    [ApiController]
    public class ContactController : ControllerBase
    {
        IContactCRUD contactCRUD = new SQLContactCrud();

        [HttpPost]
        [Route("/contact")]
        public int PostContact(Contact contact)
        {
            return contactCRUD.PostContact(contact);
        }

        [HttpGet]
        [Route("/contact")]
        public IEnumerable<Contact> GetAllContacts()
        {
            return contactCRUD.GetAllContacts();
        }

        [HttpGet]
        [Route("/contact/{id}")]
        public Contact GetContact(int id)
        {
            return contactCRUD.GetContact(id);
        }

        [HttpPut]
        [Route("/contact/{id}")]
        public void UpdateContact(int id, Contact contact)
        {
            contactCRUD.UpdateContact(id, contact);
        }

        [HttpDelete]
        [Route("/contact/{id}")]
        public void DeleteContact(int id)
        {
            contactCRUD.DeleteContact(id);
        }
    }

    public class MemoryContactCRUD : IContactCRUD
    {
        static List<Contact> contacts = new List<Contact>() { { new Contact() { Name = "Michael", PhoneNumber = "631 338 3692" } }, { new Contact() { Name = "Alex", PhoneNumber = "631 456 2453" } } };

        // Create
        public int PostContact(Contact contact)
        {
            contacts.Add(contact);
            return contacts.IndexOf(contact);
        }

        // Read
        public IEnumerable<Contact> GetAllContacts()
        {
            return contacts;
        }
        public Contact GetContact(int id)
        {
            for (int i = 0; i < contacts.Count; i++)
            {
                if (i == id)
                {
                    return contacts[i];
                }
            }
            return null;
        }

        // Update
        public void UpdateContact(int id, Contact contact)
        {
            for (int i = 0; i < contacts.Count; i++)
            {
                if (i == id)
                {
                    contacts[i] = contact;
                }
            }
        }

        // Delete
        public void DeleteContact(int id)
        {
            for (int i = 0; i < contacts.Count; i++)
            {
                if (i == id)
                {
                    contacts.RemoveAt(i);
                }
            }
        }
    }

    public class JSONContactCRUD : IContactCRUD
    {
        List<Contact> contacts = new List<Contact>();

        public JSONContactCRUD()
        {
            contacts = JsonConvert.DeserializeObject<List<Contact>>(System.IO.File.ReadAllText(@"C:\Users\Michael\source\repos\API\API\ContactList.json"));
        }

        // Create
        public int PostContact(Contact contact)
        {
            contacts.Add(contact);
            UpdateContactsJSON();

            return contacts.IndexOf(contact);
        }

        // Read
        public IEnumerable<Contact> GetAllContacts()
        {
            return contacts;
        }
        public Contact GetContact(int id)
        {
            for (int i = 0; i < contacts.Count; i++)
            {
                if (i == id)
                {
                    return contacts[i];
                }
            }
            return null;
        }

        // Update
        public void UpdateContact(int id, Contact contact)
        {
            for (int i = 0; i < contacts.Count; i++)
            {
                if (i == id)
                {
                    contacts[i] = contact;
                    UpdateContactsJSON();
                }
            }
        }

        // Delete
        public void DeleteContact(int id)
        {
            for (int i = 0; i < contacts.Count; i++)
            {
                if (i == id)
                {
                    contacts.RemoveAt(i);
                    UpdateContactsJSON();
                }
            }
        }

        void UpdateContactsJSON()
        {
            System.IO.File.WriteAllText(@"C:\Users\Michael\source\repos\API\API\ContactList.json", JsonConvert.SerializeObject(contacts, Formatting.Indented));
        }
    }

    public class SQLContactCrud : IContactCRUD
    {
        MySqlConnection connection;
        string server;
        string database;
        string uid;
        string password;
        string connectionString;

        public SQLContactCrud()
        {
            server = "sql5.freesqldatabase.com";
            database = "sql5391394";
            uid = "sql5391394";
            password = "vciDSZcCAl";
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        #region "Connect"

        bool StartConnect()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        bool CloseConnect()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        // Create

        public int PostContact(Contact contact)
        {
            string query = $"INSERT INTO `Contacts`(`ID`, `Name`, `Phone Number`) VALUES ({contact.ID},{contact.Name},{contact.PhoneNumber})";

            if (StartConnect() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);

                cmd.ExecuteNonQuery();

                CloseConnect();
            }

            return contact.ID;
        }   

        // Get

        public IEnumerable<Contact> GetAllContacts()
        {
            string query = "SELECT * FROM `Contacts` WHERE 1";

            List<Contact> contacts = new List<Contact>();

            if (StartConnect() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    Contact contact = new Contact()
                    {
                        ID = (int)dataReader["ID"],
                        Name = (string)dataReader["Name"],
                        PhoneNumber = (string)dataReader["Phone Number"]
                    };

                    contacts.Add(contact);
                }

                dataReader.Close();

                CloseConnect();

                return contacts;
            }

            return null;
        }

        public Contact GetContact(int id)
        {
            string query = $"SELECT * FROM `Contacts` WHERE `ID`={id}";

            List<Contact> contacts = new List<Contact>();

            if (StartConnect() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    Contact contact = new Contact()
                    {
                        ID = (int)dataReader["ID"],
                        Name = (string)dataReader["Name"],
                        PhoneNumber = (string)dataReader["Phone Number"]
                    };

                    contacts.Add(contact);
                }

                dataReader.Close();

                CloseConnect();

                return contacts.FirstOrDefault();
            }

            return null;
        }

        // Update

        public void UpdateContact(int id, Contact contact)
        {
            return;
        }

        // Delete

        public void DeleteContact(int id)
        {
            return;
        }
    }
}
