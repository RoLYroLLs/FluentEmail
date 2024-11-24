namespace FluentEmail.Mailgun;

public class MailGunOptions {
	public string DomainName { get; set; } = string.Empty;
	public string ApiKey { get; set; } = string.Empty;
	public MailGunRegion MailGunRegion { get; set; } = MailGunRegion.USA;
}
