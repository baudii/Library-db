using System.ComponentModel.DataAnnotations;

namespace Library_MVC.Models
{
	public class RegisterModel
	{
		[Required(ErrorMessage = "Не указано имя пользователя!")]
		[StringLength(20, MinimumLength = 5, ErrorMessage = "Поле \"Имя пользователя\" должно быть строкой длиной не менее 5 и не более 20 символов.")]
		public string? UserName { get; set; }

		[Phone]
		public string? PhoneNumber { get; set; }

		[DataType(DataType.Text)]
		public string? FirstName { get; set; }


		[DataType(DataType.Text)]
		public string? LastName { get; set; }

		[Required(ErrorMessage = "Не указан Email")]
		[EmailAddress(ErrorMessage = "Введите корректный Email")]
		public string? Email { get; set; }

		[Required(ErrorMessage = "Не указан пароль")]
		[DataType(DataType.Password)]
		public string? Password { get; set; }

		[Compare("Password", ErrorMessage = "Пароли должны совпадать")]
		public string? ConfirmPassword { get; set; }
	}
}
