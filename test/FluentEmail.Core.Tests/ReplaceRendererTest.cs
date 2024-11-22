using FluentEmail.Core.Defaults;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;

namespace FluentEmail.Core.Tests;

public class ReplaceRendererTests {

	[Test]
	public async Task CanSendEmail() {
		ITemplateRenderer templateRenderer = new ReplaceRenderer();

		Address address = new("james@test.com", "james");
		Assert.That(address.Name, Is.EqualTo("james"));

		string template = "this is name: ##Name##";
		Assert.That(await templateRenderer.ParseAsync(template, address), Is.EqualTo("this is name: james"));

		address.Name = null;
		Assert.That(await templateRenderer.ParseAsync(template, address), Is.EqualTo("this is name: "));
	}
}
