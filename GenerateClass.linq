<Query Kind="Program">
  <Connection>
    <ID>14f5f8a7-e804-417d-a011-70686b046c0b</ID>
    <Persist>true</Persist>
    <Server>MC-DEV-DB-01V</Server>
    <Database>CredentialDB</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

void Main()
{
	GenerateClass(Aspnet_UsersInRoles.First().GetType());
}

public void GenerateClass(Type type)
{
	var properties = type.GetProperties();
	var fields = type.GetFields();

	Console.WriteLine($"public class {type.Name}");

	Console.WriteLine("{");

	foreach (var field in fields)
	{
		Console.WriteLine($"public {field.FieldType} {field.Name} {{ get; set; }}");
	}

	foreach (var property in properties)
	{
		Console.WriteLine($"public {property.PropertyType.Name} {property.Name} {{ get; set; }}");
	}

	Console.WriteLine("}");
}