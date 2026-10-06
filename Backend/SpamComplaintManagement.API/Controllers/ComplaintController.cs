using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SpamComplaintManagement.Business.Interfaces;

namespace SpamComplaintManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComplaintController : ControllerBase
{
    private readonly IComplaintService _complaintService;

    public ComplaintController(IComplaintService complaintService)
    {
        _complaintService = complaintService;
    }

    [HttpPost]
    [HttpGet("user/{userId:int}")]
    public IActionResult Create([FromBody] CreateComplaintRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = _complaintService.Create(
            request.UserId,
            request.SpamType,
            request.SpamNumber,
            request.Sender,
            request.Details);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new
        {
            message = result.Message,
            complaint = result.Complaint
        });
    }

    [HttpGet("user/{userId:int}")]
    public IActionResult GetForUser(int userId)
    {
        if (userId <= 0)
            return BadRequest(new { message = "Invalid user ID." });

        return Ok(_complaintService.GetForUser(userId));
    }

    [HttpGet("{complaintId}")]
    public IActionResult Get(string complaintId, [FromQuery] int userId)
    {
        if (string.IsNullOrWhiteSpace(complaintId) || userId <= 0)
            return BadRequest(new { message = "Complaint ID and valid user ID are required." });

        var complaint = _complaintService.Get(complaintId, userId);

        if (complaint == null)
            return NotFound(new { message = "Complaint not found." });

        return Ok(complaint);
    }
}

public class CreateComplaintRequest
{
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }

    [Required]
    public string SpamType { get; set; } = "";

    [Required]
    public string SpamNumber { get; set; } = "";

    [Required]
    public string Sender { get; set; } = "";

    [Required]
    public string Details { get; set; } = "";
}
