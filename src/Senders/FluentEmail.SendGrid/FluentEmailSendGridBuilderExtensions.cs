using FluentEmail.Core.Interfaces;
using FluentEmail.SendGrid;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class FluentEmailSendGridBuilderExtensions {
	public static FluentEmailServicesBuilder AddSendGridSender(this FluentEmailServicesBuilder builder, SendGridOptions options) {
		builder.Services.TryAdd(ServiceDescriptor.Singleton<ISender>(_ => new SendGridSender(options)));
		return builder;
	}

	public static FluentEmailServicesBuilder AddSendGridSender(this FluentEmailServicesBuilder builder, string apiKey, bool sandBoxMode = false) {
		builder.Services.TryAdd(ServiceDescriptor.Singleton<ISender>(_ => new SendGridSender(apiKey, sandBoxMode)));
		return builder;
	}

	public static FluentEmailServicesBuilder AddSendGridSender(this FluentEmailServicesBuilder builder, string apiKey, string host, bool sandBoxMode = false) {
		builder.Services.TryAdd(ServiceDescriptor.Singleton<ISender>(_ => new SendGridSender(apiKey, host, sandBoxMode)));
		return builder;
	}
}
