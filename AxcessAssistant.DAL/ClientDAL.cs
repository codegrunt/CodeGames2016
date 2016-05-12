using System;
using System.Collections.Generic;
using System.Linq;
using AxcessAssistant.DAL.Models;

namespace AxcessAssistant.DAL
{
    public class ClientDAL
    {
        private List<Client> _clients;

        public ClientDAL()
        {
            init();
        }

        public Client GetClientById(int id)
        {
            return _clients.FirstOrDefault(x => x.ID == id);
        }

        public List<Client> FindClientsByName(string clientName)
        {
            return _clients.FindAll(x => x.ClientName.Contains(clientName));
        }     
        
        public List<Client> FindClientsByContactName(string contactName)
        {
            return _clients.FindAll(x => x.ContactName.Contains(contactName));
        }

        public Client GetClientByContactPhoneNumber(string phoneNumber)
        {
            return _clients.FirstOrDefault(x => x.PhoneNumber == phoneNumber);
        }

        public Client GetClientByContactEmail(string email)
        {
            return _clients.FirstOrDefault(x => x.Email == email);
        }

        private void init()
        {
            _clients.Add(new Client
            {
                ID = 1,
                ClientName = "Groovetechi",
                ContactName = "Chris P. Bacon",
                PhoneNumber = "2145551234",
                Email = "chrispbacon@hotmail.com",
                ARBalance = 2508.00m,
                Office = "Irving 175",
                Notes = new List<Note> { new Note { DateTime = new DateTime(2016, 3, 1), Text = "Called to check on account balance"}, new Note { DateTime = new DateTime(2016, 3, 1), Text = "Called to check on account balance" } }
            });

            _clients.Add(new Client
            {
                ID = 2,
                ClientName = "Unoquote",
                ContactName = "Eileen Sideways",
                PhoneNumber = "9725786423",
                Email = "eileen.sideways@gmail.com",
                ARBalance = 947.00m,
                Office = "Irving 175",
            });

            _clients.Add(new Client
            {
                ID = 3,
                ClientName = "Tripplecom",
                ContactName = "Barb Akew",
                PhoneNumber = "8178764489",
                Email = "barb.akew@yahoo.com",
                ARBalance = 2587.00m,
                Office = "Irving 175",
            });

            _clients.Add(new Client
            {
                ID = 3,
                ClientName = "Movela",
                ContactName = "Teri Dactyl",
                PhoneNumber = "8178743597",
                Email = "teri.dactyl@yahoo.com",
                ARBalance = 2587.00m,
                Office = "Irving 175",
            });
        }
    }
}
