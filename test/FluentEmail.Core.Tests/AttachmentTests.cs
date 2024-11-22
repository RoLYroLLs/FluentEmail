using System.Reflection;
using FluentEmail.Core.Models;

namespace FluentEmail.Core.Tests;

[TestFixture]
public class AttachmentTests {
	private Assembly ThisAssembly() => GetType().GetTypeInfo().Assembly;
	private const string ToEmail = "bob@test.com";
	public const string FromEmail = "johno@test.com";
	public const string Subject = "sup dawg";

	[Test]
	public void Attachment_from_stream_Is_set() {
		using FileStream stream = File.OpenRead($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}");
		Attachment attachment = new() {
			Data = stream,
			Filename = "Test.txt",
			ContentType = "text/plain"
		};

		IFluentEmail email = Email.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.Attach(attachment);

		Assert.That(email.Data.Attachments.First().Data?.Length, Is.EqualTo(20));
	}

	[Test]
	public void Attachment_from_filename_Is_set() {
		IFluentEmail email = Email.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.AttachFromFilename($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", "text/plain");

		Assert.That(email.Data.Attachments.First().Data?.Length, Is.EqualTo(20));
	}

	[Test]
	public void Attachment_from_filename_AttachmentName_Is_set() {
		string attachmentName = "attachment.txt";
		IFluentEmail email = Email.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.AttachFromFilename($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", "text/plain", attachmentName);

		Assert.That(email.Data.Attachments.First().Data?.Length, Is.EqualTo(20));
		Assert.That(email.Data.Attachments.First().Filename, Is.EqualTo(attachmentName));
	}
}
