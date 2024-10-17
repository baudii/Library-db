using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Library_MVC.Models
{
	public class LoginModel
	{
		[EmailAddress(ErrorMessage = "Введите корректный Email")]
		[Required(ErrorMessage = "Не указан Email")]
		public string? Email { get; set; }

		[Required(ErrorMessage = "Не указан пароль")]
		[DataType(DataType.Password)]
		public string? Password { get; set; }
	}
}
