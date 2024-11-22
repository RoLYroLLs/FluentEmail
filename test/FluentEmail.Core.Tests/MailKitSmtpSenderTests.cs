using FluentEmail.Core;
using FluentEmail.MailKitSmtp;
using Attachment = FluentEmail.Core.Models.Attachment;

namespace FluentEmail.MailKit.Tests;

[NonParallelizable]
public class MailKitSmtpSenderTests {
	// Warning: To pass, an smtp listener must be running on localhost:25.

	public const string ToEmail = "bob@test.com";
	public const string FromEmail = "johno@test.com";
	public const string Subject = "sup dawg";
	public const string Body = "what be the hipitity hap?";

	private readonly string _tempDirectory = Path.Combine(Path.GetTempPath(), "EmailTest");

	[SetUp]
	public void SetUp() {
		MailKitSender sender = new(new SmtpClientOptions {
			Server = "localhost",
			Port = 25,
			UseSsl = false,
			RequiresAuthentication = false,
			UsePickupDirectory = true,
			MailPickupDirectory = Path.Combine(Path.GetTempPath(), "EmailTest")
		});

		Email.DefaultSender = sender;
		Directory.CreateDirectory(_tempDirectory);
	}

	[TearDown]
	public void TearDown() => Directory.Delete(_tempDirectory, true);

	[Test]
	public void CanSendEmail() {
		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Body("<h2>Test</h2>", true);

		Core.Models.SendResponse response = email.Send();

		IEnumerable<string> files = Directory.EnumerateFiles(_tempDirectory, "*.eml");

		Assert.That(response.Successful, Is.True);
		Assert.That(files, Is.Not.Empty);
	}

	[Test]
	public async Task CanSendEmailWithAttachments() {
		MemoryStream stream = new();
		StreamWriter sw = new(stream);
		await sw.WriteLineAsync("Hey this is some text in an attachment");
		await sw.FlushAsync();
		stream.Seek(0, SeekOrigin.Begin);

		Attachment attachment = new() {
			Data = stream,
			ContentType = "text/plain",
			Filename = "MailKitAttachment.txt"
		};

		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.Body(Body)
			.Attach(attachment);

		Core.Models.SendResponse response = await email.SendAsync();

		IEnumerable<string> files = Directory.EnumerateFiles(_tempDirectory, "*.eml");

		Assert.That(response.Successful, Is.True);
		Assert.That(files, Is.Not.Empty);
	}

	[Test]
	[TestCase]
	[TestCase("logotest.png")]
	public async Task CanSendEmailWithInlineImages(string? contentId = null) {
		await using FileStream stream = File.OpenRead($"{Path.Combine(Directory.GetCurrentDirectory(), "logotest.png")}");
		Attachment attachment = new() {
			IsInline = true,
			Data = stream,
			ContentType = "image/png",
			Filename = "logotest.png",
			ContentId = contentId
		};

		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.Body("<html>Inline image here: <img src=\"cid:logotest.png\"><p>You should see an image without an attachment, or without a download prompt, depending on the email client.</p></html>", true)
			.Attach(attachment);

		Core.Models.SendResponse response = await email.SendAsync();

		IEnumerable<string> files = Directory.EnumerateFiles(_tempDirectory, "*.eml");

		Assert.That(response.Successful, Is.True);
		Assert.That(files, Is.Not.Empty);
	}

	[Test]
	public async Task CanSendEmailWithInlineImagesAndAttachmentTogether() {
		MemoryStream attachmentStream = new();
		StreamWriter sw = new(attachmentStream);
		await sw.WriteLineAsync("Hey this is some text in an attachment");
		await sw.FlushAsync();
		attachmentStream.Seek(0, SeekOrigin.Begin);

		Attachment attachment = new() {
			Data = attachmentStream,
			ContentType = "text/plain",
			Filename = "MailKitAttachment.txt",
		};

		await using FileStream inlineStream = File.OpenRead($"{Path.Combine(Directory.GetCurrentDirectory(), "logotest.png")}");

		Attachment attachmentInline = new() {
			IsInline = true,
			Data = inlineStream,
			ContentType = "image/png",
			Filename = "logotest.png",
		};

		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.Body("<html>Inline image here: <img src=\"cid:logotest.png\"><p>You should see an image inline without a picture attachment.</p><p>A single .txt file should also be attached.</p></html>", true)
			.Attach(attachment)
			.Attach(attachmentInline);

		Core.Models.SendResponse response = await email.SendAsync();

		IEnumerable<string> files = Directory.EnumerateFiles(_tempDirectory, "*.eml");
		Assert.That(response.Successful, Is.True);
		Assert.That(files, Is.Not.Empty);
	}

	[Test]
	public async Task CanSendAsyncHtmlAndPlaintextTogether() {
		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Body("<h2>Test</h2><p>some body text</p>", true)
			.PlaintextAlternativeBody("Test - Some body text");

		Core.Models.SendResponse response = await email.SendAsync();

		Assert.That(response.Successful, Is.True);
	}

	[Test]
	public void CanSendHtmlAndPlaintextTogether() {
		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Body("<h2>Test</h2><p>some body text</p>", true)
			.PlaintextAlternativeBody("Test - Some body text");

		Core.Models.SendResponse response = email.Send();

		Assert.That(response.Successful, Is.True);
	}
}
