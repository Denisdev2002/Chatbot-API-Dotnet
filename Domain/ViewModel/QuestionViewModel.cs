using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel
{
    public class QuestionViewModel
    {
        [Key]
        public string? QuestionId { get; set; }
        public string? Text { get; set; }
        public string? Session_id { get; set; }
        public string IdSession { get; set; }
    }
}