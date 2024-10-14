using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Library_MVC.Data;
using Library_MVC.Models;
using System.Security.Cryptography;

namespace Library_MVC.Controllers
{
	public class AccountController : Controller
	{
		private UserContext _dbContext; 
		
		public AccountController(UserContext context)
		{
			_dbContext = context;

		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginModel model)
		{
			if (ModelState.IsValid)
			{
				UserModel? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
				
				if (user != null)
				{
					string passwordHash = GetHashString(user.Salt + model.Password);

					if (user.PasswordHash == passwordHash)
					{
						await Authenticate(model.Email!);

						return RedirectToAction("Index", "Home");
					}
				}
				ModelState.AddModelError("", "Некорректные логин и(или) пароль");
			}
			return View(model);
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterModel model)
		{
			if (ModelState.IsValid)
			{
				UserModel? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
				if (user == null)
				{
					user = new UserModel();
					user.Salt = GetRandomgSalt();
					user.Email = model.Email;
					user.PasswordHash = GetHashString(user.Salt + model.Password!);

					_dbContext.Users.Add(user);
					await _dbContext.SaveChangesAsync();

					await Authenticate(model.Email!);

					return RedirectToAction("Index", "Home");
				}
				else
					ModelState.AddModelError("", "Аккаунт с таким адресом электронной почты уже существует");
			}
			return View(model);
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login", "Account");
		}

		private async Task Authenticate(string userName)
		{
			// создаем один claim
			var claims = new List<Claim>
			{
				new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
			};
			// создаем объект ClaimsIdentity
			ClaimsIdentity identity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
			// установка аутентификационных куки
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
		}

		public static string GetHashString(string str)
		{
			byte[] data = Convert.FromBase64String(str);
			byte[] hashByte = SHA3_256.HashData(data);
			string hash = Convert.ToBase64String(hashByte);
			return hash;
		}

		public static string GetRandomgSalt()
		{
			var randomNumber = new byte[32];
			string refreshToken = "";

			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);
				refreshToken = Convert.ToBase64String(randomNumber);
			}

			return refreshToken;
		}
	}
}
