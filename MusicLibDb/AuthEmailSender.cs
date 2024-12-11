using Microsoft.AspNetCore.Identity.UI.Services;

namespace Library_MVC
{
	public class AuthEmailSender : IEmailSender
	{
		public Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			Console.WriteLine($"Sending email to {email} with subject: {subject}");
			return Task.CompletedTask;
		}
	}
}