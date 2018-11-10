using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text.RegularExpressions;
using TestTypes;

namespace TestGenerator.Generator
{
  public class TestTypeArguments
  {
    public bool SmokeTest { get; set; }
    public bool Devel { get; set; }
    public bool Production { get; set; }
  }

  public class Common
	{
    public static string GetGeneratedTestName(string fileName, TestType testType)
			=> Regex.Replace(fileName, @"(\.cs)$", $".{testType:G}.Generated$1");

		public static bool IsGeneratedTestFile(string fileName)
			=> fileName.Contains(".Generated.");

    public static readonly SyntaxTrivia Space = SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " ");

    public static readonly SyntaxTrivia Empty = SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, "");

    public static readonly SyntaxTrivia NewLine = SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "\r\n");

    public static readonly SyntaxToken Semicolon = SyntaxFactory.Token(Empty.AsList(), SyntaxKind.SemicolonToken, NewLine.AsList());
  }
}
