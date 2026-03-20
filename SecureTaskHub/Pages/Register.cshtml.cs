using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using SecureTaskHub.Data;
using SecureTaskHub.Models;
using System.ComponentModel.DataAnnotations;

public class RegisterModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly RabbitMQService _rabbitMQService;
    public RegisterModel(AppDbContext context, RabbitMQService rabbitMQService)
    {
        _context = context;
        _rabbitMQService = rabbitMQService;
    }
    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public class RegisterInput 
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var user = new User
        {
            Username = Input.Username,
            Email = Input.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(Input.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var message = new { UserId = user.Id, Email = user.Email, Name = user.Username };
        await _rabbitMQService.SendMessageAsync(message, "user_registered_queue");
        return RedirectToPage("Login");
    }
}