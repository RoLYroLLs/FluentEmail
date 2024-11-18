using FluentEmail.Core.Models;

namespace FluentEmail.Core.Tests;

[TestFixture]
public class FluentEmailTests {
	public const string ToEmail = "bob@test.com";
	public const string FromEmail = "johno@test.com";
	public const string Subject = "sup dawg";
	private const string Body = "what be the hipitity hap?";

	[Test]
	public void To_Address_Is_Set() {
		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail);

		Assert.That(email.Data.ToAddresses[0].EmailAddress, Is.EqualTo(ToEmail));
	}

	[Test]
	public void From_Address_Is_Set() {
		IFluentEmail email = Email.From(FromEmail);

		Assert.That(email.Data.FromAddress?.EmailAddress, Is.EqualTo(FromEmail));
	}

	[Test]
	public void Subject_Is_Set() {
		IFluentEmail email = Email
			.From(FromEmail)
			.Subject(Subject);

		Assert.That(email.Data.Subject, Is.EqualTo(Subject));
	}

	[Test]
	public void Body_Is_Set() {
		IFluentEmail email = Email.From(FromEmail)
			.Body(Body);

		Assert.That(email.Data.Body, Is.EqualTo(Body));
	}

	[Test]
	public void Can_Add_Multiple_Recipients() {
		string toEmail1 = "bob@test.com";
		string toEmail2 = "ratface@test.com";

		IFluentEmail email = Email
			.From(FromEmail)
			.To(toEmail1)
			.To(toEmail2);

		Assert.That(email.Data.ToAddresses.Count, Is.EqualTo(2));
	}

	[Test]
	public void Can_Add_Multiple_Recipients_From_List() {
		List<Address> emails = [
			new("email1@email.com"),
			new("email2@email.com")
		];

		IFluentEmail email = Email
			.From(FromEmail)
			.To(emails);

		Assert.That(email.Data.ToAddresses.Count, Is.EqualTo(2));
	}

	[Test]
	public void Can_Add_Multiple_Recipients_From_String_List() {
		List<string> emails = [
			"email1@email.com",
			"email2@email.com"
		];

		IFluentEmail email = Email
			.From(FromEmail)
			.To(emails);

		Assert.That(email.Data.ToAddresses.Count, Is.EqualTo(2));
	}

	[Test]
	public void Can_Add_Multiple_Recipients_From_String_Array() {
		string[] emails = [
			"email1@email.com",
			"email2@email.com"
		];

		IFluentEmail email = Email
			.From(FromEmail)
			.To(emails);

		Assert.That(email.Data.ToAddresses.Count, Is.EqualTo(2));
	}

	[Test]
	public void Can_Add_Multiple_CCRecipients_From_List() {
		List<Address> emails = [
			new("email1@email.com"),
			new("email2@email.com")
		];

		IFluentEmail email = Email
			.From(FromEmail)
			.CC(emails);

		Assert.That(email.Data.CcAddresses.Count, Is.EqualTo(2));
	}

	[Test]
	public void Can_Add_Multiple_BCCRecipients_From_List() {
		List<Address> emails = [
			new("email1@email.com"),
			new("email2@email.com")
		];

		IFluentEmail email = Email
			.From(FromEmail)
			.BCC(emails);

		Assert.That(email.Data.BccAddresses.Count, Is.EqualTo(2));
	}

	[Test]
	public void Is_Valid_With_Properties_Set() {
		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.Body(Body);

		Assert.That(email.Data.Body, Is.EqualTo(Body));
		Assert.That(email.Data.Subject, Is.EqualTo(Subject));
		Assert.That(email.Data.FromAddress?.EmailAddress, Is.EqualTo(FromEmail));
		Assert.That(email.Data.ToAddresses[0].EmailAddress, Is.EqualTo(ToEmail));
	}

	[Test]
	public void ReplyTo_Address_Is_Set() {
		string replyEmail = "reply@email.com";

		IFluentEmail email = Email.From(FromEmail)
			.ReplyTo(replyEmail);

		Assert.That(email.Data.ReplyToAddresses.First().EmailAddress, Is.EqualTo(replyEmail));
	}

	#region Refactored tests using setup through constructors.
	[Test]
	public void New_To_Address_Is_Set() {
		IFluentEmail email = new Email(FromEmail)
			.To(ToEmail);

		Assert.That(email.Data.ToAddresses[0].EmailAddress, Is.EqualTo(ToEmail));
	}

	[Test]
	public void New_From_Address_Is_Set() {
		Email email = new(FromEmail);

		Assert.That(email.Data.FromAddress?.EmailAddress, Is.EqualTo(FromEmail));
	}

	[Test]
	public void New_Subject_Is_Set() {
		IFluentEmail email = new Email(FromEmail)
			.Subject(Subject);

		Assert.That(email.Data.Subject, Is.EqualTo(Subject));
	}

	[Test]
	public void New_Body_Is_Set() {
		IFluentEmail email = new Email(FromEmail)
			.Body(Body);

		Assert.That(email.Data.Body, Is.EqualTo(Body));
	}

	[Test]
	public void New_Can_Add_Multiple_Recipients() {
		string toEmail1 = "bob@test.com";
		string toEmail2 = "ratface@test.com";

		IFluentEmail email = new Email(FromEmail)
			.To(toEmail1)
			.To(toEmail2);

		Assert.That(email.Data.ToAddresses.Count, Is.EqualTo(2));
	}

	[Test]
	public void New_Can_Add_Multiple_Recipients_From_List() {
		List<Address> emails = [
			new("email1@email.com"),
			new("email2@email.com")
		];

		IFluentEmail email = new Email(FromEmail)
			.To(emails);

		Assert.That(email.Data.ToAddresses.Count, Is.EqualTo(2));
	}

	[Test]
	public void New_Can_Add_Multiple_CCRecipients_From_List() {
		List<Address> emails = [
			new("email1@email.com"),
			new("email2@email.com")
		];

		IFluentEmail email = new Email(FromEmail)
			.CC(emails);

		Assert.That(email.Data.CcAddresses.Count, Is.EqualTo(2));
	}

	[Test]
	public void New_Can_Add_Multiple_BCCRecipients_From_List() {
		List<Address> emails = [
			new("email1@email.com"),
			new("email2@email.com")
		];

		IFluentEmail email = new Email(FromEmail)
			.BCC(emails);

		Assert.That(email.Data.BccAddresses.Count, Is.EqualTo(2));
	}

	[Test]
	public void New_Is_Valid_With_Properties_Set() {
		IFluentEmail email = new Email(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.Body(Body);

		Assert.That(email.Data.Body, Is.EqualTo(Body));
		Assert.That(email.Data.Subject, Is.EqualTo(Subject));
		Assert.That(email.Data.FromAddress?.EmailAddress, Is.EqualTo(FromEmail));
		Assert.That(email.Data.ToAddresses[0].EmailAddress, Is.EqualTo(ToEmail));
	}

	[Test]
	public void New_ReplyTo_Address_Is_Set() {
		string replyEmail = "reply@email.com";

		IFluentEmail email = new Email(FromEmail)
			.ReplyTo(replyEmail);

		Assert.That(email.Data.ReplyToAddresses.First().EmailAddress, Is.EqualTo(replyEmail));
	}
	#endregion
}
