using Microsoft.AspNetCore.Mvc;
using Library_MVC.Data;
using Library_MVC.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Library_MVC.Data.Static;

namespace Library_MVC.Controllers
{
    public class AccountController : Controller
	{
		public const string AdminRole = "Admin";
		public const string ModeratorRole = "Moderator";
		public const string MemberRole = "Member";

		private AuthDbContext _dbContext; 
		private UserManager<UserModel> _userManager;
		private SignInManager<UserModel> _signInManager;

		public AccountController(
				UserManager<UserModel> userManager, 
				SignInManager<UserModel> signInManager, 
				AuthDbContext context)
		{
			_dbContext = context;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public IActionResult Index()
		{
			if (User.Identity != null && User.Identity.IsAuthenticated)
				return RedirectToAction(nameof(Details));

			return RedirectToAction(nameof(Login));
		}
		public IActionResult AccessDenied()
		{
			return View();
		}

		public IActionResult Error()
		{
			return View();
		}

		#region Login

		public IActionResult Login()
		{
			if (User.Identity != null && User.Identity.IsAuthenticated)
				return RedirectToAction(nameof(Details));


			if (TempData["SuccessMessage"] != null)
			{
				ViewBag.Message = "Аккаунт подтвержден!";
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email!);
				
				if (user != null)
				{
					var result = await _signInManager.PasswordSignInAsync(user, model.Password!, isPersistent: false, lockoutOnFailure: false); 

					if (result.Succeeded)
						return RedirectToAction(nameof(Index));
					else if (result.IsNotAllowed)
						return RedirectToAction(nameof(EmailConfirmationController.SendConfirmation), "EmailConfirmation", new { userId = user.Id });
					else if (result.IsLockedOut)
						ModelState.AddModelError("", "Ваш аккаунт заблокирован.");
					else
						ModelState.AddModelError("", "Неверный пароль.");

					return View(model);
				}

				ModelState.AddModelError("", "Некорректные логин и (или) пароль");
			}
			return View(model);
		}

		#endregion

		#region Register

		public IActionResult Register()
		{
			if (User.Identity != null && User.Identity.IsAuthenticated)
				return RedirectToAction(nameof(Details));
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterModel model)
		{
			if (!ModelState.IsValid)
				return View();
			
			var task = _userManager.FindByEmailAsync(model.Email!);
			var user = task.Result;

			if (user != null)
			{
				ModelState.AddModelError("", "Аккаунт с таким адресом электронной почты уже существует");
				return View();
			}
			// Password12@
			user = new UserModel();
			user.Email = model.Email;
			user.UserName = model.UserName;
			user.PhoneNumber = model.PhoneNumber;
			user.FirstName = model.FirstName;
			user.LastName = model.LastName;

			IdentityResult result = await _userManager.CreateAsync(user, model.Password!);

			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", Localization.GetRuIdentityError(error.Code));
				}
				return View(model);
			}

			await _userManager.AddToRoleAsync(user, "Member");

			if (_userManager.Options.SignIn.RequireConfirmedAccount)
			{
				return RedirectToAction(nameof(EmailConfirmationController.SendConfirmation), "EmailConfirmation", new {userId = user.Id});
			}
			else
			{
				await _signInManager.SignInAsync(user, isPersistent: false);
				return RedirectToAction(nameof(Index));
			}
		}

		#endregion

		#region Manage

		[HttpGet]
		public async Task<IActionResult> Details()
		{
			// Получаем текущего пользователя
			var user = await _userManager.FindByNameAsync(User.Identity!.Name!);

			if (user == null)
			{
				ModelState.AddModelError("", "Пользователь не найден");
				return View();
			}

			// Передаем данные пользователя в представление
			var model = new UserModel()
			{
				UserName = user.UserName,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber
			};

			var roles = await _userManager.GetRolesAsync(user);
			var userRole = roles.FirstOrDefault();
			if (userRole == null)
			{
				var result = await _userManager.AddToRoleAsync(user, "Member");
				if (result.Succeeded)
					userRole = "Member";
				else
					userRole = "Нет роли";
			}
			ViewBag.UserRole = userRole;

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Details(EditModel model)
		{
			var user = await _userManager.FindByNameAsync(model.UserName!);
			if (user == null)
			{
				ModelState.AddModelError("", $"Что-то пошло не так. Пользователь с ником {model.UserName} не найден");
				return View(new UserModel
				{
					Email = model.Email,
					FirstName = model.FirstName,
					LastName = model.LastName,
					PhoneNumber = model.PhoneNumber,
					UserName = model.UserName
				});
			}

			user.Email = model.Email;
			user.FirstName = model.FirstName;
			user.LastName = model.LastName;
			user.PhoneNumber = model.PhoneNumber;
			user.UserName = model.UserName;

			var result = await _userManager.UpdateAsync(user);

			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", Localization.GetRuIdentityError(error.Code));
				}
				return View(user);
			}

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction(nameof(Login));
		}

		#endregion

		#region Depricated

		public static string GetHashString(string str)
		{
			byte[] data = Convert.FromBase64String(str);
			byte[] hashByte = SHA3_256.HashData(data);
			string hash = Convert.ToBase64String(hashByte);
			return hash;
		}

		private static string GetRandomgSalt()
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

		#endregion
	}
}
