namespace FluentEmail.Mailtrap;

public class MailtrapOptions {
	public string UserName { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public string? Host { get; set; } = null;
	public int? Port { get; set; } = null;
}
