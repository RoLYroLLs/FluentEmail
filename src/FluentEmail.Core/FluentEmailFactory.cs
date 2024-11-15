using System;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEmail.Core;

public class FluentEmailFactory : IFluentEmailFactory {
	private readonly IServiceProvider _services;

	public FluentEmailFactory(IServiceProvider services) => _services = services;

	public IFluentEmail Create() => _services.GetService<IFluentEmail>()!;
}
