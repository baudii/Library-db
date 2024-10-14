using System.ComponentModel.DataAnnotations;

namespace Library_MVC.Models
{
	public class LoginModel
	{
		[Required(ErrorMessage = "Не указан Email")]
		[DataType(DataType.EmailAddress, ErrorMessage = "Пожалуйста, введите корректный адрес электронной почты")]
		public string? Email { get; set; }

		[Required(ErrorMessage = "Не указан пароль")]
		[DataType(DataType.Password)]
		public string? Password { get; set; }
	}
}
