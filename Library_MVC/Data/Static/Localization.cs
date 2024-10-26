namespace Library_MVC
{
	public static class Localization
	{
		public static string GetRuIdentityError(string codeError)
		{
			string message = string.Empty;
			switch (codeError)
			{
				case "DefaultError":
					message = "Произошла неизвестная ошибка.";
					break;
				case "Сбой параллелизма. Объект был изменён":
					message = "Конфликт параллелизма.";
					break;
				case "InvalidToken":
					message = "Недействительный токен.";
					break;
				case "LoginAlreadyAssociated":
					message = "Пользователь с таким логином уже существует.";
					break;
				case "InvalidUserName":
					message = "Неверный логин, логин должен содержать только буквы или цифры.";
					break;
				case "InvalidEmail":
					message = "Недействительный адрес электронной почты.";
					break;
				case "DuplicateUserName":
					message = "Этот логин уже используется.";
					break;
				case "DuplicateEmail":
					message = "Этот адрес электронной почты уже используется.";
					break;
				case "InvalidRoleName":
					message = "Недопустимая роль.";
					break;
				case "DuplicateRoleName":
					message = "Эта роль уже используется.";
					break;
				case "UserAlreadyInRole":
					message = "Пользователь уже имеет эту роль.";
					break;
				case "UserNotInRole":
					message = "Пользователь не имеет этой роли.";
					break;
				case "UserLockoutNotEnabled":
					message = "Блокировка для этого пользователя не включена.";
					break;
				case "UserAlreadyHasPassword":
					message = "У пользователя уже установлена пароль.";
					break;
				case "PasswordMismatch":
					message = "Неверный пароль.";
					break;
				case "PasswordTooShort":
					message = "Пароль слишком короткий.";
					break;
				case "PasswordRequiresNonAlphanumeric":
					message = "Пароль должен содержать хотя бы один неалфавитный символ.";
					break;
				case "PasswordRequiresDigit":
					message = "Пароль должен содержать хотя бы одну цифру ('0'-'9').";
					break;
				case "PasswordRequiresLower":
					message = "Пароль должен содержать хотя бы одну строчную букву ('a'-'z').";
					break;
				case "PasswordRequiresUpper":
					message = "Пароль должен содержать хотя бы одну заглавную букву ('A'-'Z').";
					break;
				default:
					message = "Произошла неизвестная ошибка.";
					break;
			}
			return message;
		}

		public static string GetRuBookColumnName(string columnName)
		{
			switch (columnName)
			{
				case "Id":
					return "Id";
				case "Author":
					return "Автор";
				case "Title":
					return "Название";
				case "Published Year":
					return "Год публикации";
				case "Genre":
					return "Жанр";
			}
			return columnName;
		}
	}
}
