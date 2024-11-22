using System.Collections.Generic;
using System.Linq;

namespace FluentEmail.Core.Models;

public class SendResponse {
	public string? MessageId { get; set; }
	public IList<string> ErrorMessages { get; set; } = new List<string>();
	public bool Successful => !ErrorMessages.Any();
}

public class SendResponse<T> : SendResponse {
	public T? Data { get; set; }
}
