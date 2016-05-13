namespace AxcessAssistant.Models
{
    public enum EntityType
    {
        Client,
        Invoice,
        Project,
        Note,
        Ordinal
    }

    public class Entity
    {
        public EntityType EntityType { get; set; }
        public string EntityValue { get; set; }
    }
}