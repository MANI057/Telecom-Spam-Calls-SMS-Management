
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SpamComplaintManagement.Web.Pages;

public class DashboardModel : PageModel
{
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        UserName = HttpContext.Session.GetString("UserName") ?? "User";
        UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";

        return Page();
    }
}

