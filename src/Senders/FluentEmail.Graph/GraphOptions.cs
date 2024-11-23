namespace FluentEmail.Graph;

public class GraphOptions {
	public string ClientId { get; set; } = string.Empty;
	public string TenantId { get; set; } = string.Empty;
	public string ClientSecret { get; set; } = string.Empty;
	public bool SaveSentItems { get; set; } = false;
}
