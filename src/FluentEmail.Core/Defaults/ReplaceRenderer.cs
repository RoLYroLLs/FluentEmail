using System;
using System.Reflection;
using System.Threading.Tasks;
using FluentEmail.Core.Interfaces;

namespace FluentEmail.Core.Defaults;

public class ReplaceRenderer : ITemplateRenderer {
	public string Parse<T>(string? template, T model, bool isHtml = true) {
		if (template == null) throw new ArgumentNullException(nameof(template));
		if (model == null) throw new ArgumentNullException(nameof(model));

		foreach (PropertyInfo pi in model.GetType().GetRuntimeProperties()) {
			template = template.Replace($"##{pi.Name}##", pi.GetValue(model, null)?.ToString());
		}

		return template;
	}

	public Task<string> ParseAsync<T>(string? template, T model, bool isHtml = true) => Task.FromResult(Parse(template, model, isHtml));
}
