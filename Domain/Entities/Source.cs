using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
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
