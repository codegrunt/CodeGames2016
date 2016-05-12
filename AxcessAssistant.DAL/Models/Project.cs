using System;
using System.Collections.Generic;

namespace AxcessAssistant.DAL.Models
{
    [Serializable]
    public class Project
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ClientID { get; set; }
        public string Status { get; set; }
    }
}
