using System;
using System.Collections.Generic;

namespace AxcessAssistant.DAL.Models
{
    [Serializable]
    public class Client
    {
        public int ID { get; set; }
        public string ClientName { get; set; }
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Office { get; set; }
        public List<Note> Notes { get; set; }
        public decimal ARBalance { get; set; }
    }

    [Serializable]
    public class Note
    {
        public DateTime DateTime { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return $"{this.DateTime.ToString("MM-dd-yyyy")}: {this.Text}";
        }
    }
}
