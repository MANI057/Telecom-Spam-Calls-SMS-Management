using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SpamComplaintManagement.Web.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public RegisterModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var client = _httpClientFactory.CreateClient("ApiClient");

        var request = new
        {
            fullName = Input.FullName,
            email = Input.Email,
            password = Input.Password
        };

        try
        {
            var response = await client.PostAsJsonAsync(
                "api/Auth/register",
                request);

            var result =
                await response.Content.ReadFromJsonAsync<ApiResponse>();

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage =
                    result?.Message ?? "Registration successful.";

                ModelState.Clear();

                Input = new RegisterInput();

                return Page();
            }

            ErrorMessage =
                result?.Message ?? "Registration failed.";

            return Page();
        }
        catch (HttpRequestException)
        {
            ErrorMessage =
                "Unable to connect to the API. Make sure the API is running on port 5132.";

            return Page();
        }
    }

    public class RegisterInput
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(
            100,
            MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        public string Email { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    private class ApiResponse
    {
        public string? Message { get; set; }
    }
}