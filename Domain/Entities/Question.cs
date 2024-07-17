using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Question
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string? text { get; set; }
        public string? session_id { get; set; }
        public string? user_type { get; set; }
        [JsonIgnore]
        [ForeignKey("Session")]
        public string? IdSession { get; set; }
        [JsonIgnore]
        public Session? Session { get; set; }
        [JsonIgnore]
        [ForeignKey("Conversation")]
        public string? IdConversation { get; set; }
        [JsonIgnore]
        public Conversation? Conversation { get; set; }
    }
}