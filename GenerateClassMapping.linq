<Query Kind="Program">
  <Connection>
    <ID>1ce6f4ec-8b9b-47f1-af86-13fd3d9f7b12</ID>
    <Persist>true</Persist>
    <Driver>EntityFrameworkDbContext</Driver>
    <CustomAssemblyPath>D:\Cloud\Cloud-DEV\WebServers\Insurance2\InsuranceWebApi\InsuranceData\bin\Debug\InsuranceData.dll</CustomAssemblyPath>
    <CustomTypeName>InsuranceData.EF.InsuranceDbContext</CustomTypeName>
    <AppConfigPath>D:\Cloud\Cloud-DEV\WebServers\Insurance2\InsuranceWebApi\InsuranceWebApi\Web.config</AppConfigPath>
    <CustomCxString>Data Source=MC-SQL-CLUSTER1.magaya.com;Initial Catalog=InsuranceDB;Persist Security Info=True;User ID=CIU;Password=P1s@Pr!0riT;Connection Timeout=60;MultipleActiveResultSets=true</CustomCxString>
    <IsProduction>true</IsProduction>
  </Connection>
  <Namespace>InsuranceData.Models</Namespace>
</Query>

void Main()
{
	GenerateClassMapping(typeof(UserModel), typeof(UserBase)).Dump();
}

public string GenerateClassMapping(Type type1, Type type2)
{
	var sb = new StringBuilder();
	var propsMapping = from prop1 in type1.GetProperties()
					   join prop2 in type2.GetProperties()
					   on prop1.Name equals prop2.Name
					   select new { prop1, prop2 };

	var notMappedPropertiesType1 =  type1.GetProperties().Where(a => !type2.GetProperties().Select(b => b.Name).Contains(a.Name));
	var notMappedPropertiesType2 =  type2.GetProperties().Where(a => !type1.GetProperties().Select(b => b.Name).Contains(a.Name));


	if (type1.IsGenericType || type2.IsGenericType) sb.AppendLine("//Check generics types!");

	sb.AppendLine($"private {type2.Name} MapToExists({type1.Name} {type1.Name.ToLower()}, {type2.Name} {type2.Name.ToLower()}){{");

	foreach (var item in propsMapping)
	{
		if (item.prop1.PropertyType != item.prop2.PropertyType && item.prop1.PropertyType.BaseType != item.prop2.PropertyType)
		{
			sb.AppendLine($"// Classes have the same name but different type, probably you need cast types {type2.Name.ToLower()}.{item.prop1.Name} = {type1.Name.ToLower()}.{item.prop1.Name};");
		}
		else
		{
			sb.AppendLine($"{type2.Name.ToLower()}.{item.prop1.Name} = {type1.Name.ToLower()}.{item.prop1.Name};");
		}
	}

	sb.AppendLine($"return {type2.Name.ToLower()};");
	
	sb.AppendLine("}");

	sb.AppendLine("// Not mapped properties");
	foreach (var item in notMappedPropertiesType1)
	{
		sb.AppendLine($"// {item.Name} from {type1.Name}");
	}

	foreach (var item in notMappedPropertiesType2)
	{
		sb.AppendLine($"// {item.Name} from {type2.Name}");
	}

	return sb.ToString();
}