using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Library_MVC.Models.Attributes
{
    public class PasswordValidationAttribute : ValidationAttribute
    {
        const string passwordLengthRegex = "^.{8,}$"; // Должен содержать минимум 8 символов
        const string passwordUpperRegex = "(?=.*?[A-Z])"; // Должен содержать хотя бы одну заглавную букву
        const string passwordLowerRegex = "(?=.*?[a-z])"; // Должен содержать хотя бы одну строчную букву
        const string passwordDigitRegex = "(?=.*?[0-9])"; // Должен содержать хотя бы одну цифру
        const string passwordSpecialCharRegex = "(?=.*?[#?!@$%^&*-])"; // Должен содержать хотя бы один специальный символ

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var password = value as string;

            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Пароль не может быть пустым.");
            }

            if (!Regex.IsMatch(password, passwordLengthRegex))
            {
                return new ValidationResult("Пароль должен содержать минимум 8 символов.");
            }

            if (!Regex.IsMatch(password, passwordUpperRegex))
            {
                return new ValidationResult("Пароль должен содержать хотя бы одну заглавную букву.");
            }

            if (!Regex.IsMatch(password, passwordLowerRegex))
            {
                return new ValidationResult("Пароль должен содержать хотя бы одну строчную букву.");
            }

            if (!Regex.IsMatch(password, passwordDigitRegex))
            {
                return new ValidationResult("Пароль должен содержать хотя бы одну цифру.");
            }

            if (!Regex.IsMatch(password, passwordSpecialCharRegex))
            {
                return new ValidationResult("Пароль должен содержать хотя бы один специальный символ.");
            }

            return ValidationResult.Success!;
        }
    }

}
