<Query Kind="Program" />

void Main()
{
	var path = @"D:\Experiments\Insurance\src\app";	
	var files  = Directory.GetFiles(path, "*.ts", SearchOption.AllDirectories);
	
	foreach (var file in files)
	{
		ClearUsings(file);
		file.Dump();
	}
}



public void ClearUsings(string path)
{
	var content = File.ReadAllText(path);
	var lines = File.ReadAllLines(path).ToList();
	var output = new List<string>();

	foreach (var line in lines)
	{
		if (ImportNode.IsValid(line))
		{
			var usng = new ImportNode(line);

			usng.ImportedItems = usng.ImportedItems.Where(x => CodeAnalysisHelper.IsItemPresent(content, x)).ToArray();
			if (usng.ImportedItems.Any())
			{
				output.Add(usng.ToString());
			}
		}
		else
		{
			output.Add(line);
		}
	}
	
	File.WriteAllLines(path, output);
}


public IEnumerable<ImportNode> GetUsings(string path)
{
	foreach (var line in File.ReadAllLines(path))
	{
		if(ImportNode.IsValid(line))
		yield return new ImportNode(line);
	}
}

public class ImportNode
{
	public string[] ImportedItems { get; set; }
	public string Path { get; set; }
	
	public ImportNode()
	{
		
	}

	public ImportNode(string node)
	{
		if (IsValid(node))
		{
			ImportedItems = Regex.Match(node, "import[\\s]*{([\\s\\S]+)}").Groups[1].Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
			Path = Regex.Match(node, "from[\\s]*'([\\s\\S]+)';").Groups[1].Value;
		}
	}

	public static bool IsValid(string node)
	{
		return Regex.IsMatch(node, "[\\s]*import[\\s]*{[\\s\\S]+}[\\s]*from[\\s]*'[\\s\\S]+';") && !node.Trim().StartsWith("/") && !node.Trim().EndsWith("\\");
	}

	public override string ToString()
	{
		return $"import {{ {String.Join(", ", ImportedItems)} }} from '{Path}';";
	}
	
}

public class CodeAnalysisHelper
{
	public static bool IsItemPresent(string code, string item)
	{
		return Regex.Matches(code, $"[\\W]+{item}[\\W]+").Count>1;
	}
}