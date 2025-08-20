using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebNotes.Data;
using WebNotes.Entities;
using WebNotes.Models;

namespace WebNotes.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            ApplicationDbContext context) {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model) {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return BadRequest(new { message = "User with such email already exists" });
            }
            // use email as username
            var user = new IdentityUser {UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var defaultCategory = new Category { Name = "Uncategorized", UserId = user.Id };
                _context.Add(defaultCategory);
                await _context.SaveChangesAsync();
                return Ok(GenerateJwtToken(user));
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model) {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, false, false);

            if (result.Succeeded) {
                var user = await _userManager.FindByEmailAsync(model.Email);
                return Ok(GenerateJwtToken(user));
            }

            return Unauthorized();
        }

        /// Generates a JWT (JSON Web Token) for the authenticated user
        private object GenerateJwtToken(IdentityUser user) 
        {
            // Create a JWT token handler to encode and decode tokens
            var tokenHandler = new JwtSecurityTokenHandler();

            // Retrieve the secret key from configuration and convert it to bytes
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                // Claims represent the user's identity and permissions
                Subject = new ClaimsIdentity(new[]
                {
                    // 'sub' (Subject): Unique identifier for the user (user ID)
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),

                    // 'jti' (JWT ID): Unique identifier for the token (helps prevent replay attacks)
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                    // Standard claim for the user's name (username)
                    new Claim(ClaimTypes.Name, user.UserName)
                }),

                // Token expiration: 7 days from now
                Expires = DateTime.UtcNow.AddDays(7),

                // Signing credentials: use symmetric key and HMAC-SHA256 algorithm
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Create the JWT token based on the descriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Return the token as a string, along with username and expiration time
            return new {
                token = tokenHandler.WriteToken(token),
                username = user.UserName,
                expires = token.ValidTo
            };
        }
    }
}
