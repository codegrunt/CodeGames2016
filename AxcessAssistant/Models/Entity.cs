namespace AxcessAssistant.Models
{
    public enum EntityType
    {
        Client,
        Invoice,
        Document,
        Project,
        Note,
        PhoneNumber
    }

    public class Entity
    {
        public EntityType EntityType { get; set; }
        public string EntityName { get; set; }
        public string EntityId { get; set; }
    }
}