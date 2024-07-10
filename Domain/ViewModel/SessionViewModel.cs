using Domain.Entities.DataTransferObject;

namespace Domain.ViewModel
{
    public class SessionViewModel
    {
        public string Id { get; set; }
        public string? EmailUser { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}