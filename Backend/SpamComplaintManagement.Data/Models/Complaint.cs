namespace SpamComplaintManagement.Data.Models;

public class Complaint
{
    public int Id { get; set; }
    public string ComplaintId { get; set; } = "";
    public int UserId { get; set; }
    public string SpamType { get; set; } = "SMS";
    public string SpamNumber { get; set; } = "";
    public string Sender { get; set; } = "";
    public string CommunicationDetails { get; set; } = "";
    public string Status { get; set; } = "Submitted";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<ComplaintStatusHistory> History { get; set; } = new();
}

public class ComplaintStatusHistory
{
    public string Status { get; set; } = "";
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
