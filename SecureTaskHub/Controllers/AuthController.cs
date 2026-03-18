using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SecureTaskHub.Data;
using SecureTaskHub.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SecureTaskHub.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Merhaba!");
        }
        [HttpGet("all-with-tasks")]
        public IActionResult GetUsersWithTasks()
        {
            var users = _context.Users.Include(u => u.Tasks).ToList();

            return Ok(users);
        }
        [HttpPost("register")]
        public IActionResult Register(UserRegisterDto request)
        {
            if (_context.Users.Any(u => u.Email == request.Email))
            {
                return BadRequest("Bu email zaten kayıtlı.");
            }
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("Kayıt başarılı!");
        }
        [HttpPost("login")]
        public IActionResult Login(UserLoginDto request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if(user == null)
            {
                return BadRequest("Kullanıcı bulunamadı");
            }
            if(!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Hatalı şifre");
            }
            return Ok("Giriş başarılı!");
        }
        private string CreateToken(User user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.Username)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}