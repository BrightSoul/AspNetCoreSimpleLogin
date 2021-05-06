using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimplePass.Models;

namespace SimplePass.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl, [FromServices] IConfiguration configuration)
        {
            // Sanitizziamo lo username (solo lettere e numeri ammessi)
            username = new Regex("[^a-zA-Z0-9]").Replace(username, string.Empty);
            // Otteniamo il salt e l'hash della password dalla configurazione
            string salt = configuration.GetValue<string>($"Users:{username}:Salt");
            string passwordHash = configuration.GetValue<string>($"Users:{username}:PasswordHash");

            if (salt is not null or "" && passwordHash is not null or "")
            {
                // La password è corretta se il suo hash corrisponde a quello trovato in configurazione
                using var deriveBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(salt), configuration.GetValue<int>("PasswordIterations"), HashAlgorithmName.SHA256);
                byte[] bytes = deriveBytes.GetBytes(256 / 8);
                string inputPasswordHash = Convert.ToBase64String(bytes);

                if (passwordHash == inputPasswordHash)
                {
                    // Creo la mia ClaimsPrincipal. Ci metto dentro giusto il claim del nome
                    Claim nameClaim = new Claim(ClaimTypes.Name, username);
                    ClaimsIdentity identity = new ClaimsIdentity(new[] { nameClaim }, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                    // Emetto il cookie di autenticazione
                    await HttpContext.SignInAsync(principal);

                    // Ritorno alla pagina da cui sono venuto
                    return LocalRedirect(returnUrl);
                }
            }

            ModelState.AddModelError("password", "I dati di login non sono esatti");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult AdminPanel()
        {
            return View();
        }
    }
}
