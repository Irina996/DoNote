using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Security.Claims;
using WebNoteClient.Models.Auth;
using WebNoteClient.Services;

namespace WebNoteClient.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService) 
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var authResult = await _authService.LoginAsync(model);
            if (!authResult.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password. Please try again.");
                return View(model);
            }

            await SignIn(authResult);

            return RedirectToAction("Index", "Note");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var authResult = await _authService.RegisterAsync(model);
            if (!authResult.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password. Please try again.");
                return View(model);
            }

            await SignIn(authResult);

            return RedirectToAction("Index", "Note");
        }

        private async Task SignIn(AuthResult authResult)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, authResult.Email),
                new Claim("AccessToken", authResult.Token),
            };

            var claimsIdentity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
                ExpiresUtc = authResult.Expires,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}
