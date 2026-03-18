using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using SecureTaskHub.Data;
using SecureTaskHub.Models;
using System.ComponentModel.DataAnnotations;

public class RegisterModel : PageModel
{
    private readonly AppDbContext _context;

    public RegisterModel(AppDbContext context) => _context = context;

    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public class RegisterInput // Bu aslýnda senin RegisterDTO'n!
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

        return RedirectToPage("Login");
    }
}