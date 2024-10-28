using System.ComponentModel.DataAnnotations;

namespace Library_MVC.Models
{
	public class UserViewModel
	{
		[Required]
		[Display(Name = "Имя пользователя")]
		public string? UserName { get; set; }

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string? Email { get; set; }

		[Display(Name = "Номер телефона")]
		public string? PhoneNumber { get; set; }

		[Display(Name = "Имя")]
		public string? FirstName { get; set; }

		[Display(Name = "Фамилия")]
		public string? LastName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string? Password { get; set; }
	}
}
