using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentEmail.Core.Interfaces;
using Fluid;
using Fluid.Ast;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace FluentEmail.Liquid;

public class LiquidRenderer : ITemplateRenderer {
	private readonly IOptions<LiquidRendererOptions> _options;
	private readonly LiquidParser _parser;

	public LiquidRenderer(IOptions<LiquidRendererOptions> options) {
		_options = options;
		_parser = new LiquidParser();
	}

	public string Parse<T>(string? template, T model, bool isHtml = true) => ParseAsync(template, model, isHtml).GetAwaiter().GetResult();

	public async Task<string> ParseAsync<T>(string? template, T model, bool isHtml = true) {
		LiquidRendererOptions rendererOptions = _options.Value;

		// Check for a custom file provider
		IFileProvider? fileProvider = rendererOptions.FileProvider;
		IFluidTemplate viewTemplate = ParseTemplate(template);

		TemplateContext context = new(model, rendererOptions.TemplateOptions) {
			// provide some services to all statements
			AmbientValues = {
				["FileProvider"] = fileProvider,
				["Sections"] = new Dictionary<string, List<Statement>>()
			},
			Options = {
				FileProvider = fileProvider
			}
		};

		rendererOptions.ConfigureTemplateContext?.Invoke(context, model!);

		string body = await viewTemplate.RenderAsync(context, rendererOptions.TextEncoder);

		// if a layout is specified while rendering a view, execute it
		if (context.AmbientValues.TryGetValue("Layout", out object? layoutPath)) {
			context.AmbientValues["Body"] = body;
			IFluidTemplate layoutTemplate = ParseLiquidFile((string)layoutPath, fileProvider!);
			return await layoutTemplate.RenderAsync(context, rendererOptions.TextEncoder);
		}

		return body;
	}

	private IFluidTemplate ParseLiquidFile(
		string path,
		IFileProvider? fileProvider) {
		static void ThrowMissingFileProviderException() {
			const string message = "Cannot parse external file, file provider missing";
			throw new ArgumentNullException(nameof(LiquidRendererOptions.FileProvider), message);
		}

		if (fileProvider is null) {
			ThrowMissingFileProviderException();
		}

		IFileInfo fileInfo = fileProvider!.GetFileInfo(path);
		using Stream stream = fileInfo.CreateReadStream();
		using StreamReader sr = new(stream);

		return ParseTemplate(sr.ReadToEnd());
	}

	private IFluidTemplate ParseTemplate(string? content) => !_parser.TryParse(content, out IFluidTemplate? template, out string? errors)
			? throw new Exception(string.Join(Environment.NewLine, errors))
			: template;
}
