using FluentAssertions;
using FluentEmail.Core;
using Fluid;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace FluentEmail.Liquid.Tests.ComplexModel;

public class ComplexModelRenderTests {
	public ComplexModelRenderTests() => SetupRenderer();

	[Test]
	public void Can_Render_Complex_Model_Properties() {
		ParentModel model = new() {
			ParentName = new NameDetails { Firstname = "Luke", Surname = "Dinosaur" },
			ChildrenNames = [
				new NameDetails { Firstname = "ChildFirstA", Surname = "ChildLastA" },
				new NameDetails { Firstname = "ChildFirstB", Surname = "ChildLastB" }
			]
		};

		string expected = @"
Parent: Luke
Children:

* ChildFirstA ChildLastA
* ChildFirstB ChildLastB
";

		IFluentEmail email = Email
			.From(TestData.FromEmail)
			.To(TestData.ToEmail)
			.Subject(TestData.Subject)
			.UsingTemplate(Template(), model);
		email.Data.Body.Should().Be(expected);
	}

	private string Template() => @"
Parent: {{ ParentName.Firstname }}
Children:
{% for Child in ChildrenNames %}
* {{ Child.Firstname }} {{ Child.Surname }}{% endfor %}
";

	private static void SetupRenderer(IFileProvider? fileProvider = null, Action<TemplateContext, object>? configureTemplateContext = null) {
		LiquidRendererOptions options = new() {
			FileProvider = fileProvider,
			ConfigureTemplateContext = configureTemplateContext,
			TemplateOptions = new TemplateOptions { MemberAccessStrategy = new UnsafeMemberAccessStrategy() }
		};
		Email.DefaultRenderer = new LiquidRenderer(Options.Create(options));
	}
}
