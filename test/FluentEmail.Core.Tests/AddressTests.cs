namespace FluentEmail.Core.Tests;

[TestFixture]
public class AddressTests {
	[Test]
	public void SplitAddress_Test() {
		IFluentEmail email = Email
			.From("test@test.com")
			.To("james@test.com;john@test.com", "James 1;John 2");

		Assert.That(email.Data.ToAddresses.Count, Is.EqualTo(2));
		Assert.That(email.Data.ToAddresses[0].EmailAddress, Is.EqualTo("james@test.com"));
		Assert.That(email.Data.ToAddresses[1].EmailAddress, Is.EqualTo("john@test.com"));
		Assert.That(email.Data.ToAddresses[0].Name, Is.EqualTo("James 1"));
		Assert.That(email.Data.ToAddresses[1].Name, Is.EqualTo("John 2"));
	}

	[Test]
	public void SplitAddress_Test2() {
		IFluentEmail email = Email
				.From("test@test.com")
				.To("james@test.com; john@test.com", "James 1");

		Assert.That(email.Data.ToAddresses.Count, Is.EqualTo(2));
		Assert.That(email.Data.ToAddresses[0].EmailAddress, Is.EqualTo("james@test.com"));
		Assert.That(email.Data.ToAddresses[1].EmailAddress, Is.EqualTo("john@test.com"));
		Assert.That(email.Data.ToAddresses[0].Name, Is.EqualTo("James 1"));
		Assert.That(email.Data.ToAddresses[1].Name, Is.EqualTo(string.Empty));
	}

	[Test]
	public void SplitAddress_Test3() {
		IFluentEmail email = Email
				.From("test@test.com")
				.To("james@test.com; john@test.com;   Fred@test.com", "James 1;;Fred");

		Assert.That(email.Data.ToAddresses.Count, Is.EqualTo(3));
		Assert.That(email.Data.ToAddresses[0].EmailAddress, Is.EqualTo("james@test.com"));
		Assert.That(email.Data.ToAddresses[1].EmailAddress, Is.EqualTo("john@test.com"));
		Assert.That(email.Data.ToAddresses[2].EmailAddress, Is.EqualTo("Fred@test.com"));
		Assert.That(email.Data.ToAddresses[0].Name, Is.EqualTo("James 1"));
		Assert.That(email.Data.ToAddresses[1].Name, Is.EqualTo(string.Empty));
		Assert.That(email.Data.ToAddresses[2].Name, Is.EqualTo("Fred"));
	}

	[Test]
	public void SetFromAddress() {
		Email email = new();
		email.SetFrom("test@test.test", "test");

		Assert.That(email.Data.FromAddress?.EmailAddress, Is.EqualTo("test@test.test"));
		Assert.That(email.Data.FromAddress?.Name, Is.EqualTo("test"));
	}

	#region Refactored tests using setup through constructor.
	[Test]
	public void New_SplitAddress_Test() {
		IFluentEmail email = new Email()
			.To("james@test.com;john@test.com", "James 1;John 2");

		Assert.That(email.Data.ToAddresses[0].EmailAddress, Is.EqualTo("james@test.com"));
		Assert.That(email.Data.ToAddresses[1].EmailAddress, Is.EqualTo("john@test.com"));
		Assert.That(email.Data.ToAddresses[0].Name, Is.EqualTo("James 1"));
		Assert.That(email.Data.ToAddresses[1].Name, Is.EqualTo("John 2"));
	}

	[Test]
	public void New_SplitAddress_Test2() {
		IFluentEmail email = new Email()
			.To("james@test.com; john@test.com", "James 1");

		Assert.That(email.Data.ToAddresses.Count, Is.EqualTo(2));
		Assert.That(email.Data.ToAddresses[0].EmailAddress, Is.EqualTo("james@test.com"));
		Assert.That(email.Data.ToAddresses[1].EmailAddress, Is.EqualTo("john@test.com"));
		Assert.That(email.Data.ToAddresses[0].Name, Is.EqualTo("James 1"));
		Assert.That(email.Data.ToAddresses[1].Name, Is.EqualTo(string.Empty));
	}

	public void New_SplitAddress_Test3() {
		IFluentEmail email = new Email()
			.To("james@test.com; john@test.com;   Fred@test.com", "James 1;;Fred");

		Assert.That(email.Data.ToAddresses.Count, Is.EqualTo(3));
		Assert.That(email.Data.ToAddresses[0].EmailAddress, Is.EqualTo("james@test.com"));
		Assert.That(email.Data.ToAddresses[1].EmailAddress, Is.EqualTo("john@test.com"));
		Assert.That(email.Data.ToAddresses[2].EmailAddress, Is.EqualTo("Fred@test.com"));
		Assert.That(email.Data.ToAddresses[0].Name, Is.EqualTo("James 1"));
		Assert.That(email.Data.ToAddresses[1].Name, Is.EqualTo(string.Empty));
		Assert.That(email.Data.ToAddresses[2].Name, Is.EqualTo("Fred"));
	}
	#endregion
}
