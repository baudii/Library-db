using Library_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Web;

namespace Library_MVC.Controllers
{
	public class EmailConfirmationController : Controller
	{
		private UserManager<UserModel> _userManager;
		private IEmailSender _emailSender;

		public EmailConfirmationController(UserManager<UserModel> userManager, IEmailSender emailSender)
		{
			_userManager = userManager;
			_emailSender = emailSender;
		}

		public IActionResult Index()
		{
			return View();
		}
		public IActionResult RegisterConfirmation(string url)
		{
			if (TempData["ErrorMessage"] != null)
			{
				ModelState.AddModelError("", TempData["ErrorMessage"]!.ToString()!);
			}

			return View((object)url);
		}

		public async Task<IActionResult> ConfirmEmail(string? userId, string? code)
		{
			if (userId == null || code == null)
			{
				TempData["ErrorMessage"] = "Некорректная ссылка для подтверждения.";
				return RedirectToAction(nameof(RegisterConfirmation));
			}

			// Найдем пользователя по его идентификатору
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				TempData["ErrorMessage"] = "Пользователь не найден.";
				return RedirectToAction(nameof(RegisterConfirmation));
			}

			// Подтверждаем email
			var result = await _userManager.ConfirmEmailAsync(user, code);
			if (!result.Succeeded)
			{

				// Если подтверждение не удалось, показываем ошибку
				TempData["ErrorMessage"] = "Ошибка при подтверждении электронной почты. Попробуйте позже.";
				return RedirectToAction(nameof(RegisterConfirmation));
			}

			// Если подтверждение прошло успешно, возвращаем соответствующее сообщение
			TempData["SuccessMessage"] = "Ваш электронный адрес был успешно подтвержден!";
			return RedirectToAction(nameof(AccountController.Login), "Account"); // или на страницу логина
		}

		public async Task<IActionResult> SendConfirmation(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				TempData["ErrorMessage"] = "Внутренняя ошибка. Пользователь с таким Id не найден. Повторите попытку позже.";
				return RedirectToAction(nameof(RegisterConfirmation));
			}
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

			var callbackUrl = Url.Action(
				nameof(ConfirmEmail),
				"EmailConfirmation",
				new { userId = user.Id, code },
				protocol: Request.Scheme);

			await _emailSender.SendEmailAsync(
				user.Email!,
				"Подтверждение электронной почты",
				$"Пожалуйста, подтвердите вашу учетную запись, перейдя по <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>ссылке</a>.");

			return RedirectToAction(nameof(RegisterConfirmation), new { url = callbackUrl });
		}

	}
}
