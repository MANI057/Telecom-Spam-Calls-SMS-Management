using System.Net.Http.Json;
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

    public async Task OnGetAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("Api");

            var response = await client.GetAsync("api/Complaints");

            if (response.IsSuccessStatusCode)
            {
                Complaints =
                    await response.Content.ReadFromJsonAsync<List<ComplaintViewModel>>()
                    ?? new List<ComplaintViewModel>();
            }
            else
            {
                ErrorMessage =
                    $"Unable to load complaints. API returned {response.StatusCode}.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading complaints: {ex.Message}";
        }
    }
}

public class ComplaintViewModel
{
    public int ComplaintId { get; set; }

    public string? SpamType { get; set; }

    public string? SpamNumber { get; set; }

    public string? Sender { get; set; }

    public string? Status { get; set; }
}