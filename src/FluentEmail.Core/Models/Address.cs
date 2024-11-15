namespace FluentEmail.Core.Models;

public class Address {
	public string? Name { get; set; }
	public string EmailAddress { get; set; }

	public Address(string emailAddress, string? name = null) {
		EmailAddress = emailAddress;
		Name = name;
	}

	public override string ToString() => Name == null ? EmailAddress : $"{Name} <{EmailAddress}>";

	public override int GetHashCode() => base.GetHashCode();

	public override bool Equals(object? obj) {
		if (obj == null || GetType() != obj.GetType()) {
			return false;
		}

		Address otherAddress = (Address)obj;
		return EmailAddress == otherAddress.EmailAddress && Name == otherAddress.Name;
	}
}
