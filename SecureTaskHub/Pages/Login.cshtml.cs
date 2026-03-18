using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecureTaskHub.Data; // AppDbContext için
using System.ComponentModel.DataAnnotations;

namespace SecureTaskHub.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;

        // Constructor ile veritabanýný içeri alýyoruz
        public LoginModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoginInput Input { get; set; } = new();

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public class LoginInput
        {
            [Required(ErrorMessage = "E-posta gerekli.")]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Þifre gerekli.")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // 1. ADIM: Veritabanýnda kullanýcýyý bul
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Input.Email);

            // 2. ADIM: Kullanýcý var mý ve þifre doðru mu?
            if (user == null || !BCrypt.Net.BCrypt.Verify(Input.Password, user.PasswordHash))
            {
                // Hatalýysa "Giriþ baþarýlý" demesin, hata versin
                ErrorMessage = "E-posta veya þifre hatalý!";
                SuccessMessage = null;
                return Page();
            }

            // 3. ADIM: Eðer buraya geldiyse artýk gerçekten baþarýlýdýr
            SuccessMessage = $"Hoþ geldin, {user.Username}!";
            ErrorMessage = null;

            // Burada istersen kullanýcýyý ana sayfaya yönlendirirsin:
            // return RedirectToPage("/Index");

            return Page();
        }
    }
}