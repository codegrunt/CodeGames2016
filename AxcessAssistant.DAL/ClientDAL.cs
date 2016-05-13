using System;
using System.Collections.Generic;
using System.Linq;
using AxcessAssistant.DAL.Models;

namespace AxcessAssistant.DAL
{
    public class ClientDAL
    {
        private static List<Client> _clients;

        public ClientDAL()
        {
            if(_clients == null)
                init();
        }

        public Client GetClientById(int id)
        {
            return _clients.FirstOrDefault(x => x.ID == id);
        }

        public List<Client> FindClientsByName(string name)
        {
            return _clients.FindAll(x => x.ClientName.ToLower().Contains(name.ToLower()) || x.ContactName.Replace(".", " .").ToLower().Contains(name.ToLower()));
        }     

        public Client GetClientByContactPhoneNumber(string phoneNumber)
        {
            return _clients.FirstOrDefault(x => x.PhoneNumber == phoneNumber);
        }

        public Client GetClientByContactEmail(string email)
        {
            return _clients.FirstOrDefault(x => x.Email == email);
        }

        public bool AddNote(int clientId, string note)
        {
            var clt = GetClientById(clientId);
            if (clt != null)
                clt.Notes.Add(new Note { Text = note, DateTime = DateTime.Now });
            return true;
        }

        private void init()
        {
            _clients = new List<Client>();
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
                Notes = new List<Note> { new Note { DateTime = new DateTime(2016, 2, 12), Text = "Need more information on corrected W-2" } }
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
