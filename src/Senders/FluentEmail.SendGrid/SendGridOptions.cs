namespace FluentEmail.SendGrid;

public class SendGridOptions {
	public string ApiKey { get; set; } = string.Empty;
	public string? Host { get; set; } = null;
	public bool SandBoxMode { get; set; } = false;
}
