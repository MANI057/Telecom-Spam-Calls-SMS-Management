
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SpamComplaintManagement.Web.Pages.Complaints;

public class CreateModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CreateModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public string SpamType { get; set; } = string.Empty;

    [BindProperty]
    public string SpamNumber { get; set; } = string.Empty;

    [BindProperty]
    public string Sender { get; set; } = string.Empty;

    [BindProperty]
    public string Details { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public string SuccessMessage { get; set; } = string.Empty;

    public string ComplaintId { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
            return RedirectToPage("/Account/Login");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
            return RedirectToPage("/Account/Login");

        if (string.IsNullOrWhiteSpace(SpamType) ||
            string.IsNullOrWhiteSpace(SpamNumber) ||
            string.IsNullOrWhiteSpace(Sender) ||
            string.IsNullOrWhiteSpace(Details))
        {
            ErrorMessage = "Please complete all complaint fields.";
            return Page();
        }

        var client = _httpClientFactory.CreateClient("SpamComplaintAPI");

        var request = new
        {
            userId = userId.Value,
            spamType = SpamType.Trim(),
            spamNumber = SpamNumber.Trim(),
            sender = Sender.Trim(),
            details = Details.Trim()
        };

        try
        {
            var response = await client.PostAsJsonAsync(
                "api/Complaint",
                request);

            var responseBody =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Unable to submit complaint.";
                return Page();
            }

            var result =
                System.Text.Json.JsonSerializer.Deserialize<CreateComplaintResponse>(
                    responseBody,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (result?.Complaint == null)
            {
                ErrorMessage = "Complaint was submitted, but the response was invalid.";
                return Page();
            }

            ComplaintId = result.Complaint.ComplaintId;
            SuccessMessage = "Complaint submitted successfully.";

            SpamType = string.Empty;
            SpamNumber = string.Empty;
            Sender = string.Empty;
            Details = string.Empty;

            return Page();
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Unable to connect to the backend API.";
            return Page();
        }
    }

    public class CreateComplaintResponse
    {
        public string Message { get; set; } = string.Empty;
        public ComplaintResponse? Complaint { get; set; }
    }

    public class ComplaintResponse
    {
        public string ComplaintId { get; set; } = string.Empty;
    }
}

