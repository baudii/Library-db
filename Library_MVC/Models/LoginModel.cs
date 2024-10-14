using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Library_MVC.Models.Attributes;

namespace Library_MVC.Models
{
	public class LoginModel
	{
		[EmailAddress(ErrorMessage = "Введите корректный Email")]
		[Required(ErrorMessage = "Не указан Email")]
		public string? Email { get; set; }

		[PasswordValidation(ErrorMessage = "Пароль не соответствует требованиям.")]
		[Required(ErrorMessage = "Не указан пароль")]
		public string? Password { get; set; }
	}
}
