using FluentEmail.Core;
using FluentEmail.Core.Models;
using Newtonsoft.Json;

namespace FluentEmail.Mailgun.Tests;

public class MailgunSenderTests {
	private const string ToEmail = "bentest1@mailinator.com";
	public const string FromEmail = "ben@test.com";
	public const string Subject = "Attachment Tests";
	public const string Body = "This email is testing the attachment functionality of MailGun.";

	[SetUp]
	public void SetUp() {
		MailgunSender sender = new("sandboxcf5f41bbf2f84f15a386c60e253b5fe9.mailgun.org", "key-8d32c046d7f14ada8d5ba8253e3e30de");
		Email.DefaultSender = sender;
	}

	[Test]
	public async Task CanSendEmail() {
		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.Body(Body);

		SendResponse response = await email.SendAsync();

		Assert.That(response.Successful, Is.True);
	}

	[Test]
	public async Task GetMessageIdInResponse() {
		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.Body(Body);

		SendResponse response = await email.SendAsync();

		Assert.That(response.Successful, Is.True);
		Assert.That(response.MessageId, Is.Not.Empty);
	}

	[Test]
	public async Task CanSendEmailWithTag() {
		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.Body(Body)
			.Tag("test");

		SendResponse response = await email.SendAsync();

		Assert.That(response.Successful, Is.True);
	}

	[Test]
	public async Task CanSendEmailWithVariables() {
		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.Body(Body)
			.Header("X-Mailgun-Variables", JsonConvert.SerializeObject(new Variable { Var1 = "Test" }));

		SendResponse response = await email.SendAsync();

		Assert.That(response.Successful, Is.True);
	}

	[Test]
	public async Task CanSendEmailWithAttachments() {
		MemoryStream stream = new();
		StreamWriter sw = new(stream);
		sw.WriteLine("Hey this is some text in an attachment");
		sw.Flush();
		stream.Seek(0, SeekOrigin.Begin);

		Attachment attachment = new() {
			Data = stream,
			ContentType = "text/plain",
			Filename = "mailgunTest.txt"
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

	[Test]
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
			.Body("<html>Inline image here: <img src=\"cid:logotest.png\"><p>You should see an image without an attachment, or without a download prompt, depending on the email client.</p></html>", true)
			.Attach(attachment);

		SendResponse response = await email.SendAsync();

		Assert.That(response.Successful, Is.True);
	}

	class Variable {
		public string? Var1 { get; set; }
	}
}
