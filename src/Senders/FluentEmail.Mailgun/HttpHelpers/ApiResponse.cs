﻿using System.Collections.Generic;
using System.Linq;

namespace FluentEmail.Mailgun.HttpHelpers;

public class ApiResponse {
	public bool Success => !Errors.Any();
	public IList<ApiError> Errors { get; set; } = new List<ApiError>();
}

public class ApiResponse<T> : ApiResponse {
	public T? Data { get; set; }
}

public class ApiError {
	public string? ErrorCode { get; set; }
	public string? ErrorMessage { get; set; }
	public string? PropertyName { get; set; }
}
