using System.ComponentModel.DataAnnotations;

namespace Library_MVC.Models
{
	public class EditUserViewModel
	{
		public string? Id { get; set; }

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

		[DataType(DataType.Password)]
		[Display(Name = "Новый пароль")]
		public string? NewPassword { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Новая роль")]
		public string? NewRole { get; set; }
	}
}
