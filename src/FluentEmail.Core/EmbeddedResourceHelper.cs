using System;
using System.IO;
using System.Reflection;

namespace FluentEmail.Core;

internal static class EmbeddedResourceHelper {
	internal static string? GetResourceAsString(Assembly assembly, string path) {
		using Stream? stream = assembly.GetManifestResourceStream(path);
		if (stream == null) throw new Exception($"{path} was not found in embedded resources."); ;
		using StreamReader reader = new(stream);
		string result = reader.ReadToEnd();
		return result;
	}
}
