using System.Collections.Generic;
using System.Threading.Tasks;
using RazorLight.Razor;

namespace FluentEmail.Razor;

public class InMemoryRazorLightProject : RazorLightProject {
	public override Task<RazorLightProjectItem> GetItemAsync(string templateKey) => Task.FromResult<RazorLightProjectItem>(new TextSourceRazorProjectItem(templateKey, templateKey));

	public override Task<IEnumerable<RazorLightProjectItem>> GetImportsAsync(string templateKey) => Task.FromResult<IEnumerable<RazorLightProjectItem>>(new List<RazorLightProjectItem>());
}
