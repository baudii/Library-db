using System.ComponentModel.DataAnnotations;

namespace Library_MVC.Models
{
	public class EditModel
	{
		[Required(ErrorMessage = "Не указано имя пользователя!")]
		[StringLength(20, MinimumLength = 5)]
		public string? UserName { get; set; }

		[Required(ErrorMessage = "Не указан Email")]
		[EmailAddress(ErrorMessage = "Введите корректный Email")]
		public string? Email { get; set; }

		[Phone]
		public string? PhoneNumber { get; set; }

		[DataType(DataType.Text)]
		public string? FirstName { get; set; }

		[DataType(DataType.Text)]
		public string? LastName { get; set; }
	}
}
