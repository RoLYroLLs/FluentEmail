using FluentEmail.Core;
using Attachment = FluentEmail.Core.Models.Attachment;

namespace FluentEmail.SendGrid.Tests;

public class SendGridSenderTests {
	public const string ApiKey = "missing-credentials"; // TODO: Put your API key here

	private const string ToEmail = "fluentEmail@mailinator.com";
	public const string ToName = "FluentEmail Mailinator";
	public const string FromEmail = "test@fluentmail.com";
	public const string FromName = "SendGridSender Test";

	[SetUp]
	public void SetUp() {
		if (string.IsNullOrWhiteSpace(ApiKey)) throw new ArgumentException("SendGrid Api Key needs to be supplied");

		SendGridSender sender = new(ApiKey, true);
		Email.DefaultSender = sender;
	}

	[Test, Ignore("No sendgrid credentials")]
	public async Task CanSendEmail() {
		const string subject = "SendMail Test";
		const string body = "This email is testing send mail functionality of SendGrid Sender.";

		IFluentEmail email = Email
			.From(FromEmail, FromName)
			.To(ToEmail, ToName)
			.Subject(subject)
			.Body(body);

		Core.Models.SendResponse response = await email.SendAsync();

		Assert.That(response.Successful, Is.True);
	}

	[Test, Ignore("No sendgrid credentials")]
	public async Task CanSendTemplateEmail() {
		const string subject = "SendMail Test";
		const string templateId = "123456-insert-your-own-id-here";
		object templateData = new {
			Name = ToName,
			ArbitraryValue = "The quick brown fox jumps over the lazy dog."
		};

		IFluentEmail email = Email
			.From(FromEmail, FromName)
			.To(ToEmail, ToName)
			.Subject(subject);

		Core.Models.SendResponse response = await email.SendWithTemplateAsync(templateId, templateData);

		Assert.That(response.Successful, Is.True);
	}

	[Test, Ignore("No sendgrid credentials")]
	public async Task CanSendEmailWithReplyTo() {
		const string subject = "SendMail Test";
		const string body = "This email is testing send mail with ReplyTo functionality of SendGrid Sender.";

		IFluentEmail email = Email
			.From(FromEmail, FromName)
			.To(ToEmail, ToName)
			.ReplyTo(ToEmail, ToName)
			.Subject(subject)
			.Body(body);

		Core.Models.SendResponse response = await email.SendAsync();

		Assert.That(response.Successful, Is.True);
	}

	[Test, Ignore("No sendgrid credentials")]
	public async Task CanSendEmailWithCategory() {
		const string subject = "SendMail Test";
		const string body = "This email is testing send mail with Categories functionality of SendGrid Sender.";

		IFluentEmail email = Email
			.From(FromEmail, FromName)
			.To(ToEmail, ToName)
			.ReplyTo(ToEmail, ToName)
			.Subject(subject)
			.Tag("TestCategory")
			.Body(body);

		Core.Models.SendResponse response = await email.SendAsync();

		Assert.That(response.Successful, Is.True);
	}

	[Test, Ignore("No sendgrid credentials")]
	public async Task CanSendEmailWithAttachments() {
		const string subject = "SendMail With Attachments Test";
		const string body = "This email is testing the attachment functionality of SendGrid Sender.";

		await using FileStream stream = File.OpenRead($"{Directory.GetCurrentDirectory()}/test-binary.xlsx");
		Attachment attachment = new() {
			Data = stream,
			ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
			Filename = "test-binary.xlsx"
		};

		IFluentEmail email = Email
			.From(FromEmail, FromName)
			.To(ToEmail, ToName)
			.Subject(subject)
			.Body(body)
			.Attach(attachment);

		Core.Models.SendResponse response = await email.SendAsync();

		Assert.That(response.Successful, Is.True);
	}

	[Test, Ignore("No sendgrid credentials")]
	public async Task CanSendHighPriorityEmail() {
		const string subject = "SendMail Test";
		const string body = "This email is testing send mail functionality of SendGrid Sender.";

		IFluentEmail email = Email
			.From(FromEmail, FromName)
			.To(ToEmail, ToName)
			.Subject(subject)
			.Body(body)
			.HighPriority();

		Core.Models.SendResponse response = await email.SendAsync();

		Assert.That(response.Successful, Is.True);
	}

	[Test, Ignore("No sendgrid credentials")]
	public async Task CanSendLowPriorityEmail() {
		const string subject = "SendMail Test";
		const string body = "This email is testing send mail functionality of SendGrid Sender.";

		IFluentEmail email = Email
			.From(FromEmail, FromName)
			.To(ToEmail, ToName)
			.Subject(subject)
			.Body(body)
			.LowPriority();

		Core.Models.SendResponse response = await email.SendAsync();

		Assert.That(response.Successful, Is.True);
	}
}
