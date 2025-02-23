﻿using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using Microsoft.Exchange.WebServices.Data;

namespace FluentEmail.Exchange;

public class ExchangeSender : ISender {
	private readonly ExchangeService _meExchangeClient;

	public ExchangeSender(ExchangeService paExchangeClient) => _meExchangeClient = paExchangeClient;

	public SendResponse Send(IFluentEmail email, CancellationToken? token = null) => System.Threading.Tasks.Task.Run(() => SendAsync(email, token)).Result;

	public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null) {
		SendResponse response = new();

		EmailMessage message = CreateMailMessage(email);

		if (token?.IsCancellationRequested ?? false) {
			response.ErrorMessages.Add("Message was cancelled by cancellation token.");
			return response;
		}

		message.Save();
		message.SendAndSaveCopy();

		return response;
	}

	private EmailMessage CreateMailMessage(IFluentEmail paEmail) {
		EmailData paData = paEmail.Data;

		EmailMessage loExchangeMessage = new(_meExchangeClient) {
			Subject = paData.Subject,
			Body = paData.Body,
		};

		if (!string.IsNullOrEmpty(paData.FromAddress?.EmailAddress))
			loExchangeMessage.From = new EmailAddress(paData.FromAddress.Name, paData.FromAddress.EmailAddress);

		paData.ToAddresses.ForEach(x => {
			loExchangeMessage.ToRecipients.Add(new EmailAddress(x.Name, x.EmailAddress));
		});

		paData.CcAddresses.ForEach(x => {
			loExchangeMessage.CcRecipients.Add(new EmailAddress(x.Name, x.EmailAddress));
		});

		paData.BccAddresses.ForEach(x => {
			loExchangeMessage.BccRecipients.Add(new EmailAddress(x.EmailAddress, x.Name));
		});

		switch (paData.Priority) {
			case Priority.Low:
				loExchangeMessage.Importance = Importance.Low;
				break;

			case Priority.Normal:
				loExchangeMessage.Importance = Importance.Normal;
				break;

			case Priority.High:
				loExchangeMessage.Importance = Importance.High;
				break;
		}

		paData.Attachments.ForEach(x => {
			//System.Net.Mail.Attachment a = new System.Net.Mail.Attachment(x.Data, x.Filename, x.ContentType);
			//a.ContentId = x.ContentId;
			loExchangeMessage.Attachments.AddFileAttachment(x.Filename, x.Data);
		});

		return loExchangeMessage;
	}
}
