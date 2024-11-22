using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace FluentEmail.Graph.Tests;

public class Tests {
	//TODO: For these tests to pass you will need to supply the following details from an Azure AD / Office 365 Tenant
	public const string AppId = ""; //Add your AAD Graph App ID here
	public const string TenantId = ""; //Add your AAD Tenant ID here
	public const string GraphSecret = ""; //Add your AAD Graph Client Secret here
	public const string SenderEmail = ""; //Add a sender email address from your Office 365 tenant
	public const string ToEmail = "fluentemail@mailinator.com"; //change this if you like
	private const bool SaveSent = false;

	[SetUp]
	public void Setup() {
		if (string.IsNullOrWhiteSpace(AppId)) throw new ArgumentException("Graph App ID needs to be supplied");
		if (string.IsNullOrWhiteSpace(TenantId)) throw new ArgumentException("Graph tenant ID needs to be supplied");
		if (string.IsNullOrWhiteSpace(GraphSecret)) throw new ArgumentException("Graph client secret needs to be supplied");
		if (string.IsNullOrWhiteSpace(SenderEmail)) throw new ArgumentException("Sender email address needs to be supplied");

		GraphSender sender = new(AppId, TenantId, GraphSecret, SaveSent);

		Email.DefaultSender = sender;
	}

	[Test, Ignore("Missing Graph credentials")]
	public void CanSendEmail() {
		IFluentEmail email = Email
			.From(SenderEmail)
			.To(ToEmail)
			.Subject("Test Email")
			.Body("Test email from Graph sender unit test");

		SendResponse response = email.Send();

		Assert.That(response.Successful, Is.True);
	}

	[Test, Ignore("Missing Graph credentials")]
	public async Task CanSendEmailAsync() {
		IFluentEmail email = Email
			.From(SenderEmail)
			.To(ToEmail)
			.Subject("Test Async Email")
			.Body("Test email from Graph sender unit test");

		SendResponse response = await email.SendAsync();

		Assert.That(response.Successful, Is.True);
	}

	[Test, Ignore("Missing Graph credentials")]
	public async Task CanSendEmailWithAttachments() {
		MemoryStream stream = new();
		StreamWriter sw = new(stream);
		await sw.WriteLineAsync("Hey this is some text in an attachment");
		await sw.FlushAsync();
		stream.Seek(0, SeekOrigin.Begin);

		Attachment attachment = new() {
			ContentType = "text/plain",
			Filename = "graphtest.txt",
			Data = stream
		};

		IFluentEmail email = Email
			.From(SenderEmail)
			.To(ToEmail)
			.Subject("Test Email with Attachments")
			.Body("Test email from Graph sender unit test")
			.Attach(attachment);

		SendResponse response = await email.SendAsync();

		Assert.That(response.Successful, Is.True);
	}

	[Test, Ignore("Missing Graph credentials")]
	public async Task CanSendHighPriorityEmail() {
		IFluentEmail email = Email
			.From(SenderEmail)
			.To(ToEmail)
			.Subject("Test High Priority Email")
			.Body("Test email from Graph sender unit test")
			.HighPriority();

		SendResponse response = await email.SendAsync();

		Assert.That(response.Successful, Is.True);
	}
}
