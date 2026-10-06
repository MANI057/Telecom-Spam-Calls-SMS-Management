using SpamComplaintManagement.Data.Models;

namespace SpamComplaintManagement.Data.Repositories;

public class InMemoryRepository
{
    private readonly List<User> _users = new();
    private readonly List<Complaint> _complaints = new();
    private int _userId = 0;
    private int _complaintId = 0;

    public User? FindUserByEmail(string email) =>
        _users.FirstOrDefault(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

    public User AddUser(User user)
    {
        user.Id = ++_userId;
        _users.Add(user);
        return user;
    }

    public Complaint AddComplaint(Complaint complaint)
    {
        complaint.Id = ++_complaintId;
        _complaints.Add(complaint);
        return complaint;
    }

    public List<Complaint> GetComplaintsByUser(int userId) =>
        _complaints.Where(x => x.UserId == userId)
                   .OrderByDescending(x => x.CreatedAt)
                   .ToList();

    public Complaint? GetComplaintForUser(string complaintId, int userId) =>
        _complaints.FirstOrDefault(x =>
            x.ComplaintId.Equals(complaintId, StringComparison.OrdinalIgnoreCase) &&
            x.UserId == userId);

    public int GetComplaintCount(int userId) => _complaints.Count(x => x.UserId == userId);
}
