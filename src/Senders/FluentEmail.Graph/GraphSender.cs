using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;

namespace FluentEmail.Graph;

public class GraphSender : ISender {
	private readonly bool _saveSent;
	private readonly GraphServiceClient _graphClient;

	public GraphSender(GraphOptions options) : this(options.TenantId, options.ClientId, options.ClientSecret, options.SaveSentItems) { }

	public GraphSender(string clientId, string tenantId, string clientSecret, bool saveSentItems) {
		_saveSent = saveSentItems;

		TokenCredentialOptions tokenOptions = new() {
			AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
		};

		ClientSecretCredential clientSecretCredential = new(tenantId, clientId, clientSecret, tokenOptions);
		_graphClient = new GraphServiceClient(clientSecretCredential);
	}

	public SendResponse Send(IFluentEmail email, CancellationToken? token = null) => SendAsync(email, token).GetAwaiter().GetResult();

	public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null) {
		if (email.Data.FromAddress == null || string.IsNullOrEmpty(email.Data.FromAddress.EmailAddress)) {
			throw new ArgumentNullException(nameof(email.Data.FromAddress));
		}

		Message message = new() {
			Subject = email.Data.Subject,
			Body = new ItemBody {
				Content = email.Data.Body,
				ContentType = email.Data.IsHtml ? BodyType.Html : BodyType.Text
			},
			From = new Recipient {
				EmailAddress = new EmailAddress {
					Address = email.Data.FromAddress.EmailAddress,
					Name = email.Data.FromAddress.Name
				}
			}
		};

		if (email.Data.ToAddresses is { Count: > 0 }) {
			List<Recipient> toRecipients = new();

			email.Data.ToAddresses.ForEach(r => {
				if (!string.IsNullOrWhiteSpace(r.EmailAddress)) {
					toRecipients.Add(new Recipient {
						EmailAddress = new EmailAddress {
							Address = r.EmailAddress.ToString(),
							Name = r.Name
						}
					});
				}
			});

			message.ToRecipients = toRecipients;
		}

		if (email.Data.BccAddresses is { Count: > 0 }) {
			List<Recipient> bccRecipients = new();

			email.Data.BccAddresses.ForEach(r => {
				if (!string.IsNullOrWhiteSpace(r.EmailAddress)) {
					bccRecipients.Add(new Recipient {
						EmailAddress = new EmailAddress {
							Address = r.EmailAddress.ToString(),
							Name = r.Name
						}
					});
				}
			});

			message.BccRecipients = bccRecipients;
		}

		if (email.Data.CcAddresses is { Count: > 0 }) {
			List<Recipient> ccRecipients = new();

			email.Data.CcAddresses.ForEach(r => {
				if (!string.IsNullOrWhiteSpace(r.EmailAddress)) {
					ccRecipients.Add(new Recipient {
						EmailAddress = new EmailAddress {
							Address = r.EmailAddress.ToString(),
							Name = r.Name
						}
					});
				}
			});

			message.CcRecipients = ccRecipients;
		}

		if (email.Data.Attachments is { Count: > 0 }) {
			List<Microsoft.Graph.Models.Attachment> attachments = new();

			email.Data.Attachments.ForEach(a => {
				if (a.Data != null) {
					attachments.Add(new FileAttachment {
						Name = a.Filename,
						ContentType = a.ContentType,
						ContentBytes = GetAttachmentBytes(a.Data)
					});
				}
			});

			message.Attachments = attachments;
		}

		message.Importance =email.Data.Priority switch {
			Priority.High => Importance.High,
			Priority.Normal => Importance.Normal,
			Priority.Low => Importance.Low,
			_ => Importance.Normal,
		};

		try {
			SendMailPostRequestBody body = new() { Message = message, SaveToSentItems = _saveSent };

			await _graphClient.Users[email.Data.FromAddress.EmailAddress]
				.SendMail
				.PostAsync(body);

			return new SendResponse {
				MessageId = message.Id
			};
		} catch (Exception ex) {
			return new SendResponse {
				ErrorMessages = new List<string> { ex.Message }
			};
		}
	}

	private static byte[] GetAttachmentBytes(Stream stream) {
		using MemoryStream m = new();
		stream.CopyTo(m);
		return m.ToArray();
	}
}
