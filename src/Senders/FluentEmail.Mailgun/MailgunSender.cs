using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using FluentEmail.Mailgun.HttpHelpers;

namespace FluentEmail.Mailgun;

public class MailgunSender : ISender {
	private readonly HttpClient _httpClient;

	public MailgunSender(string domainName, string apiKey, MailGunRegion mailGunRegion = MailGunRegion.USA) {
		string url = mailGunRegion switch {
			MailGunRegion.USA => $"https://api.mailgun.net/v3/{domainName}/",
			MailGunRegion.EU => $"https://api.eu.mailgun.net/v3/{domainName}/",
			_ => throw new ArgumentException($"'{mailGunRegion}' is not a valid value for {nameof(mailGunRegion)}"),
		};
		_httpClient = new HttpClient {
			BaseAddress = new Uri(url)
		};

		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{apiKey}")));
	}

	public SendResponse Send(IFluentEmail email, CancellationToken? token = null) => SendAsync(email, token).GetAwaiter().GetResult();

	public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null) {
		if (email.Data.FromAddress == null || string.IsNullOrEmpty(email.Data.FromAddress.EmailAddress)) {
			throw new ArgumentNullException(nameof(email.Data.FromAddress));
		}

		List<KeyValuePair<string, string>> parameters = new() {
			new KeyValuePair<string, string>("from", $"{email.Data.FromAddress.Name} <{email.Data.FromAddress.EmailAddress}>")
		};
		email.Data.ToAddresses.ForEach(x => {
			parameters.Add(new KeyValuePair<string, string>("to", $"{x.Name} <{x.EmailAddress}>"));
		});
		email.Data.CcAddresses.ForEach(x => {
			parameters.Add(new KeyValuePair<string, string>("cc", $"{x.Name} <{x.EmailAddress}>"));
		});
		email.Data.BccAddresses.ForEach(x => {
			parameters.Add(new KeyValuePair<string, string>("bcc", $"{x.Name} <{x.EmailAddress}>"));
		});
		email.Data.ReplyToAddresses.ForEach(x => {
			parameters.Add(new KeyValuePair<string, string>("h:Reply-To", $"{x.Name} <{x.EmailAddress}>"));
		});
		parameters.Add(new KeyValuePair<string, string>("subject", email.Data.Subject ?? string.Empty));

		parameters.Add(new KeyValuePair<string, string>(email.Data.IsHtml ? "html" : "text", email.Data.Body ?? string.Empty));

		if (!string.IsNullOrEmpty(email.Data.PlaintextAlternativeBody)) {
			parameters.Add(new KeyValuePair<string, string>("text", email.Data.PlaintextAlternativeBody));
		}

		email.Data.Tags.ForEach(x => {
			parameters.Add(new KeyValuePair<string, string>("o:tag", x));
		});

		foreach (KeyValuePair<string, string> emailHeader in email.Data.Headers) {
			string key = emailHeader.Key;
			if (!key.StartsWith("h:")) {
				key = "h:" + emailHeader.Key;
			}

			parameters.Add(new KeyValuePair<string, string>(key, emailHeader.Value));
		}

		List<HttpFile> files = new();
		email.Data.Attachments.ForEach(x => {
			string param = x.IsInline ? "inline" : "attachment";
			files.Add(new HttpFile {
				ParameterName = param,
				Data = x.Data,
				Filename = x.Filename,
				ContentType = x.ContentType
			});
		});

		ApiResponse<MailgunResponse> response = await _httpClient.PostMultipart<MailgunResponse>("messages", parameters, files).ConfigureAwait(false);

		SendResponse result = new() { MessageId = response.Data?.Id };
		if (!response.Success) {
			result.ErrorMessages.AddRange(response.Errors.Where(x => !string.IsNullOrWhiteSpace(x.ErrorMessage)).Select(x => x.ErrorMessage!));
			return result;
		}

		return result;
	}
}
