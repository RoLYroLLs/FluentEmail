using System.Dynamic;
using FluentEmail.Core;

namespace FluentEmail.Razor.Tests;

public class RazorTests {
	public const string ToEmail = "bob@test.com";
	public const string FromEmail = "johno@test.com";
	public const string Subject = "sup dawg";

	[Test]
	public void Anonymous_Model_With_List_Template_Matches() {
		string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

		IFluentEmail email = new Email(FromEmail) {
			Renderer = new RazorRenderer()
		}
				.To(ToEmail)
				.Subject(Subject)
				.UsingTemplate(template, new { Name = "LUKE", Numbers = new[] { "1", "2", "3" } });

		Assert.That(email.Data.Body, Is.EqualTo("sup LUKE here is a list 123"));
	}

	[Test]
	public void Reuse_Cached_Templates() {
		string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";
		string template2 = "sup @Model.Name this is the second template";

		for (int i = 0; i < 10; i++) {
			IFluentEmail email = new Email(FromEmail) {
				Renderer = new RazorRenderer()
			}
					.To(ToEmail)
					.Subject(Subject)
					.UsingTemplate(template, new { Name = i, Numbers = new[] { "1", "2", "3" } });

			Assert.That(email.Data.Body, Is.EqualTo("sup " + i + " here is a list 123"));

			IFluentEmail email2 = new Email(FromEmail) {
				Renderer = new RazorRenderer()
			}
					.To(ToEmail)
					.Subject(Subject)
					.UsingTemplate(template2, new { Name = i });

			Assert.That(email2.Data.Body, Is.EqualTo("sup " + i + " this is the second template"));
		}
	}

	[Test]
	public void New_Anonymous_Model_Template_Matches() {
		string template = "sup @Model.Name";

		IFluentEmail email = new Email(FromEmail) {
			Renderer = new RazorRenderer()
		}
				.To(ToEmail)
				.Subject(Subject)
				.UsingTemplate(template, new { Name = "LUKE" });

		Assert.That(email.Data.Body, Is.EqualTo("sup LUKE"));
	}

	[Test]
	public void New_Anonymous_Model_With_List_Template_Matches() {
		string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

		IFluentEmail email = new Email(FromEmail) {
			Renderer = new RazorRenderer()
		}
				.To(ToEmail)
				.Subject(Subject)
				.UsingTemplate(template, new { Name = "LUKE", Numbers = new[] { "1", "2", "3" } });

		Assert.That(email.Data.Body, Is.EqualTo("sup LUKE here is a list 123"));
	}

	[Test]
	public void New_Reuse_Cached_Templates() {
		string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";
		string template2 = "sup @Model.Name this is the second template";

		for (int i = 0; i < 10; i++) {
			IFluentEmail email = new Email(FromEmail) {
				Renderer = new RazorRenderer()
			}
					.To(ToEmail)
					.Subject(Subject)
					.UsingTemplate(template, new { Name = i, Numbers = new[] { "1", "2", "3" } });

			Assert.That(email.Data.Body, Is.EqualTo("sup " + i + " here is a list 123"));

			IFluentEmail email2 = new Email(FromEmail) {
				Renderer = new RazorRenderer()
			}
					.To(ToEmail)
					.Subject(Subject)
					.UsingTemplate(template2, new { Name = i });

			Assert.That(email2.Data.Body, Is.EqualTo("sup " + i + " this is the second template"));
		}
	}

	[Test]
	public void Should_be_able_to_use_project_layout_with_viewbag() {
		string projectRoot = Directory.GetCurrentDirectory();
		Email.DefaultRenderer = new RazorRenderer(projectRoot);

		string template = @"
@{
	Layout = ""./Shared/_Layout.cshtml"";
}
sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

		dynamic viewBag = new ExpandoObject();
		viewBag.Title = "Hello!";
		IFluentEmail email = new Email(FromEmail) {
			Renderer = new RazorRenderer()
		}
			.To(ToEmail)
			.Subject(Subject)
			.UsingTemplate(template, new ViewModelWithViewBag { Name = "LUKE", Numbers = ["1", "2", "3"], ViewBag = viewBag });

		Assert.That(email.Data.Body, Is.EqualTo($"<h1>Hello!</h1>{Environment.NewLine}<div>{Environment.NewLine}sup LUKE here is a list 123</div>"));
	}

	[Test]
	public void Should_be_able_to_use_embedded_layout_with_viewbag() {
		string template = @"
@{
	Layout = ""_EmbeddedLayout.cshtml"";
}
sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

		dynamic viewBag = new ExpandoObject();
		viewBag.Title = "Hello!";
		IFluentEmail email = new Email(FromEmail) {
			Renderer = new RazorRenderer(typeof(RazorTests))
		}
			.To(ToEmail)
			.Subject(Subject)
			.UsingTemplate(template, new ViewModelWithViewBag { Name = "LUKE", Numbers = ["1", "2", "3"], ViewBag = viewBag });

		Assert.That(email.Data.Body, Is.EqualTo($"<h2>Hello!</h2>{Environment.NewLine}<div>{Environment.NewLine}sup LUKE here is a list 123</div>"));
	}
}

public class ViewModelWithViewBag : IViewBagModel {
	public required ExpandoObject ViewBag { get; set; }
	public string? Name { get; set; }
	public string[]? Numbers { get; set; }
}
