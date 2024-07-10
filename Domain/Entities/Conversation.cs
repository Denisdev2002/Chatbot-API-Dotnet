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
        public string Id { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("Session")]
        public string? IdSession { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Session Session { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [ForeignKey("Response")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ResponseId { get; set; }

        [JsonProperty("response")]
        public Response Response { get; set; }


    }

    public class Response
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [System.Text.Json.Serialization.JsonIgnore]
        public string ResponseId { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("sources")]
        public List<Source> Sources { get; set; }

    }

    public class Source
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [System.Text.Json.Serialization.JsonIgnore]
        public string SourceId { get; set; }

        [JsonProperty("source_document")]
        public string? SourceDocument { get; set; }

        [JsonProperty("page_document")]
        public float? PageDocument { get; set; }
    }
}