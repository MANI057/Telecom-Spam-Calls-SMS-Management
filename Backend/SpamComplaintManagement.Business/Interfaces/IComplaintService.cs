using SpamComplaintManagement.Data.Models;

namespace SpamComplaintManagement.Business.Interfaces;

public interface IComplaintService
{
    (bool Success, string Message, Complaint? Complaint) Create(
        int userId, string spamType, string spamNumber, string sender, string details);

    List<Complaint> GetForUser(int userId);

    Complaint? Get(string complaintId, int userId);

    (List<Complaint> Complaints, int TotalRecords) SearchAndFilter(
        int userId,
        string? search,
        string? status,
        string? spamType,
        int pageNumber,
        int pageSize);
}
