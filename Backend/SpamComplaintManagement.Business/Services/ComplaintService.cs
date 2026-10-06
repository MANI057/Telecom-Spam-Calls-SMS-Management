using SpamComplaintManagement.Business.Interfaces;
using SpamComplaintManagement.Data.Models;
using SpamComplaintManagement.Data.Repositories;

namespace SpamComplaintManagement.Business.Services;

public class ComplaintService : IComplaintService
{
    private readonly InMemoryRepository _repo;

    public ComplaintService(InMemoryRepository repo)
    {
        _repo = repo;
    }

    public (bool Success, string Message, Complaint? Complaint) Create(
        int userId,
        string spamType,
        string spamNumber,
        string sender,
        string details)
    {
        if (string.IsNullOrWhiteSpace(spamType) ||
            string.IsNullOrWhiteSpace(spamNumber) ||
            string.IsNullOrWhiteSpace(sender) ||
            string.IsNullOrWhiteSpace(details))
        {
            return (false, "Please complete all complaint fields.", null);
        }

        var complaint = new Complaint
        {
            ComplaintId =
                $"CMP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"[..17]
                .ToUpperInvariant(),

            UserId = userId,
            SpamType = spamType,
            SpamNumber = spamNumber.Trim(),
            Sender = sender.Trim(),
            CommunicationDetails = details.Trim(),
            Status = "Submitted"
        };

        complaint.History.Add(
            new ComplaintStatusHistory
            {
                Status = "Submitted"
            });

        return (
            true,
            "Complaint submitted successfully.",
            _repo.AddComplaint(complaint)
        );
    }

    public List<Complaint> GetForUser(int userId)
    {
        return _repo.GetComplaintsByUser(userId);
    }

    public Complaint? Get(string complaintId, int userId)
    {
        return _repo.GetComplaintForUser(complaintId, userId);
    }

    public (List<Complaint> Complaints, int TotalRecords) SearchAndFilter(
        int userId,
        string? search,
        string? status,
        string? spamType,
        int pageNumber,
        int pageSize)
    {
        var complaints = _repo.GetComplaintsByUser(userId)
            .AsEnumerable();

        // SEARCH
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            complaints = complaints.Where(c =>
                c.ComplaintId.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase) ||

                c.SpamNumber.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase) ||

                c.Sender.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase) ||

                c.SpamType.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase));
        }

        // STATUS FILTER
        if (!string.IsNullOrWhiteSpace(status))
        {
            complaints = complaints.Where(c =>
                c.Status.Equals(
                    status,
                    StringComparison.OrdinalIgnoreCase));
        }

        // SPAM TYPE FILTER
        if (!string.IsNullOrWhiteSpace(spamType))
        {
            complaints = complaints.Where(c =>
                c.SpamType.Equals(
                    spamType,
                    StringComparison.OrdinalIgnoreCase));
        }

        var filteredComplaints = complaints.ToList();

        var totalRecords = filteredComplaints.Count;

        // PAGINATION
        var pagedComplaints = filteredComplaints
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (pagedComplaints, totalRecords);
    }
}
