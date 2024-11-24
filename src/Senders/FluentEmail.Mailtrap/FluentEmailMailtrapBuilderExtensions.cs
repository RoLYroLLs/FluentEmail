using FluentEmail.Core.Interfaces;
using FluentEmail.Mailtrap;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class FluentEmailMailtrapBuilderExtensions {
	public static FluentEmailServicesBuilder AddMailtrapSender(this FluentEmailServicesBuilder builder, MailtrapOptions options) {
		builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(_ => new MailtrapSender(options)));
		return builder;
	}
	public static FluentEmailServicesBuilder AddMailtrapSender(this FluentEmailServicesBuilder builder, string userName, string password, string? host = null, int? port = null) {
		builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(_ => new MailtrapSender(userName, password, host, port)));
		return builder;
	}
}
