namespace FluentEmail.Liquid.Tests;

public class ParentModel {
	public string? Id { get; set; }
	public NameDetails? ParentName { get; set; }
	public List<NameDetails> ChildrenNames { get; set; } = [];
}

public class NameDetails {
	public string? Firstname { get; set; }
	public string? Surname { get; set; }
}
