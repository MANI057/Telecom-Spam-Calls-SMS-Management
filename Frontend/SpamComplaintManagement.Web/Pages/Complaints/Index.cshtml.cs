using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SpamComplaintManagement.Web.Pages.Complaints;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public List<ComplaintViewModel> Complaints { get; set; } = new();

    public string? ErrorMessage { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SpamType { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public int TotalRecords { get; set; }

    public int TotalPages { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var userIdString = HttpContext.Session.GetString("UserId");

            if (!int.TryParse(userIdString, out var userId))
            {
                ErrorMessage = "User session has expired. Please login again.";
                return;
            }

            var client = _httpClientFactory.CreateClient("SpamApi");

            var url =
                $"api/Complaint/user/{userId}" +
                $"?search={Uri.EscapeDataString(Search ?? string.Empty)}" +
                $"&status={Uri.EscapeDataString(Status ?? string.Empty)}" +
                $"&spamType={Uri.EscapeDataString(SpamType ?? string.Empty)}" +
                $"&pageNumber={PageNumber}" +
                $"&pageSize={PageSize}";

            var result =
                await client.GetFromJsonAsync<ComplaintSearchResult>(url);

            if (result != null)
            {
                Complaints = result.Complaints ?? new List<ComplaintViewModel>();
                TotalRecords = result.TotalRecords;
                TotalPages = result.TotalPages;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Unable to load complaints: {ex.Message}";
        }
    }

    public class ComplaintSearchResult
    {
        public List<ComplaintViewModel> Complaints { get; set; } = new();

        public int TotalRecords { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }
    }

    public class ComplaintViewModel
    {
        public string ComplaintId { get; set; } = "";

        public string SpamType { get; set; } = "";

        public string SpamNumber { get; set; } = "";

        public string Sender { get; set; } = "";

        public string Status { get; set; } = "";
    }
}
