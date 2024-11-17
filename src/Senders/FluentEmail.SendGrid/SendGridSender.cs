using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using SendGridAttachment = SendGrid.Helpers.Mail.Attachment;

namespace FluentEmail.SendGrid;

public class SendGridSender : ISendGridSender {
	private readonly string _apiKey;
	private readonly bool _sandBoxMode;

	public SendGridSender(string apiKey, bool sandBoxMode = false) {
		_apiKey = apiKey;
		_sandBoxMode = sandBoxMode;
	}
	public SendResponse Send(IFluentEmail email, CancellationToken? token = null) => SendAsync(email, token).GetAwaiter().GetResult();

	public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null) {
		SendGridMessage mailMessage = await BuildSendGridMessage(email);

		if (email.Data.IsHtml) {
			mailMessage.HtmlContent = email.Data.Body;
		} else {
			mailMessage.PlainTextContent = email.Data.Body;
		}

		if (!string.IsNullOrEmpty(email.Data.PlaintextAlternativeBody)) {
			mailMessage.PlainTextContent = email.Data.PlaintextAlternativeBody;
		}

		SendResponse sendResponse = await SendViaSendGrid(mailMessage, token);

		return sendResponse;
	}

	public async Task<SendResponse> SendWithTemplateAsync(IFluentEmail email, string templateId, object templateData, CancellationToken? token = null) {
		SendGridMessage mailMessage = await BuildSendGridMessage(email);

		mailMessage.SetTemplateId(templateId);
		mailMessage.SetTemplateData(templateData);

		SendResponse sendResponse = await SendViaSendGrid(mailMessage, token);

		return sendResponse;
	}

	private async Task<SendGridMessage> BuildSendGridMessage(IFluentEmail email) {
		if (email.Data.FromAddress == null || string.IsNullOrEmpty(email.Data.FromAddress.EmailAddress)) {
			throw new ArgumentNullException(nameof(email.Data.FromAddress));
		}

		SendGridMessage mailMessage = new();
		mailMessage.SetSandBoxMode(_sandBoxMode);

		mailMessage.SetFrom(ConvertAddress(email.Data.FromAddress));

		if (email.Data.ToAddresses.Any(a => !string.IsNullOrWhiteSpace(a.EmailAddress)))
			mailMessage.AddTos(email.Data.ToAddresses.Select(ConvertAddress).ToList());

		if (email.Data.CcAddresses.Any(a => !string.IsNullOrWhiteSpace(a.EmailAddress)))
			mailMessage.AddCcs(email.Data.CcAddresses.Select(ConvertAddress).ToList());

		if (email.Data.BccAddresses.Any(a => !string.IsNullOrWhiteSpace(a.EmailAddress)))
			mailMessage.AddBccs(email.Data.BccAddresses.Select(ConvertAddress).ToList());

		if (email.Data.ReplyToAddresses.Any(a => !string.IsNullOrWhiteSpace(a.EmailAddress)))
			// SendGrid does not support multiple ReplyTo addresses
			mailMessage.SetReplyTo(email.Data.ReplyToAddresses.Select(ConvertAddress).First());

		mailMessage.SetSubject(email.Data.Subject);

		if (email.Data.Headers.Any()) {
			mailMessage.AddHeaders(email.Data.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
		}

		if (email.Data.Tags.Any()) {
			mailMessage.Categories = email.Data.Tags.ToList();
		}

		if (email.Data.IsHtml) {
			mailMessage.HtmlContent = email.Data.Body;
		} else {
			mailMessage.PlainTextContent = email.Data.Body;
		}

		switch (email.Data.Priority) {
			case Priority.High:
				// https://stackoverflow.com/questions/23230250/set-email-priority-with-sendgrid-api
				mailMessage.AddHeader("Priority", "Urgent");
				mailMessage.AddHeader("Importance", "High");
				// https://docs.microsoft.com/en-us/openspecs/exchange_server_protocols/ms-oxcmail/2bb19f1b-b35e-4966-b1cb-1afd044e83ab
				mailMessage.AddHeader("X-Priority", "1");
				mailMessage.AddHeader("X-MSMail-Priority", "High");
				break;

			case Priority.Normal:
				// Do not set anything.
				// Leave default values. It means Normal Priority.
				break;

			case Priority.Low:
				// https://stackoverflow.com/questions/23230250/set-email-priority-with-sendgrid-api
				mailMessage.AddHeader("Priority", "Non-Urgent");
				mailMessage.AddHeader("Importance", "Low");
				// https://docs.microsoft.com/en-us/openspecs/exchange_server_protocols/ms-oxcmail/2bb19f1b-b35e-4966-b1cb-1afd044e83ab
				mailMessage.AddHeader("X-Priority", "5");
				mailMessage.AddHeader("X-MSMail-Priority", "Low");
				break;
		}

		if (email.Data.Attachments.Any()) {
			foreach (Core.Models.Attachment attachment in email.Data.Attachments) {
				SendGridAttachment sendGridAttachment = await ConvertAttachment(attachment);
				mailMessage.AddAttachment(sendGridAttachment.Filename, sendGridAttachment.Content,
					sendGridAttachment.Type, sendGridAttachment.Disposition, sendGridAttachment.ContentId);
			}
		}

		return mailMessage;
	}

	private async Task<SendResponse> SendViaSendGrid(SendGridMessage mailMessage, CancellationToken? token = null) {
		SendGridClient sendGridClient = new(_apiKey);
		Response sendGridResponse = await sendGridClient.SendEmailAsync(mailMessage, token.GetValueOrDefault());

		SendResponse sendResponse = new();

		if (sendGridResponse.Headers.TryGetValues(
			"X-Message-ID",
			out IEnumerable<string> messageIds)) {
			sendResponse.MessageId = messageIds.FirstOrDefault();
		}

		if (IsHttpSuccess((int)sendGridResponse.StatusCode)) return sendResponse;

		sendResponse.ErrorMessages.Add($"{sendGridResponse.StatusCode}");
		Dictionary<string, dynamic> messageBodyDictionary = await sendGridResponse.DeserializeResponseBodyAsync();

		if (messageBodyDictionary.TryGetValue("errors", out dynamic errors)) {
			foreach (dynamic error in errors) {
				sendResponse.ErrorMessages.Add($"{error}");
			}
		}

		return sendResponse;
	}

	private EmailAddress ConvertAddress(Address address) => new(address.EmailAddress, address.Name);

	private async Task<SendGridAttachment> ConvertAttachment(Core.Models.Attachment attachment) => attachment.Data == null
			? throw new ArgumentNullException(nameof(attachment.Data), "Attachment data cannot be null")
			: new SendGridAttachment {
				Content = await GetAttachmentBase64String(attachment.Data),
				Filename = attachment.Filename,
				Type = attachment.ContentType,
				ContentId = attachment.ContentId,
				Disposition = attachment.IsInline ? "inline" : "attachment",
			};

	private async Task<string> GetAttachmentBase64String(Stream stream) {
		using MemoryStream ms = new();
		await stream.CopyToAsync(ms);
		return Convert.ToBase64String(ms.ToArray());
	}

	private bool IsHttpSuccess(int statusCode) => statusCode is >=200 and <300;
}
