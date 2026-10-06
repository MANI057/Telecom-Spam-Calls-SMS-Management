
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SpamComplaintManagement.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Email and password are required.";
            return Page();
        }

        var client = _httpClientFactory.CreateClient("ApiClient");

        var request = new
        {
            email = Email.Trim(),
            password = Password
        };

        try
        {
            var response = await client.PostAsJsonAsync(
                "api/Auth/login",
                request);

            var responseBody =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            var result =
                System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(
                    responseBody,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (result == null || result.User == null)
            {
                ErrorMessage = "Login response is invalid.";
                return Page();
            }

            HttpContext.Session.SetInt32(
                "UserId",
                result.User.Id);

            HttpContext.Session.SetString(
                "UserName",
                result.User.FullName);

            HttpContext.Session.SetString(
                "UserEmail",
                result.User.Email);

            return RedirectToPage("/Dashboard");
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Unable to connect to the backend API.";
            return Page();
        }
    }

    public class LoginResponse
    {
        public string Message { get; set; } = string.Empty;

        public LoginUser? User { get; set; }
    }

    public class LoginUser
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}

