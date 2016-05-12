using System;

namespace AxcessAssistant.DAL.Models
{
    [Serializable]
    public class Document
    {
        public int ID { get; set; } 
        public string Name { get; set; }
        public DocumentType DocType  { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int ClientID { get; set; }
        public decimal Amount { get; set; }
    }
}
