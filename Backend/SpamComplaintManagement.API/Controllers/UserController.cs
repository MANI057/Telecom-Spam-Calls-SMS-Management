using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpamComplaintManagement.Data.Data;
using SpamComplaintManagement.Data.Models;

namespace SpamComplaintManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public UserController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET: api/User
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _db.LoginCredentials
            .AsNoTracking()
            .Select(x => new
            {
                id = x.Id,
                fullName = x.FullName,
                email = x.Email,
                createdAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    // GET: api/User/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _db.LoginCredentials
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new
            {
                id = x.Id,
                fullName = x.FullName,
                email = x.Email,
                createdAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        return Ok(user);
    }

    // PUT: api/User/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(
        int id,
        [FromBody] UpdateUserRequest request)
    {
        var user = await _db.LoginCredentials
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return BadRequest(new
            {
                message = "Full name is required."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new
            {
                message = "Email is required."
            });
        }

        var email = request.Email.Trim().ToLowerInvariant();

        var emailAlreadyUsed = await _db.LoginCredentials
            .AnyAsync(x =>
                x.Id != id &&
                x.Email == email);

        if (emailAlreadyUsed)
        {
            return Conflict(new
            {
                message = "This email is already used by another user."
            });
        }

        user.FullName = request.FullName.Trim();
        user.Email = email;

        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "User updated successfully.",
            user = new
            {
                id = user.Id,
                fullName = user.FullName,
                email = user.Email,
                createdAt = user.CreatedAt
            }
        });
    }

    // DELETE: api/User/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _db.LoginCredentials
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        _db.LoginCredentials.Remove(user);

        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "User deleted successfully."
        });
    }
}

public class UpdateUserRequest
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}