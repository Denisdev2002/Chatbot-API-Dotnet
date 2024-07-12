using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Conversation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[JsonIgnore]
        public string? Id { get; set; }
        [JsonProperty("session_id")]
        public string? SessionId { get; set; }
        [JsonProperty("response")]
        public Response? Response { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("Response")]
        public string? ResponseId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("Session")]
        public string? IdSession { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Session? Session { get; set; }


    }

}