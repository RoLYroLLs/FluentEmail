using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace FluentEmail.Mailtrap.Tests;

public class MailtrapSenderTests {
	public const string ToEmail = "testto.fluentemail@mailinator.com";
	public const string FromEmail = "testfrom.fluentemail@mailinator.com";
	public const string Subject = "Mailtrap Email Test";
	public const string Body = "This email is testing the functionality of mailtrap.";
	public const string Username = ""; // Mailtrap SMTP inbox username
	public const string Password = ""; // Mailtrap SMTP inbox password

	[SetUp]
	public void SetUp() {
		MailtrapSender sender = new(Username, Password);
		Email.DefaultSender = sender;
	}

	[Test, Ignore("Missing credentials")]
	public void CanSendEmail() {
		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.Body(Body);

		SendResponse response = email.Send();

		Assert.That(response.Successful, Is.True);
	}

	[Test, Ignore("Missing credentials")]
	public async Task CanSendEmailAsync() {
		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.Body(Body);

		SendResponse response = await email.SendAsync();

		Assert.That(response.Successful, Is.True);
	}

	[Test, Ignore("Missing credentials")]
	public async Task CanSendEmailWithAttachments() {
		MemoryStream stream = new();
		StreamWriter sw = new(stream);
		await sw.WriteLineAsync("Hey this is some text in an attachment");
		await sw.FlushAsync();
		stream.Seek(0, SeekOrigin.Begin);

		Attachment attachment = new() {
			Data = stream,
			ContentType = "text/plain",
			Filename = "mailtrapTest.txt"
		};

		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.Body(Body)
			.Attach(attachment);

		SendResponse response = await email.SendAsync();

		Assert.That(response.Successful, Is.True);
	}

	[Test, Ignore("Missing credentials")]
	public async Task CanSendEmailWithInlineImages() {
		await using FileStream stream = File.OpenRead($"{Path.Combine(Directory.GetCurrentDirectory(), "logotest.png")}");
		Attachment attachment = new() {
			IsInline = true,
			Data = stream,
			ContentType = "image/png",
			Filename = "logotest.png"
		};

		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.Body("<html>Inline image here: <img src=\"cid:logotest.png\">" +
				  "<p>You should see an image without an attachment, or without a download prompt, depending on the email client.</p></html>", true)
			.Attach(attachment);

		SendResponse response = await email.SendAsync();

		Assert.That(response.Successful, Is.True);
	}
}
