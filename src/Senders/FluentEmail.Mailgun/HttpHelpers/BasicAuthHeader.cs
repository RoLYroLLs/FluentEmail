using System;
using System.Net.Http.Headers;

namespace FluentEmail.Mailgun.HttpHelpers;

public static class BasicAuthHeader {
	public static AuthenticationHeaderValue GetHeader(string username, string password) => new("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}")));
}
