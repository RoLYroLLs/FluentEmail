using FluentEmail.Core.Interfaces;
using FluentEmail.Graph;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class FluentEmailGraphBuilderExtensions {
	public static FluentEmailServicesBuilder AddGraphSender(this FluentEmailServicesBuilder builder, GraphOptions options) {
		builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(_ => new GraphSender(options)));
		return builder;
	}

	public static FluentEmailServicesBuilder AddGraphSender(this FluentEmailServicesBuilder builder, string clientId, string tenantId, string clientSecret, bool saveSentItems = false) {
		builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(_ => new GraphSender(clientId, tenantId, clientSecret, saveSentItems)));
		return builder;
	}
}
