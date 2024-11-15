using System.Globalization;
using System.Reflection;
using FluentEmail.Core.Defaults;
using FluentEmail.Core.Interfaces;

namespace FluentEmail.Core.Tests;

[TestFixture]
public class TemplateEmailTests {
	private Assembly ThisAssembly() => GetType().GetTypeInfo().Assembly;
	public const string ToEmail = "bob@test.com";
	public const string FromEmail = "johno@test.com";
	public const string Subject = "sup dawg";

	[Test]
	public void Anonymous_Model_Template_From_File_Matches() {
		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.UsingTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL" });

		Assert.That(email.Data.Body, Is.EqualTo("yo email FLUENTEMAIL"));
	}

	[Test]
	public void Using_Template_From_Not_Existing_Culture_File_Using_Default_Template() {
		CultureInfo culture = new("fr-FR");
		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL", culture }, culture);

		Assert.That(email.Data.Body, Is.EqualTo("yo email FLUENTEMAIL"));
	}

	[Test]
	public void Using_Template_From_Culture_File() {
		CultureInfo culture = new("he-IL");
		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL" }, culture);

		Assert.That(email.Data.Body, Is.EqualTo("hebrew email FLUENTEMAIL"));
	}

	[Test]
	public void Using_Template_From_Current_Culture_File() {
		CultureInfo culture = new("he-IL");
		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL" }, culture);

		Assert.That(email.Data.Body, Is.EqualTo("hebrew email FLUENTEMAIL"));
	}

	[Test]
	public void Anonymous_Model_Template_Matches() {
		string template = "sup ##Name##";

		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.UsingTemplate(template, new { Name = "LUKE" });

		Assert.That(email.Data.Body, Is.EqualTo("sup LUKE"));
	}

	[Test]
	public void Set_Custom_Template() {
		string template = "sup ##Name## here is a list @foreach(var i in Model.Numbers) { @i }";

		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.UsingTemplateEngine(new TestTemplate())
			.UsingTemplate(template, new { Name = "LUKE", Numbers = new[] { "1", "2", "3" } });

		Assert.That(email.Data.Body, Is.EqualTo("custom template"));
	}

	[Test]
	public void Using_Template_From_Embedded_Resource() {
		IFluentEmail email = Email
			.From(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.UsingTemplateFromEmbedded("FluentEmail.Core.Tests.test-embedded.txt", new { Test = "EMBEDDEDTEST" }, ThisAssembly());

		Assert.That(email.Data.Body, Is.EqualTo("yo email EMBEDDEDTEST"));
	}

	[Test]
	public void New_Anonymous_Model_Template_From_File_Matches() {
		IFluentEmail email = new Email(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.UsingTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL" });

		Assert.That(email.Data.Body, Is.EqualTo("yo email FLUENTEMAIL"));
	}

	[Test]
	public void New_Using_Template_From_Not_Existing_Culture_File_Using_Default_Template() {
		CultureInfo culture = new("fr-FR");
		IFluentEmail email = new Email(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL", culture }, culture);

		Assert.That(email.Data.Body, Is.EqualTo("yo email FLUENTEMAIL"));
	}

	[Test]
	public void New_Using_Template_From_Culture_File() {
		CultureInfo culture = new("he-IL");
		IFluentEmail email = new Email(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL" }, culture);

		Assert.That(email.Data.Body, Is.EqualTo("hebrew email FLUENTEMAIL"));
	}

	[Test]
	public void New_Using_Template_From_Current_Culture_File() {
		CultureInfo culture = new("he-IL");
		IFluentEmail email = new Email(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL" }, culture);

		Assert.That(email.Data.Body, Is.EqualTo("hebrew email FLUENTEMAIL"));
	}

	[Test]
	public void New_Set_Custom_Template() {
		string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

		IFluentEmail email = new Email(new TestTemplate(), new SaveToDiskSender("/"), FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.UsingTemplate(template, new { Name = "LUKE", Numbers = new[] { "1", "2", "3" } });

		Assert.That(email.Data.Body, Is.EqualTo("custom template"));
	}

	[Test]
	public void New_Using_Template_From_Embedded_Resource() {
		IFluentEmail email = new Email(FromEmail)
			.To(ToEmail)
			.Subject(Subject)
			.UsingTemplateFromEmbedded("FluentEmail.Core.Tests.test-embedded.txt", new { Test = "EMBEDDEDTEST" }, ThisAssembly());

		Assert.That(email.Data.Body, Is.EqualTo("yo email EMBEDDEDTEST"));
	}
}

public class TestTemplate : ITemplateRenderer {
	public string Parse<T>(string? template, T model, bool isHtml = true) => "custom template";

	public Task<string> ParseAsync<T>(string? template, T model, bool isHtml = true) => Task.FromResult(Parse(template, model, isHtml));
}
