using SpamComplaintManagement.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpamComplaintManagement.Data.Models;

namespace SpamComplaintManagement.Web.Pages.Complaints;

public class DetailsModel : PageModel
{
    private readonly IComplaintService _service;
    public DetailsModel(IComplaintService service) => _service = service;
    public Complaint? Complaint { get; set; }

    public IActionResult OnGet(string id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");
        Complaint = _service.Get(id, userId.Value);
        return Page();
    }
}
