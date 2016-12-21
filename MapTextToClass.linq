<Query Kind="Program" />

void Main()
{
	var str = "{headerName: \'Contact First Name\', field: \"ContactFirstName\" ,width:180}";

	MapTextToClass<ColumnInfo>(str, $"{{headerName: \'(?<{nameof(ColumnInfo.HeaderName)}>[\\s\\S]+?)\', field: \"(?<{nameof(ColumnInfo.Field)}>[\\s\\S]+?)\"[\\s\\S]*}}").Dump();
}

public IEnumerable<T> MapTextToClass<T>(string text, string pattern, Dictionary<string, Func<string, object>> mappers = null) where T : new()
{
	var properties = (typeof(T)).GetProperties();
	var regex = new Regex(pattern);
	var groupNames = regex.GetGroupNames();

	var matches = regex.Matches(text);

	foreach (Match match in matches)
	{
		var item = new T();
		foreach (var groupName in groupNames)
		{
			var group = match.Groups[groupName];
			var mapper = mappers != null ? (mappers.ContainsKey(groupName) ? mappers[groupName] : null) : null;

			if (group.Success)
			{
				var prop = properties.SingleOrDefault(x => x.Name == groupName);
				if (prop != null)
				{
					prop.SetValue(item, mapper != null ? mapper(group.Value) : group.Value);
				}
			}
		}

		yield return item;
	}
}


public class ColumnInfo
{
	public string HeaderName { get; set; }
	public string Field { get; set; }
}