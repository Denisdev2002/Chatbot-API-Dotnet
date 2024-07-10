
namespace Domain.ViewModel
{
    public class ConversationViewModel
    {
        public string? Id { get; set; }
        public string? SessionId { get; set; }
        public string? IdSession { get; set; }
        public string? ResponseId { get; set; }
    }

    public class ResponseViewModel
    {
        public string? ResponseId { get; set; }
        public string? Result { get; set; }
        public string? SourceId { get; set; }
    }

    public class SourceViewModel
    {
        public string? SourceId { get; set; }
        public string? SourceDocument { get; set; }
        public float? PageDocument { get; set; }
    }
}