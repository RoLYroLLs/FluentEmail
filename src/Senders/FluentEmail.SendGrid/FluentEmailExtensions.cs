using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace FluentEmail.SendGrid;

public static class FluentEmailExtensions {
	public static async Task<SendResponse> SendWithTemplateAsync(this IFluentEmail email, string templateId, object templateData) {
		ISendGridSender sendGridSender = (email.Sender as ISendGridSender)!;
		return await sendGridSender.SendWithTemplateAsync(email, templateId, templateData);
	}
}
