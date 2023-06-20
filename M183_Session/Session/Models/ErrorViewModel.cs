namespace Session.Models
{
    public class ErrorViewModel
    {
        public string? TraceId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(TraceId);
    }
}