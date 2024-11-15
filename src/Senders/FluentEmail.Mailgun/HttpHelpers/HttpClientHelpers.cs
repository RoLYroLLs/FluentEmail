using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentEmail.Core;
using Newtonsoft.Json;

namespace FluentEmail.Mailgun.HttpHelpers;

public class HttpClientHelpers {
	public static HttpContent GetPostBody(IEnumerable<KeyValuePair<string, string>> parameters) {
		IEnumerable<string> formatted = parameters.Select(x => x.Key + "=" + x.Value);
		return new StringContent(string.Join("&", formatted), Encoding.UTF8, "application/x-www-form-urlencoded");
	}

	public static HttpContent GetJsonBody(object value) => new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");

	public static HttpContent GetMultipartFormDataContentBody(IEnumerable<KeyValuePair<string, string>> parameters, IEnumerable<HttpFile> files) {
		MultipartFormDataContent mpContent = new();

		parameters.ForEach(p => {
			mpContent.Add(new StringContent(p.Value), p.Key);
		});

		files.ForEach(file => {
			using MemoryStream memoryStream = new();
			file.Data?.CopyTo(memoryStream);
			mpContent.Add(new ByteArrayContent(memoryStream.ToArray()), file.ParameterName, file.Filename);
		});

		return mpContent;
	}
}

public static class HttpClientExtensions {
	public static async Task<ApiResponse<T>> Get<T>(this HttpClient client, string url) {
		HttpResponseMessage response = await client.GetAsync(url);
		QuickResponse<T> qr = await QuickResponse<T>.FromMessage(response);
		return qr.ToApiResponse();
	}

	public static async Task<ApiResponse> GetFile(this HttpClient client, string url) {
		HttpResponseMessage response = await client.GetAsync(url);
		QuickFile qr = await QuickFile.FromMessage(response);
		return qr.ToApiResponse();
	}

	public static async Task<ApiResponse<T>> Post<T>(this HttpClient client, string url, IEnumerable<KeyValuePair<string, string>> parameters) {
		HttpResponseMessage response = await client.PostAsync(url, HttpClientHelpers.GetPostBody(parameters));
		QuickResponse<T> qr = await QuickResponse<T>.FromMessage(response);
		return qr.ToApiResponse();
	}

	public static async Task<ApiResponse<T>> Post<T>(this HttpClient client, string url, object data) {
		HttpResponseMessage response = await client.PostAsync(url, HttpClientHelpers.GetJsonBody(data));
		QuickResponse<T> qr = await QuickResponse<T>.FromMessage(response);
		return qr.ToApiResponse();
	}

	public static async Task<ApiResponse<T>> PostMultipart<T>(this HttpClient client, string url, IEnumerable<KeyValuePair<string, string>> parameters, IEnumerable<HttpFile> files) {
		HttpResponseMessage response = await client.PostAsync(url, HttpClientHelpers.GetMultipartFormDataContentBody(parameters, files)).ConfigureAwait(false);
		QuickResponse<T> qr = await QuickResponse<T>.FromMessage(response);
		return qr.ToApiResponse();
	}

	public static async Task<ApiResponse> Delete(this HttpClient client, string url) {
		HttpResponseMessage response = await client.DeleteAsync(url);
		QuickResponse qr = await QuickResponse.FromMessage(response);
		return qr.ToApiResponse();
	}
}

public class QuickResponse {
	public HttpResponseMessage? Message { get; set; }

	public string? ResponseBody { get; set; }

	public IList<ApiError> Errors { get; set; } = new List<ApiError>();

	public ApiResponse ToApiResponse() => new() {
		Errors = Errors
	};

	public static async Task<QuickResponse> FromMessage(HttpResponseMessage message) {
		QuickResponse response = new() {
			Message = message,
			ResponseBody = await message.Content.ReadAsStringAsync()
		};

		if (!message.IsSuccessStatusCode) {
			response.HandleFailedCall();
		}

		return response;
	}

	protected void HandleFailedCall() {
		try {
			Errors = JsonConvert.DeserializeObject<List<ApiError>>(ResponseBody ?? string.Empty) ?? new List<ApiError>();

			if (!Errors.Any()) {
				Errors.Add(new ApiError {
					ErrorMessage = !string.IsNullOrEmpty(ResponseBody) ? ResponseBody : Message?.StatusCode.ToString()
				});
			}
		} catch (Exception) {
			Errors.Add(new ApiError {
				ErrorMessage = !string.IsNullOrEmpty(ResponseBody) ? ResponseBody : Message?.StatusCode.ToString()
			});
		}
	}
}

public class QuickResponse<T> : QuickResponse {
	public T? Data { get; set; }

	public new ApiResponse<T> ToApiResponse() => new() {
		Errors = Errors,
		Data = Data
	};

	public new static async Task<QuickResponse<T>> FromMessage(HttpResponseMessage message) {
		QuickResponse<T> response = new() {
			Message = message,
			ResponseBody = await message.Content.ReadAsStringAsync()
		};

		if (message.IsSuccessStatusCode) {
			try {
				response.Data = JsonConvert.DeserializeObject<T>(response.ResponseBody);
			} catch (Exception) {
				response.HandleFailedCall();
			}
		} else {
			response.HandleFailedCall();
		}

		return response;
	}
}

public class QuickFile : QuickResponse<Stream> {
	public new static async Task<QuickFile> FromMessage(HttpResponseMessage message) {
		QuickFile response = new() {
			Message = message,
			ResponseBody = await message.Content.ReadAsStringAsync()
		};

		if (message.IsSuccessStatusCode) {
			response.Data = await message.Content.ReadAsStreamAsync();
		} else {
			response.HandleFailedCall();
		}

		return response;
	}
}
