namespace AxcessAssistant.DAL.Models
{
    public class Client
    {
        public int ID { get; set; }
        public string ClientName { get; set; }
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Office { get; set; }

        public decimal ARBalance { get; set; }
    }
}
