using System.Text.RegularExpressions;
using TestTypes;

namespace TestGenerator.Generator
{
	public class Common
	{
		public static string GetGeneratedTestName(string fileName, TestType testType)
			=> Regex.Replace(fileName, @"(\.cs)$", $".{testType:G}.Generated$1");

		public static bool IsGeneratedTestFile(string fileName)
			=> fileName.Contains(".Generated.");
	}
}
